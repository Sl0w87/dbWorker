namespace dbWorker
{
    public class Database
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string Path { get; set; }
        public int Dialect { get; set; }
        public string Charset { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public string GetConnectionString(Connection connection) 
        { 
            return 
                $"User={User};" +
                $"Password={Password};" +
                $"Database={Path};" +
                $"DataSource={connection.DataSource};" +
                $"Port={connection.Port};" +
                $"Dialect={Dialect};" +
                $"Charset={Charset};" +
                $"Role={Role};" +
                $"Pooling={connection.Pooling};" +
                $"MinPoolSize={connection.MinPoolSize};" +
                $"MaxPoolSize={connection.MaxPoolSize};" +
                $"PackSize={connection.PackSize};" +
                $"ServerType={connection.ServerType};";
        }
        public Database()
        {            
            User = "SYSDBA";
            Password = "masterkey";
            Dialect = 3;
            Charset = "NONE";  
        }
    }
}