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
            var fileCount = 0;
            var scriptFile = "";

            try
            {                
                var scriptPath = ScriptPath?? conf.ScriptPath;
                if (!String.IsNullOrWhiteSpace(scriptPath) && 
                    Directory.Exists(scriptPath))
                {
                    var files = Directory.GetFiles(scriptPath);
                    foreach (var file in files)
                    {
                        fileCount++;
                        Console.WriteLine(fileCount + ":" + file);
                    }
                    var fileIdent = Prompt.GetInt("Choose script:");
                    scriptFile = File.ReadAllText(files[fileIdent]);
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
                var connecion = new Connection();
                using (FbConnection fbConnection = new FbConnection(connecion.ToString()))
                {
                    if (fbConnection == null)
                    {
                        Console.WriteLine("Can't create a connection.");
                        return;
                    }
                    fbConnection.Open();
                    try
                    {
                        FbCommand fbCommand = new FbCommand(scriptFile, fbConnection);
                        fbCommand.ExecuteNonQuery();
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
