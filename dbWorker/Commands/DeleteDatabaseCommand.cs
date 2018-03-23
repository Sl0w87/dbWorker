using McMaster.Extensions.CommandLineUtils;
using System.ComponentModel.DataAnnotations;
using dbWorker;
using dbWorker.Firebird;

namespace dbWorker.Commands
{
    [Command(Description = "Remove a database from a connection", ThrowOnUnexpectedArgument = false),
    HelpOption]
    public class DeleteDatabaseCommand
    {
        [Option("-sn|--servername", Description = "Name of the server in the config"), Required]
        public string ServerName { get; set; }    
        [Option("-n|--name", Description = "Name of the database in the config"), Required]
        public string Name { get; set; }
        public void OnExecute(IConsole console)
        {
            console.WriteLine("Remove a database from a connection");
            var config = new Config(Statics.ConfigurationPath);
            config.Load();
            var connection = config.Connections.Find(s => s.Name == ServerName);
            if (connection == null)
            {
                console.WriteLine($"Connection with name '{ServerName}' can't be found in the configuration file");
                return;
            }
            var database = connection.Databases.RemoveAll(d => d.Name == Name);
            config.Save();
            console.WriteLine("Database removed");
        }
    }
}