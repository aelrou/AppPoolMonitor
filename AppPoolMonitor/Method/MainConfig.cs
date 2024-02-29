using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace AppPoolMonitor.Method
{
    internal class MainConfig
    {
        internal readonly Dictionary<string, string[]> Config;
        public MainConfig(string workDir, Log logConsole)
        {
            string configFile = "config.json";
            string configFilePath = workDir + "\\" + configFile;
            if (File.Exists(configFilePath))
            {
                logConsole.Write("Read config " + configFilePath);
                string configText = File.ReadAllText(configFilePath);
                Config = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(configText);
            }
            else
            {
                logConsole.Write("Config not found.");
                Config = new Dictionary<string, string[]>();

                string[] AppPoolName = { "Default Website" };
                Config.Add("AppPoolName", AppPoolName);

                string[] RequestCountThreshold = { "20" };
                Config.Add("RequestCountThreshold", RequestCountThreshold);

                string[] DBDataSource = { "server\\instance" };
                Config.Add("DBDataSource", DBDataSource);

                string[] DBInitialCatalog = { "database" };
                Config.Add("DBInitialCatalog", DBInitialCatalog);

                string[] AppPooDBUserIdlName = { "username" };
                Config.Add("DBUserId", AppPooDBUserIdlName);

                string[] DBPassword = { "password" };
                Config.Add("DBPassword", DBPassword);

                string[] UrlIgnoreList = { "firstignore.url", "secondignore.url" };
                Config.Add("UrlIgnoreList", UrlIgnoreList);

                logConsole.Write("Write config " + configFilePath);
                string configText = JsonConvert.SerializeObject(Config, Formatting.Indented);
                File.WriteAllText(configFilePath, configText);
            }

            try
            {
                logConsole.Write("AppPoolName");
                if (Config["AppPoolName"].Length != 1)
                {
                    throw new InvalidOperationException("Number of values must equal 1");
                }
                Config["AppPoolName"].GetValue(0).ToString();
                
                logConsole.Write("RequestCountThreshold");
                if (Config["RequestCountThreshold"].Length != 1)
                {
                    throw new InvalidOperationException("Number of values must equal 1");
                }
                Config["RequestCountThreshold"].GetValue(0).ToString();
                
                logConsole.Write("DBDataSource");
                if (Config["DBDataSource"].Length != 1)
                {
                    throw new InvalidOperationException("Number of values must equal 1");
                }
                Config["DBDataSource"].GetValue(0).ToString();
                
                logConsole.Write("DBInitialCatalog");
                if (Config["DBInitialCatalog"].Length != 1)
                {
                    throw new InvalidOperationException("Number of values must equal 1");
                }
                Config["DBInitialCatalog"].GetValue(0).ToString();

                logConsole.Write("DBUserId");
                if (Config["DBUserId"].Length != 1)
                {
                    throw new InvalidOperationException("Number of values must equal 1");
                }
                Config["DBUserId"].GetValue(0).ToString();

                logConsole.Write("DBPassword");
                if (Config["DBPassword"].Length != 1)
                {
                    throw new InvalidOperationException("Number of values must equal 1");
                }
                Config["DBPassword"].GetValue(0).ToString();

                logConsole.Write("UrlIgnoreList");
                if (Config["UrlIgnoreList"].Length == 0)
                {
                    throw new InvalidOperationException("Number of values must be greater than 0");
                }
                foreach (var url in Config["UrlIgnoreList"])
                {
                    url.ToString();
                }
            }
            catch (Exception ex)
            {
                logConsole.Write(ex.Message);
                Environment.Exit(1);
            }
        }
    }
}