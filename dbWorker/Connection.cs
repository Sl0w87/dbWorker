namespace dbWorker
{
    public class Connection
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        public string DataSource { get; set; }
        public int Port { get; set; }
        public int Dialect { get; set; }
        public string Charset { get; set; }
        public string Role { get; set; }
        public string _Connection { get; set; }
        public bool Pooling { get; set; }
        public int MinPoolSize { get; set; }
        public int MaxPoolSize { get; set; }
        public int PackSize { get; set; }
        public int ServerType { get; set; }

        public Connection()
        {
            User = "SYSDBA";
            Password = "masterkey";
            DataSource = "localhost";
            Port = 3050;
            Dialect = 3;
            Charset = "NONE";                    
        }

        public override string ToString()
        {            
            return 
                $"User={User}" +
                $"Password={Password}" +
                $"Database={Database}" +
                $"DataSource={DataSource}" +
                $"Port={Port}" +
                $"Dialect={Dialect}" +
                $"Charset={Charset}" +
                $"Role={Role}" +
                $"Connection={_Connection}" +
                $"Pooling={Pooling}" +
                $"MinPoolSize={MinPoolSize}" +
                $"MaxPoolSize={MaxPoolSize}" +
                $"PackSize={PackSize}" +
                $"ServerType={ServerType}";
        }
    }
}