using System.IO;
using System.Reflection;

namespace dbWorker
{
    static class Statics
    {
        public static string ConfigurationPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
    }
}