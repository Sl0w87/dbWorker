using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using dbWorker.Firebird;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace dbWorker
{
    public class Config
    {        
        readonly string _path;
        public List<Connection> Connections { get; set; }
        public string ScriptPath { get; set; }
        private readonly string ConfigPath;

        private Config()
        {

        }        
        public Config(string path)
        {
            _path = path;
            if (_path != null)
            {
                ConfigPath = Path.Combine(_path, "config.json");
                ScriptPath = Path.Combine(_path, "Scripts");
            }
            Connections = new List<Connection>();
        }
        public bool Exist()
        {
            return File.Exists(ConfigPath);
        }
        public void Load(bool createScriptFileFolder = true)
        {
            if (createScriptFileFolder && ScriptPath != null && !Directory.Exists(ScriptPath))
                Directory.CreateDirectory(ScriptPath);

            if (Exist())
            {
                string jsonFile =  File.ReadAllText(ConfigPath);
                
                var config = JsonConvert.DeserializeObject<Config>(jsonFile);
                this.Connections = config.Connections;
                this.ScriptPath = config.ScriptPath;
            }
        }

        public void Save()
        {
            var jsonFile = JsonConvert.SerializeObject(this, formatting: Formatting.Indented);
            File.WriteAllText(ConfigPath, jsonFile);
        }
    }
}