using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace AppPoolMonitor.Method
{
    internal class MainConfig
    {
        internal readonly Dictionary<string, string> Config;
        public MainConfig(string workDir, Log logConsole)
        {
            string configFile = "config.json";
            string configFilePath = workDir + "\\" + configFile;
            if (File.Exists(configFilePath))
            {
                logConsole.Write("Read config " + configFilePath);
                string configText = File.ReadAllText(configFilePath);
                Config = JsonConvert.DeserializeObject<Dictionary<string, string>>(configText);
            }
            else
            {
                logConsole.Write("Config not found.");
                Config = new Dictionary<string, string>();
                Config.Add("AppPoolName", "Default Website");
                Config.Add("RequestCountThreshold", "20");
                Config.Add("DBDataSource", "server\\instance");
                Config.Add("DBInitialCatalog", "database");
                Config.Add("DBUserId", "username");
                Config.Add("DBPassword", "password");

                logConsole.Write("Write config " + configFilePath);
                string configText = JsonConvert.SerializeObject(Config, Formatting.Indented);
                File.WriteAllText(configFilePath, configText);
            }

            try
            {
                logConsole.Write("AppPoolName");
                Config["AppPoolName"].ToString();
                
                logConsole.Write("RequestCountThreshold");
                Config["RequestCountThreshold"].ToString();
                
                logConsole.Write("DBDataSource");
                Config["DBDataSource"].ToString();
                
                logConsole.Write("DBInitialCatalog");
                Config["DBInitialCatalog"].ToString();

                logConsole.Write("DBUserId");
                Config["DBUserId"].ToString();

                logConsole.Write("DBPassword");
                Config["DBPassword"].ToString();
            }
            catch (Exception ex)
            {
                logConsole.Write(ex.Message);
                Environment.Exit(1);
            }
        }
    }
}