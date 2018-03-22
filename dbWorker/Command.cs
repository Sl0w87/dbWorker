using FirebirdSql.Data.FirebirdClient;

namespace dbWorker
{
    public static class Command
    {
        public static void ExecuteCommand(Database database, string command)
        {
            using (FbConnection fbConnection = new FbConnection(database.ConnectionString))
            {
                fbConnection.Open();
                try
                {
                    FbCommand fbCommand = fbConnection.CreateCommand();
                    fbCommand.CommandText = command;
                    fbCommand.ExecuteNonQuery();                   
                }
                finally
                {
                    fbConnection.Close();
                }
            }
        }
    }
}