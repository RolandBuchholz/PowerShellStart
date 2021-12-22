using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PowerShellStart
{
    class Program
    {
        static int Main(string[] args)
        {
            string starttyp;
            starttyp = null;

            string Auftragsnummer;
            Auftragsnummer = null;

            string PSPfad;
            PSPfad = @"C:\Work\Administration\PowerShellScripts\";

            //Befehlszeilenargumente auslesen 
            string[] CommandLineArgs = Environment.GetCommandLineArgs();

            //CommandLineArgs[0]: Immer der Dateipfad 
            if (CommandLineArgs.Length > 1)
            {
                if (CommandLineArgs[1] == "get" | CommandLineArgs[1] == "set")
                {
                    starttyp = CommandLineArgs[1];
                    Auftragsnummer = CommandLineArgs[2];
                }
                else
                {
                    Auftragsnummer = CommandLineArgs[1];
                }
            }

            Task<int> result;
            int exitCode;

            var watch = Stopwatch.StartNew();

            switch (starttyp)
            {
                case "get":
                    result = GetFileAsync(PSPfad, Auftragsnummer);
                    exitCode = result.Result;
                    break;
                case "set":
                    result = SetFileAsync(PSPfad, Auftragsnummer);
                    exitCode = result.Result;
                    break;
                default:
                    result = GetFileAsync(PSPfad, Auftragsnummer);
                    exitCode = result.Result;
                    break;
            }
            var stopTimeMs = watch.ElapsedMilliseconds;
            
            Console.WriteLine($"Downloadtime: {stopTimeMs} ms");
            return exitCode;
        }

        static async Task<int> GetFileAsync(string PSPfad, string Auftragsnummer)
        {
            string PsName;
            PsName = "GetVaultFile.ps1";

            try
            {
                using Process getFile = new();
                getFile.StartInfo.UseShellExecute = false;
                getFile.StartInfo.FileName = "PowerShell.exe";
                getFile.StartInfo.Arguments = $"{PSPfad}{PsName} {Auftragsnummer}";
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
        static async Task<int> SetFileAsync(string PSPfad, string Auftragsnummer)
        {
            string PsName;
            PsName = "SetVaultFile.ps1";

            try
            {
                using Process setFile = new();
                setFile.StartInfo.UseShellExecute = false;
                setFile.StartInfo.FileName = "PowerShell.exe";
                setFile.StartInfo.Arguments = $"{PSPfad}{PsName} {Auftragsnummer}";
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