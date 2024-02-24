using System;
using System.IO;

namespace AppPoolMonitor.Method
{
    internal class Log
    {
        internal string WorkDir;
        internal string LogFile;

        public Log()
        {
            WorkDir = null;
            LogFile = null;
        }
        public void Write(string logText)
        {
            Console.WriteLine(logText);
            File.AppendAllText(WorkDir + "\\" + LogFile, Environment.NewLine + logText);
        }
        public void Clear()
        {
            File.Delete(WorkDir + "\\" + LogFile);
        }

    }
}