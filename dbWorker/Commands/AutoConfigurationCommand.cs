using McMaster.Extensions.CommandLineUtils;
using System.ComponentModel.DataAnnotations;
using dbWorker;
using dbWorker.Firebird;
using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;

namespace dbWorker.Commands
{
    [Command(Description = "Try to auto configure config file", ThrowOnUnexpectedArgument = false),
    HelpOption]
    public class AutoConfigurationCommand
    {
        public static IEnumerable<string> GetFiles(string root, string searchPattern)
        {
            Stack<string> pending = new Stack<string>();
            pending.Push(root);
            while (pending.Count != 0)
            {
            var path = pending.Pop();
            string[] next = null;
            try
            {
                next = Directory.GetFiles(path, searchPattern);          
            }
            catch { }
            if(next != null && next.Length != 0)
                foreach (var file in next) yield return file;
            try
            {
                next = Directory.GetDirectories(path);
                foreach (var subdir in next) pending.Push(subdir);
            }
            catch { }
            }
        }


        public void OnExecute(IConsole console)
        {
            console.WriteLine("Try to find firebird installations");
            var config = new Config(Statics.ConfigurationPath);
            config.Load();
            
            foreach (var logicalDrive in Directory.GetLogicalDrives())
            {
                console.WriteLine($"Searching at {logicalDrive}");
                IEnumerable<string> fbConfigs = AutoConfigurationCommand.GetFiles(logicalDrive, "firebird.conf");
                if (fbConfigs == null)
                    continue;                
                foreach (var fbConfig in fbConfigs)
                {                
                    console.WriteLine($"Reading {fbConfig}");
                    var port = "3050";
                    var fbConfigHandler = new ConfigHandler(fbConfig);
                    fbConfigHandler.TryGetValue("RemoteServicePort", out port);
                    var connection = new Connection();                
                    connection.Name = Path.GetDirectoryName(fbConfig);
                    connection.Port = Convert.ToInt32(port);                    
                    
                    var aliasConfs = Directory.GetFiles(Path.GetFullPath(fbConfig));
                    if (aliasConfs == null)
                        continue;
                    foreach (var aliasConf in aliasConfs)
                    {
                        console.WriteLine($"Reading {aliasConf}");
                        var aliasConfigHandler = new ConfigHandler(aliasConf);
                        var dbPaths = aliasConfigHandler.GetValues();
                        if (dbPaths == null)
                            continue;
                        foreach (var databasePath in dbPaths)
                        {
                            var database = new Database();
                            database.Name = databasePath.Key;
                            database.Path = databasePath.Value;  
                            connection.Databases.Add(database); 
                        }
                    }
                    config.Connections.Add(connection);
                }    
            }            

            config.Save();
            console.WriteLine("Config created");
        }
    }
}