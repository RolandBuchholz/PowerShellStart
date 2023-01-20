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
            string starttyp;
            string auftragsnummer;
            bool readOnly = false;
            bool customFile = false;
            const string pathPowershellScripts = @"C:\Work\Administration\PowerShellScripts\";

            //Befehlszeilenargumente auslesen 
            string[] commandLineArgs = Environment.GetCommandLineArgs();

            //CommandLineArgs[0]: Immer der Dateipfad 
            if (commandLineArgs.Length > 2)
            {
                starttyp = commandLineArgs[1].ToLower();
                switch (starttyp)
                {
                    case "get":
                        if (commandLineArgs.Length == 3)
                        {
                            auftragsnummer = commandLineArgs[2];
                        }
                        else if ((commandLineArgs.Length == 4))
                        {
                            auftragsnummer = commandLineArgs[2];
                            readOnly = Convert.ToBoolean(commandLineArgs[3]);
                        }
                        else
                        {
                            auftragsnummer = commandLineArgs[2];
                            readOnly = Convert.ToBoolean(commandLineArgs[3]);
                            customFile = Convert.ToBoolean(commandLineArgs[4]);
                        }
                        break;
                    case "set" or "undo":
                        if (commandLineArgs.Length == 3)
                        {
                            auftragsnummer = commandLineArgs[2];
                        }
                        else
                        {
                            auftragsnummer = commandLineArgs[2];
                            customFile = Convert.ToBoolean(commandLineArgs[3]);
                        }
                        break;
                    default:
                        return 0;
                }
            }
            else
            {
                if (commandLineArgs.Length == 2)
                {
                    starttyp = "get";
                    auftragsnummer = commandLineArgs[1];
                }
                else
                {
                    return 0;
                }
            }

            Stopwatch watch = Stopwatch.StartNew();
            Task<int> result = StartPowershellScript(pathPowershellScripts, starttyp, auftragsnummer, readOnly, customFile);
            ExitCode = result.Result;

            downloadInfo.ExitCode = ExitCode;
            long stopTimeMs = watch.ElapsedMilliseconds;
            Console.WriteLine("---DownloadInfo---");
            Console.WriteLine(JsonSerializer.Serialize(downloadInfo));
            Console.WriteLine("---DownloadInfo---");
            Console.WriteLine($"Downloadtime: {stopTimeMs} ms");

            return ExitCode;
        }

        static async Task<int> StartPowershellScript(string pathPowershellScripts, string starttyp, string auftragsnummer, bool readOnly, bool customFile)
        {
            string powershellScriptName = starttyp switch
            {
                "get" => "GetVaultFile.ps1",
                "set" => "SetVaultFile.ps1",
                "undo" => "UndoVaultFile.ps1",
                _ => "GetVaultFile.ps1",
            };
            string readOnlyPowershell = readOnly ? "$true" : "$false";
            string customFilePowershell = customFile ? "$true" : "$false";

            try
            {
                using Process psScript = new();
                psScript.StartInfo.UseShellExecute = false;
                psScript.StartInfo.FileName = "PowerShell.exe";
                if (starttyp == "get")
                {
                    psScript.StartInfo.Arguments = $"{pathPowershellScripts}{powershellScriptName} {auftragsnummer} {readOnlyPowershell} {customFilePowershell}";
                }
                else
                {
                    psScript.StartInfo.Arguments = $"{pathPowershellScripts}{powershellScriptName} {auftragsnummer} {customFilePowershell}";
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