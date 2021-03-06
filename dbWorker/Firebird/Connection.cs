using System.Collections.Generic;

namespace dbWorker.Firebird
{
    public class Connection
    {
        public string Name { get; set; }
        public string DataSource { get; set; }
        public int Port { get; set; }
        public bool Pooling { get; set; }
        public int MinPoolSize { get; set; }
        public int MaxPoolSize { get; set; }
        public int PackSize { get; set; }
        public int ServerType { get; set; }
        public List<Database> Databases { get; set; }
        public Connection()
        {
            DataSource = "localhost";
            Port = 3050;
            Databases = new List<Database>();
        }
    }
}