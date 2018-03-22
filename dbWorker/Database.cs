namespace dbWorker
{
    public class Database
    {
        Connection _connection { get; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Path { get; set; }
        public int Dialect { get; set; }
        public string Charset { get; set; }
        public string Role { get; set; }
        public string ConnectionString 
        { 
            get 
            {
                return 
                    $"User={User};" +
                    $"Password={Password};" +
                    $"Database={Path};" +
                    $"DataSource={_connection.DataSource};" +
                    $"Port={_connection.Port};" +
                    $"Dialect={Dialect};" +
                    $"Charset={Charset};" +
                    $"Role={Role};" +
                    $"Pooling={_connection.Pooling};" +
                    $"MinPoolSize={_connection.MinPoolSize};" +
                    $"MaxPoolSize={_connection.MaxPoolSize};" +
                    $"PackSize={_connection.PackSize};" +
                    $"ServerType={_connection.ServerType};";
            }
        }
        public Database(Connection connection)
        {            
            _connection = connection;
            User = "SYSDBA";
            Password = "masterkey";
            Dialect = 3;
            Charset = "NONE";  
        }
    }
}