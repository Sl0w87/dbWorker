using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace dbWorker
{
    public class Config
    {
        static readonly string appPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        static readonly string configPath = Path.Combine(appPath, "config.json");
        static readonly string scriptPath = Path.Combine(appPath, "Scripts");
        public Connection Con { get; set; }
        public string ScriptPath { get; set; }

        public Config()
        {
            ScriptPath = scriptPath;
            Con = new Connection();
        }

        public static Config Load()
        {
            Console.WriteLine(configPath);
            string jsonFile = "";
            if (!File.Exists(configPath))
            {
                jsonFile = JsonConvert.SerializeObject(new Config(), formatting: Formatting.Indented);
                File.WriteAllText(configPath, jsonFile);
            }
            else
                jsonFile = File.ReadAllText(configPath);
            return JsonConvert.DeserializeObject<Config>(jsonFile);
        }
    }
}