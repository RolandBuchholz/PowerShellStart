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
            string pathPowershellScripts;
            pathPowershellScripts = @"C:\Work\Administration\PowerShellScripts\";
            //pathPowershellScripts = @"C:\Users\Buchholz.PPS\source\BE\PS_Scripts\BE_PS_Scripts\PS_Scripts\";

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

            Task<int> result;
            switch (starttyp)
            {
                case "get":
                    result = GetFileAsync(pathPowershellScripts, auftragsnummer, readOnly);
                    ExitCode = result.Result;
                    break;
                case "set":
                    result = SetFileAsync(pathPowershellScripts, auftragsnummer);
                    ExitCode = result.Result;
                    break;
                case "undo":
                    result = UndoFileAsync(pathPowershellScripts, auftragsnummer);
                    ExitCode = result.Result;
                    break;
                default:
                    result = GetFileAsync(pathPowershellScripts, auftragsnummer, readOnly);
                    ExitCode = result.Result;
                    break;
            }
            downloadInfo.ExitCode = ExitCode;
            long stopTimeMs = watch.ElapsedMilliseconds;
            Console.WriteLine("---DownloadInfo---");
            //JsonSerializerOptions jso = new JsonSerializerOptions();
            //jso.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            Console.WriteLine(JsonSerializer.Serialize<DownloadInfo>(downloadInfo));
            Console.WriteLine("---DownloadInfo---");
            Console.WriteLine($"Downloadtime: {stopTimeMs} ms");

            return ExitCode;
        }

        static async Task<int> GetFileAsync(string pathPowershellScripts, string auftragsnummer, bool readOnly)
        {
            string powershellScriptName = "GetVaultFile.ps1";
            string readOnlyPowershell = readOnly ? "$true" : "$false";

            try
            {
                using Process getFile = new();
                getFile.StartInfo.UseShellExecute = false;
                getFile.StartInfo.FileName = "PowerShell.exe";
                getFile.StartInfo.Arguments = $"{pathPowershellScripts}{powershellScriptName} {auftragsnummer} {readOnlyPowershell}";
                getFile.StartInfo.CreateNoWindow = false;
                getFile.StartInfo.RedirectStandardOutput = true;
                getFile.Start();
                getFile.WaitForExit();
                string downloadResult = await getFile.StandardOutput.ReadToEndAsync();
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
                
                Console.WriteLine("GetVaultFile finished ...");
                await Task.CompletedTask;
                return getFile.ExitCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await Task.CompletedTask;
                return 4;
            }
        }
        static async Task<int> SetFileAsync(string pathPowershellScripts, string auftragsnummer)
        {
            string powershellScriptName = "SetVaultFile.ps1";

            try
            {
                using Process getFile = new();
                getFile.StartInfo.UseShellExecute = false;
                getFile.StartInfo.FileName = "PowerShell.exe";
                getFile.StartInfo.Arguments = $"{pathPowershellScripts}{powershellScriptName} {auftragsnummer}";
                getFile.StartInfo.CreateNoWindow = false;
                getFile.StartInfo.RedirectStandardOutput = true;
                getFile.Start();
                getFile.WaitForExit();
                string downloadResult = await getFile.StandardOutput.ReadToEndAsync();
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

                Console.WriteLine("GetVaultFile finished ...");
                await Task.CompletedTask;
                return getFile.ExitCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await Task.CompletedTask;
                return 4;
            }
        }
        static async Task<int> UndoFileAsync(string pathPowershellScripts, string auftragsnummer)
        {
            string powershellScriptName = "UndoVaultFile.ps1";

            try
            {
                using Process getFile = new();
                getFile.StartInfo.UseShellExecute = false;
                getFile.StartInfo.FileName = "PowerShell.exe";
                getFile.StartInfo.Arguments = $"{pathPowershellScripts}{powershellScriptName} {auftragsnummer}";
                getFile.StartInfo.CreateNoWindow = false;
                getFile.StartInfo.RedirectStandardOutput = true;
                getFile.Start();
                getFile.WaitForExit();
                string downloadResult = await getFile.StandardOutput.ReadToEndAsync();
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

                Console.WriteLine("GetVaultFile finished ...");
                await Task.CompletedTask;
                return getFile.ExitCode;
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