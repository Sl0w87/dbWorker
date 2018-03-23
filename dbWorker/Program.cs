using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using dbWorker.Commands;
using dbWorker.Firebird;
using FirebirdSql.Data.FirebirdClient;
using McMaster.Extensions.CommandLineUtils;

namespace dbWorker
{
    [Command(ThrowOnUnexpectedArgument = false),
    HelpOption,
    Subcommand("AddServer", typeof(AddServerCommand)),
    Subcommand("RemoveServer", typeof(DeleteServerCommand)),
    Subcommand("AddDatabase", typeof(AddDatabaseCommand)),
    Subcommand("RemoveDatabase", typeof(DeleteDatabaseCommand)),
    Subcommand("ExecuteScript", typeof(ExecuteScriptCommand)),
    Subcommand("Manager", typeof(ManagerCommand)),
    Subcommand("AutoConfigure", typeof(AutoConfigurationCommand))]
    class Program
    {
        public static void Main(string[] args) => CommandLineApplication.Execute<Program>(args);
        private void OnExecute()
        {
            // CommandLineApplication.Execute<ManagerCommand>();
            CommandLineApplication.Execute<AutoConfigurationCommand>();
        }              
    }
}
