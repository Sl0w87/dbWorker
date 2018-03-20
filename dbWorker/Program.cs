using System;
using System.IO;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using McMaster.Extensions.CommandLineUtils;

namespace dbWorker
{
    [HelpOption]
    class Program
    {
        public static void Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        [Option(Description="Script path")]
        public string ScriptPath { get; }

        private void OnExecuteAsync()
        {
            var conf = Config.Load();
            var connectionString = conf.Con.ToString();
            var fileCount = 0;
            var scriptFile = "";

            try
            {                
                var scriptPath = ScriptPath?? conf.ScriptPath;
                var files = Directory.GetFiles(scriptPath); 
                if (!String.IsNullOrWhiteSpace(scriptPath) && 
                    Directory.Exists(scriptPath) &&
                    files.Length > 0)
                {                               
                    foreach (var file in files)
                    {
                        fileCount++;                    
                        Console.WriteLine($"{fileCount} = {Path.GetFileName(file)}");
                    }
                    var fileIdent = Prompt.GetInt("Choose script:");
                    scriptFile = File.ReadAllText(files[--fileIdent]);
                }            
            }
            catch(Exception ex)
            {
                Console.WriteLine(String.Concat("Error while reading script file.", Environment.NewLine, ex.Message));
                return;
            }

            if (String.IsNullOrWhiteSpace(scriptFile) || 
                File.Exists(scriptFile))
            {
                Console.WriteLine("Not a valid script file.");
                return;
            }

            try
            {
                using (FbConnection fbConnection = new FbConnection(connectionString))
                {
                    if (fbConnection == null)
                    {
                        Console.WriteLine("Can't create a connection.");
                        return;
                    }
                    fbConnection.Open();
                    try
                    {
                        try
                        {   
                            FbCommand fbCommand = fbConnection.CreateCommand();
                            fbCommand.CommandText = scriptFile;
                            fbCommand.ExecuteNonQuery();
                        }
                        catch (Exception Ex)
                        {
                            Console.WriteLine(Ex.Message);
                            return;
                        }
                    }
                    finally
                    {
                        fbConnection.Close();
                    }
                }
            }            
            catch(Exception ex)
            {
                Console.WriteLine(String.Concat("Error while trying to communicate with database.", Environment.NewLine, ex.Message));
                return;
            }
        }
    }
}
