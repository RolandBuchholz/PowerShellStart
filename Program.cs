using System;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;

namespace PowerShellStart
{
    class Program
    {

        static DownloadInfo downloadInfo = new();

        public static int ExitCode { get; private set; }

        static int Main(string[] args)
        {
            string starttyp = string.Empty;
            string auftragsnummer = string.Empty;
            bool readOnly = false;
            const string pathPowershellScripts = @"C:\Work\Administration\PowerShellScripts\";

            //Befehlszeilenargumente auslesen 
            string[] commandLineArgs = Environment.GetCommandLineArgs();
            //CommandLineArgs[0]: Immer der Dateipfad 
            if (commandLineArgs.Length > 1)
            {
                if (commandLineArgs[1] == "get" | commandLineArgs[1] == "set" | commandLineArgs[1] == "undo")
                {
                    starttyp = commandLineArgs[1];
                    auftragsnummer = commandLineArgs[2];
                    if (commandLineArgs.Length >= 4)
                    {
                        readOnly = Convert.ToBoolean(commandLineArgs[3]);
                    }
                }
                else
                {
                    auftragsnummer = commandLineArgs[1];
                }
            }

            Stopwatch watch = Stopwatch.StartNew();
            Task<int> result = StartPowershellScript(pathPowershellScripts, starttyp, auftragsnummer, readOnly);
            ExitCode = result.Result;

            downloadInfo.ExitCode = ExitCode;
            long stopTimeMs = watch.ElapsedMilliseconds;
            Console.WriteLine("---DownloadInfo---");
            Console.WriteLine(JsonSerializer.Serialize<DownloadInfo>(downloadInfo));
            Console.WriteLine("---DownloadInfo---");
            Console.WriteLine($"Downloadtime: {stopTimeMs} ms");

            return ExitCode;
        }

        static async Task<int> StartPowershellScript(string pathPowershellScripts, string starttyp, string auftragsnummer, bool readOnly)
        {
            string powershellScriptName;

            switch (starttyp)
            {
                case "get":
                    powershellScriptName = "GetVaultFile.ps1";
                    break;
                case "set":
                    powershellScriptName = "SetVaultFile.ps1";
                    break;
                case "undo":
                    powershellScriptName = "UndoVaultFile.ps1";
                    break;
                default:
                    powershellScriptName = "GetVaultFile.ps1";
                    break;
            }

            string readOnlyPowershell = readOnly ? "$true" : "$false";

            try
            {
                using Process psScript = new();
                psScript.StartInfo.UseShellExecute = false;
                psScript.StartInfo.FileName = "PowerShell.exe";
                if (starttyp == "get")
                {
                    psScript.StartInfo.Arguments = $"{pathPowershellScripts}{powershellScriptName} {auftragsnummer} {readOnlyPowershell}";
                }
                else
                {
                    psScript.StartInfo.Arguments = $"{pathPowershellScripts}{powershellScriptName} {auftragsnummer}";
                }
                psScript.StartInfo.CreateNoWindow = true;
                psScript.StartInfo.RedirectStandardOutput = true;
                psScript.Start();
                string downloadResult = await psScript.StandardOutput.ReadToEndAsync();
                psScript.WaitForExit();
                
                if (!string.IsNullOrWhiteSpace(downloadResult))
                {
                    try
                    {
                        downloadInfo = JsonSerializer.Deserialize<DownloadInfo>(downloadResult.Split("---DownloadInfo---")[1]);
                    }
                    catch
                    {
                        Console.WriteLine("Keine DownloadInfo gefunden");
                    }
                }

                Console.WriteLine($"Vault PowershellScript: {powershellScriptName} finished ...");
                await Task.CompletedTask;
                return psScript.ExitCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await Task.CompletedTask;
                return 4;
            }
        }
    }
}