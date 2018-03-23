using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace dbWorker.Firebird
{
    public class ConfigHandler
    {
        private readonly Dictionary<string, string> _configItems = new Dictionary<string, string>();

        internal ConfigHandler() 
            : this("resources/config/pitaya.ini")
        { }

        public ConfigHandler(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException($"The parameter {nameof(fileName)} may not be empty or whitespace", nameof(fileName));
            }

            if (File.Exists(fileName) == false)
            {
                throw new FileNotFoundException("File could not be found.", fileName);
            }

            _configItems = File.ReadLines(fileName)
                            .Where(IsConfigurationLine)
                            .Select(line => line.Split('='))
                            .ToDictionary(line => line[0], line => line[1]);
        }

        private static bool IsConfigurationLine(string line)
        {
            return !line.StartsWith("#") && line.Contains("=");
        }

        public bool TryGetValue(string key, out string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return _configItems.TryGetValue(key, out value);
        }

        public Dictionary<string, string> GetValues()
        {
            return _configItems;
        }
    }
}