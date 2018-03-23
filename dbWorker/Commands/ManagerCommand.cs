using System;
using System.Collections.Generic;
using System.IO;
using dbWorker.Firebird;
using McMaster.Extensions.CommandLineUtils;

namespace dbWorker.Commands
{
    [Command("Manager"),
    HelpOption]
    public class ManagerCommand
    {
        IConsole _console;

        public void OnExecute(IConsole console)
        {
            _console = console;
            Config conf;
            try
            {
                conf = loadConfig();
            }
            catch (Exception ex)
            {
                console.WriteLine($"Error loading config file: {ex.Message}");
                return;
            }
            if (conf.Connections == null || conf.Connections.Count == 0)
            {
                try
                {                    
                    var server = configureNewServer();
                    var db = configureNewDatabase();
                    server.Databases.Add(db);
                    conf.Connections.Add(server);
                    conf.Save();
                }
                catch (Exception ex)
                {
                    console.WriteLine($"Error at first configuration: {ex.Message}");
                    return;
                }
            }
            Connection connection;
            try
            {
                connection = ChooseConnection(conf.Connections);                
            }
            catch (Exception ex)
            {
                console.WriteLine($"Error on getting connection: {ex.Message}");
                return;
            }
            if (connection == null)
            {
                console.WriteLine("Not a valid connection.");
                return;
            }
            Database database;
            try
            {
                database = ChooseDatabase(connection);                
            }
            catch (Exception ex)
            {
                console.WriteLine($"Error on getting database: {ex.Message}");
                return;
            }
            if (database == null)
            {
                console.WriteLine("Not a valid database.");
                return;
            }

            do
            {
                string scriptFile;
                try
                {
                    scriptFile = ChooseScriptFile(conf.ScriptPath);                            
                }
                catch (Exception ex)
                {
                    console.WriteLine($"Error on getting script file: {ex.Message}");
                    return;
                }
                if (scriptFile.Length == 0)
                {
                    console.WriteLine("Not a valid script file.");
                    return;
                }
                ExecuteScript(connection, database, scriptFile);
            } while (Prompt.GetYesNo("Execute another script?", true));            
        }
        private Config loadConfig()
        {         
            _console.WriteLine("Load config file");
            var path = Statics.ConfigurationPath;
            var config = new Config(path);
            config.Load();                
            _console.WriteLine("Config loaded");
            return config;
        }
        private Connection ChooseConnection(List<Connection> connections)
        {
            _console.WriteLine("Server:");
            var index = 0;
            foreach (var item in connections)
            {
                index++;
                _console.WriteLine($"[{index}]: {item.Name} ({item.DataSource}/{item.Port})");
            } 
            var connectionIdent = Prompt.GetInt("Choose connection: ");

            return connections[--connectionIdent];
        }
        private Database ChooseDatabase(Connection connection)
        {
            _console.WriteLine("Databases:");
            var index = 0;
            foreach (var item in connection.Databases)
            {
                index++;
                _console.WriteLine($"[{index}]: {item.Name} ({item.Path})");
            } 
            var databaseIdent = Prompt.GetInt("Choose database: ");

            return connection.Databases[--databaseIdent];
        }
        private string ChooseScriptFile(string scriptPath)
        {
            _console.WriteLine("Script files:");
            var files = Directory.GetFiles(scriptPath); 
            var index = 0;             
            foreach (var file in files)
            {
                index++;                    
                _console.WriteLine($"[{index}]: {Path.GetFileName(file)}");
            }
            var fileIdent = Prompt.GetInt("Choose script: ");
            return files[--fileIdent];
        }
        private Connection configureNewServer()
        {
            _console.WriteLine("Configure server");
            var serverType = Prompt.GetInt("ServerType (0 = remote; 1 = embedded): ", 0);
            var dataSource = Prompt.GetString("DataSource: ", "localhost");
            var port = Prompt.GetInt("Port: ", 3050);
            var connectionName = Prompt.GetString("Name: ");
            
            var con = new Connection();
            con.ServerType = serverType;
            con.DataSource = dataSource;
            con.Port = port;
            con.Name = connectionName;
            return con;
        }
        private Database configureNewDatabase()
        {            
            _console.WriteLine("Configure database");
            var databasePath = Prompt.GetString("Database path: ");
            var user = Prompt.GetString("User: ", "SYSDBA");
            var password = Prompt.GetString("Password: ", "masterkey");
            var databaseName = Prompt.GetString("Name: ");
            var database = new Database();
            database.Password = password;
            database.Path = databasePath;
            database.User = user;
            database.Name = databaseName;
            return database;
        }
        private void ExecuteScript(Connection connection, Database database, string scriptFile)
        {
            string scriptText;
            try
            {                
                scriptText = File.ReadAllText(scriptFile);
            }
            catch (Exception ex)
            {
                _console.WriteLine($"Error on reading script file: {ex.Message}");
                return;
            }
            if (scriptFile.Length == 0)
            {
                _console.WriteLine("Script file is empty.");
                return;
            }            
            try
            {
                var rowsAffected = Command.ExecuteCommand(connection, database, scriptText);
                _console.WriteLine("Script executed: Rows affected = {rowsAffected}");
            }            
            catch(Exception ex)
            {
                _console.WriteLine(String.Concat("Error while trying to communicate with database.", Environment.NewLine, ex.Message));
                return;
            }
        }
    }
}