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
        public void Delete()
        {
            File.Delete(WorkDir + "\\" + LogFile);
        }

        public void Rotate()
        {
            FileInfo file = new FileInfo(WorkDir + "\\" + LogFile);
            if (file.Exists)
            {
                if (file.Length >= 1048576)
                {
                    File.Move(WorkDir + "\\" + LogFile,
                        WorkDir + "\\" + file.Name + DateTime.Now.ToString("_yyyy-MM-dd_HHmmss") + file.Extension);
                }
            }
        }
    }
}