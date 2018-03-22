using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace dbWorker
{
    public class Config
    {        
        readonly string _path;
        readonly string _configPath;
        public List<Connection> Connections { get; set; }
        public string ScriptPath { get; set; }

        private Config()
        {

        }        
        public Config(string path)
        {
            _path = path;
            if (_path != null)
            {
                _configPath = Path.Combine(_path, "config.json");
                ScriptPath = Path.Combine(_path, "Scripts");
            }
            Connections = new List<Connection>();
        }
        public bool Exist()
        {
            return File.Exists(_configPath);
        }
        public void Load(bool createScriptFileFolder = true)
        {
            if (createScriptFileFolder && ScriptPath != null && !Directory.Exists(ScriptPath))
                Directory.CreateDirectory(ScriptPath);

            if (Exist())
            {
                string jsonFile =  File.ReadAllText(_configPath);
                
                var config = JsonConvert.DeserializeObject<Config>(jsonFile);
                this.Connections = config.Connections;
                this.ScriptPath = config.ScriptPath;
            }
        }

        public void Save()
        {
            var jsonFile = JsonConvert.SerializeObject(this, formatting: Formatting.Indented);
            File.WriteAllText(_configPath, jsonFile);
        }
    }
}