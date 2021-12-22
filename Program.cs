using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PowerShellStart
{
    class Program
    {
        static int Main(string[] args)
        {
            string starttyp = string.Empty;

            string auftragsnummer = string.Empty;

            string pathPowershellScripts;
            pathPowershellScripts = @"C:\Work\Administration\PowerShellScripts\";

            //Befehlszeilenargumente auslesen 
            string[] commandLineArgs = Environment.GetCommandLineArgs();

            //CommandLineArgs[0]: Immer der Dateipfad 
            if (commandLineArgs.Length > 1)
            {
                if (commandLineArgs[1] == "get" | commandLineArgs[1] == "set")
                {
                    starttyp = commandLineArgs[1];
                    auftragsnummer = commandLineArgs[2];
                }
                else
                {
                    auftragsnummer = commandLineArgs[1];
                }
            }

            Task<int> result;
            int exitCode;

            var watch = Stopwatch.StartNew();

            switch (starttyp)
            {
                case "get":
                    result = GetFileAsync(pathPowershellScripts, auftragsnummer);
                    exitCode = result.Result;
                    break;
                case "set":
                    result = SetFileAsync(pathPowershellScripts, auftragsnummer);
                    exitCode = result.Result;
                    break;
                default:
                    result = GetFileAsync(pathPowershellScripts, auftragsnummer);
                    exitCode = result.Result;
                    break;
            }
            var stopTimeMs = watch.ElapsedMilliseconds;
            
            Console.WriteLine($"Downloadtime: {stopTimeMs} ms");
            return exitCode;
        }

        static async Task<int> GetFileAsync(string pathPowershellScripts, string auftragsnummer)
        {
            string powershellScriptName;
            powershellScriptName = "GetVaultFile.ps1";

            try
            {
                using Process getFile = new();
                getFile.StartInfo.UseShellExecute = false;
                getFile.StartInfo.FileName = "PowerShell.exe";
                getFile.StartInfo.Arguments = $"{pathPowershellScripts}{powershellScriptName} {auftragsnummer}";
                getFile.StartInfo.CreateNoWindow = false;
                getFile.Start();
                getFile.WaitForExit();

                Console.WriteLine("GetVaultFile finished ......");
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
            string powershellScriptName;
            powershellScriptName = "SetVaultFile.ps1";

            try
            {
                using Process setFile = new();
                setFile.StartInfo.UseShellExecute = false;
                setFile.StartInfo.FileName = "PowerShell.exe";
                setFile.StartInfo.Arguments = $"{pathPowershellScripts}{powershellScriptName} {auftragsnummer}";
                setFile.StartInfo.CreateNoWindow = false;
                setFile.Start();
                setFile.WaitForExit();

                Console.WriteLine("SetVaultFile finished ......");
                await Task.CompletedTask;
                return setFile.ExitCode;
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