using McMaster.Extensions.CommandLineUtils;
using System.ComponentModel.DataAnnotations;
using dbWorker;
using dbWorker.Firebird;

namespace dbWorker.Commands
{
    [Command(Description = "Remove a connection", ThrowOnUnexpectedArgument = false),
    HelpOption]
    public class DeleteServerCommand
    {
        [Option("-n|--name", Description = "Name of the connection to remove from configuration file"), Required]
        public string Name { get; set; }
        public void OnExecute(IConsole console)
        {
            console.WriteLine("Remove connections from configuration file");
            var config = new Config(Statics.ConfigurationPath);
            config.Load();
            config.Connections.RemoveAll(n => n.Name == Name);
            config.Save();
            console.WriteLine("Connections removed");
        }
    }
}