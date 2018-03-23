using McMaster.Extensions.CommandLineUtils;
using System.ComponentModel.DataAnnotations;
using dbWorker;
using dbWorker.Firebird;

namespace dbWorker.Commands
{
    [Command(Description = "Add a database to a connection", ThrowOnUnexpectedArgument = false),
    HelpOption]
    public class AddDatabaseCommand
    {
        [Option("-sn|--servername", Description = "Name of the connection in the config"), Required]
        public string ServerName { get; set; }        
        [Option("-u|--user", Description = "Database user name"), Required]
        public string User { get; set; }
        [Option("-pw|--password", Description = "Database user password"), Required]
        public string Password { get; set; }
        [Option("-p|--path", Description = "Database path"), Required]
        public string Path { get; set; }
        [Option("-d|--dialect", Description = "Database dialect")]
        public int Dialect { get; set; }
        [Option("-c|--charset", Description = "Database charset")]
        public string Charset { get; set; }
        [Option("-r|--role", Description = "Database user role")]
        public string Role { get; set; }
        [Option("-n|--name", Description = "Name of the Database in the configuration file"), Required]
        public string Name { get; set; }
        public void OnExecute(IConsole console)
        {
            console.WriteLine("Add a database to a connection");
            var config = new Config(Statics.ConfigurationPath);
            config.Load();
            var connection = config.Connections.Find(s => s.Name == ServerName);
            if (connection == null)
            {
                console.WriteLine($"Connection with name '{ServerName}' can't be found in the configuration file");
                return;
            }
            var database = new Database();
            database.Charset = Charset;
            database.Dialect = Dialect;
            database.Name = Name;
            database.Password = Password;
            database.Path = Path;
            database.Role = Role;
            database.User = User;
            connection.Databases.Add(database);
            config.Save();
            console.WriteLine("Database added");
        }
    }
}