using McMaster.Extensions.CommandLineUtils;
using System.ComponentModel.DataAnnotations;
using dbWorker;
using dbWorker.Firebird;

namespace dbWorker.Commands
{
    [Command(Description = "Add a connection", ThrowOnUnexpectedArgument = false),
    HelpOption]
    public class AddServerCommand
    {
        [Option("-n|--name", Description = "Name of the server"), Required]
        public string Name { get; set; }
        [Option("-ds|--datasource", Description = "Path to the server"), Required]
        public string DataSource { get; set; }
        [Option("-p|--port", Description = "Port for the connection"), Required]
        public int Port { get; set; }
        [Option("-pl|--pooling", Description = "Use Pooling?")]
        public bool Pooling { get; set; }
        [Option("-minps|--minpoolsize", Description = "Minimum pooling size")]
        public int MinPoolSize { get; set; }
        [Option("-maxps|--maxpoolsize", Description = "Maximum pooling size")]
        public int MaxPoolSize { get; set; }
        [Option("-ps|--packsize", Description = "Pack size")]
        public int PackSize { get; set; }
        [Option("-st|--servertype", Description = "Server type (0=remote; 1=embedded)"), Required]
        public int ServerType { get; set; }
        public void OnExecute(IConsole console)
        {
            console.WriteLine("Add new connection to config file");
            var config = new Config(Statics.ConfigurationPath);
            config.Load();
            var connection = new Connection();
            connection.DataSource = DataSource;
            connection.MaxPoolSize = MaxPoolSize;
            connection.MinPoolSize = MinPoolSize;
            connection.Name = Name;
            connection.PackSize = PackSize;
            connection.Pooling = Pooling;
            connection.Port = Port;
            connection.ServerType = ServerType;
            config.Connections.Add(connection);
            config.Save();
            console.WriteLine("New connection added");
        }
    }
}