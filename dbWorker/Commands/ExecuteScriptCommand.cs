using McMaster.Extensions.CommandLineUtils;
using System.ComponentModel.DataAnnotations;
using dbWorker;
using dbWorker.Firebird;
using System.IO;
using System;

namespace dbWorker.Commands
{
    [Command(Description = "Execute a script", ThrowOnUnexpectedArgument = false),
    HelpOption]
    public class ExecuteScriptCommand
    {
        [Option("-sn|--servername", Description = "Name of the connection from configuration file"), Required]
        public string ServerName { get; set; }
        [Option("-dn|--databasename", Description = "Name of the database from configuration file"), Required]
        public string DatabaseName { get; set; }
        [Option("-s|--script", Description = "Path of the script"), Required]
        public string Script { get; set; }
        public void OnExecute(IConsole console)
        {
            console.WriteLine("Execute script");
            var config = new Config(Statics.ConfigurationPath);
            config.Load();

            var connection = config.Connections.Find(c => c.Name == ServerName);
            if (connection == null)
            {
                console.WriteLine($"Connection with name '{ServerName}' can't be found in the configuration file");
                return;
            }

            var database = connection.Databases.Find(d => d.Name == DatabaseName);
            if (database == null)
            {
                console.WriteLine($"Database with name '{DatabaseName}' can't be found in the configuration file");
                return;
            }

            var scriptText = File.ReadAllText(Script);
            if (scriptText.Length == 0)
            {
                console.WriteLine($"Script is empty");
                return;
            }

            var rowsAffected = Firebird.Command.ExecuteCommand(connection, database, scriptText);
            console.WriteLine("Script executed" + Environment.NewLine + $"Rows affected: {rowsAffected}");
        }
    }
}