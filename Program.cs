using System;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace PowerShellStart
{
    class Program
    {
        static int Main(string[] args)
        {
            String starttyp;
            starttyp = null;

            String Auftragsnummer;
            Auftragsnummer = null;

            String PSPfad;
            PSPfad = @"C:\Work\Administration\PowerShellScripts\";

            //Befehlszeilenargumente auslesen 
            string[] CommandLineArgs = Environment.GetCommandLineArgs();

            //CommandLineArgs[0]: Immer der Dateipfad 
            if (CommandLineArgs.Length > 1)
            {
                // Console.WriteLine(CommandLineArgs[1].ToString());
                // Console.WriteLine(CommandLineArgs[2].ToString());
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

            //Warte 
            //Console.ReadLine();

            int exitCode=0;

            switch (starttyp)
            {
                case "get":
                    exitCode=GetFile(PSPfad, Auftragsnummer);
                    break;
                case "set":
                    exitCode=SetFile(PSPfad, Auftragsnummer);
                    break;
                default:
                    exitCode=GetFile(PSPfad, Auftragsnummer);
                    break;
            }

            System.Threading.Thread.Sleep(1000);
            return exitCode;

        }

        static int GetFile(string PSPfad,String Auftragsnummer)
        {
            String PsName;
            PsName = "GetVaultFile.ps1";

            //Process.Start("PowerShell.exe", $"{PSPfad}{PsName} {Auftragsnummer}").WaitForExit();

            try
            {
                Process getFile = new Process();

                getFile.StartInfo.UseShellExecute = false;
                getFile.StartInfo.FileName = "PowerShell.exe";
                getFile.StartInfo.Arguments = $"{PSPfad}{PsName} {Auftragsnummer}";
                getFile.StartInfo.CreateNoWindow = false;
                getFile.Start();
                getFile.WaitForExit();

                Console.WriteLine("GetVaultFile finished ......");

                return getFile.ExitCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 4;
            }
            
        }
        static int SetFile(string PSPfad, String Auftragsnummer)
        {
            String PsName;
            PsName = "SetVaultFile.ps1";

            //Process.Start("PowerShell.exe", $"{PSPfad}{PsName} {Auftragsnummer}").WaitForExit();

            try
            {
                Process setFile = new Process();

                setFile.StartInfo.UseShellExecute = false;
                setFile.StartInfo.FileName = "PowerShell.exe";
                setFile.StartInfo.Arguments = $"{PSPfad}{PsName} {Auftragsnummer}";
                setFile.StartInfo.CreateNoWindow = false;
                setFile.Start();
                setFile.WaitForExit();

                Console.WriteLine("SetVaultFile finished ......");

                return setFile.ExitCode;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 4;
            }
        }
    }

}