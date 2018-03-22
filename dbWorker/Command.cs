using FirebirdSql.Data.FirebirdClient;

namespace dbWorker
{
    public static class Command
    {
        public static int ExecuteCommand(Connection connection, Database database, string command)
        {
            using (FbConnection fbConnection = new FbConnection(database.GetConnectionString(connection)))
            {
                fbConnection.Open();
                try
                {
                    FbCommand fbCommand = fbConnection.CreateCommand();
                    fbCommand.CommandText = command;
                    return fbCommand.ExecuteNonQuery();                   
                }
                finally
                {
                    fbConnection.Close();
                }
            }
        }
    }
}