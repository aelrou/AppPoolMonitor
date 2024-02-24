using System;
using System.IO;

namespace AppPoolMonitor.Method
{
    internal class MainArgs
    {
        internal readonly string WorkDir;

        public MainArgs(string[] args)
        {
            if (args.Length == 1)
            {
                if (Directory.Exists(args[0]))
                {
                    WorkDir = args[0];
                }
                else
                {
                    Console.WriteLine(String.Concat("Cannot access \"", args[0], "\""));
                    Console.ReadLine();
                    Environment.Exit(1);
                }
            }
            else
            {
                Console.WriteLine("Working directory argument required.");
                Console.WriteLine();
                Console.WriteLine("Usage: AppPoolMonitor.exe \"C:\\Users\\Public\\AppPoolMonitor\"");
                Console.ReadLine();
                Environment.Exit(1);
            }
        }
    }
}
