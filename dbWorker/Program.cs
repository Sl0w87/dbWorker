using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using McMaster.Extensions.CommandLineUtils;

namespace dbWorker
{
    [HelpOption]
    class Program
    {
        [Option(Description="Script path")]
        public string ScriptPath { get; }
        public static void Main(string[] args) => CommandLineApplication.Execute<Program>(args);
        private Config loadConfig()
        {         
            Console.WriteLine("Load config file");
            var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var config = new Config(path);
            config.Load();                
            Console.WriteLine("Config loaded");
            return config;
        }
        private Connection ChooseConnection(List<Connection> connections)
        {
            Console.WriteLine("Server:");
            var index = 0;
            foreach (var item in connections)
            {
                index++;
                Console.WriteLine($"[{index}]: {item.DataSource}/{item.Port}");
            } 
            var connectionIdent = Prompt.GetInt("Choose connection: ");

            return connections[--connectionIdent];
        }
        private Database ChooseDatabase(Connection connection)
        {
            Console.WriteLine("Databases:");
            var index = 0;
            foreach (var item in connection.Databases)
            {
                index++;
                Console.WriteLine($"[{index}]: {item.Path}");
            } 
            var databaseIdent = Prompt.GetInt("Choose database: ");

            return connection.Databases[--databaseIdent];
        }
        private string ChooseScriptFile(string scriptPath)
        {
            Console.WriteLine("Script files:");
            var files = Directory.GetFiles(scriptPath); 
            var index = 0;             
            foreach (var file in files)
            {
                index++;                    
                Console.WriteLine($"[{index}]: {Path.GetFileName(file)}");
            }
            var fileIdent = Prompt.GetInt("Choose script: ");
            return files[--fileIdent];
        }
        private void firstConfiguration(ref Config conf)
        {
            Console.WriteLine("Configure server");
            var serverType = Prompt.GetInt("ServerType (0 = remote; 1 = embedded) [default = 0]: ");
            var dataSource = Prompt.GetString("DataSource [default = localhost]: ");
            var port = Prompt.GetInt("Port [default = 3050]: ");

            Console.WriteLine("Configure database");
            var databasePath = Prompt.GetString("Database path: ");
            var user = Prompt.GetString("User [default = SYSDBA]: ");
            var password = Prompt.GetPassword("Password [default = masterkey]: ");
            
            var con = new Connection();
            con.ServerType = serverType;
            con.DataSource = dataSource;
            con.Port = port;
            var database = new Database(con);
            database.Password = password;
            database.Path = databasePath;
            database.User = user;
            con.Databases.Add(database);
            conf.Connections.Add(con);        
        }
        private void OnExecuteAsync()
        {
            Config conf;
            try
            {
                conf = loadConfig();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading config file: {ex.Message}");
                return;
            }
            if (conf.Connections == null || conf.Connections.Count == 0)
            {
                try
                {                    
                    firstConfiguration(ref conf);
                    conf.Save();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error at first configuration: {ex.Message}");
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
                Console.WriteLine($"Error on getting connection: {ex.Message}");
                return;
            }
            if (connection == null)
            {
                Console.WriteLine("Not a valid connection.");
                return;
            }
            Database database;
            try
            {
                database = ChooseDatabase(connection);                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on getting database: {ex.Message}");
                return;
            }
            if (database == null)
            {
                Console.WriteLine("Not a valid database.");
                return;
            }
            string scriptFile;
            try
            {
                scriptFile = ChooseScriptFile(conf.ScriptPath);                            
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on getting script file: {ex.Message}");
                return;
            }
            if (scriptFile.Length == 0)
            {
                Console.WriteLine("Not a valid script file.");
                return;
            }
            string scriptText;
            try
            {                
                scriptText = File.ReadAllText(scriptFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on reading script file: {ex.Message}");
                return;
            }
            if (scriptFile.Length == 0)
            {
                Console.WriteLine("Script file is empty.");
                return;
            }            
            try
            {
               Command.ExecuteCommand(database, scriptText);
            }            
            catch(Exception ex)
            {
                Console.WriteLine(String.Concat("Error while trying to communicate with database.", Environment.NewLine, ex.Message));
                return;
            }
            Console.WriteLine("Script executed");
        }
    }
}
