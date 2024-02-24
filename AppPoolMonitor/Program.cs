using System;
using System.Data;
using System.Linq;
using System.Text;
using AppPoolMonitor.Method;
using Microsoft.Web.Administration;

namespace AppPoolMonitor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            MainArgs sessionArgs = new MainArgs(args);

            string consoleLogFile = "console.log";
            Log logConsole = new Log()
            {
                WorkDir = sessionArgs.WorkDir,
                LogFile = consoleLogFile
            };
            logConsole.Clear();

            MainConfig session = new MainConfig(sessionArgs.WorkDir, logConsole);

            string requestCountLogFile = "requestCount.log";
            Log logCount = new Log()
            {
                WorkDir = sessionArgs.WorkDir,
                LogFile = requestCountLogFile
            };

            string requestsLogFile = "requestList.log";
            Log logRequests = new Log()
            {
                WorkDir = sessionArgs.WorkDir,
                LogFile = requestsLogFile
            };

            string deadlockLogFile = "deadlockList.log";
            Log logDeadlocks = new Log()
            {
                WorkDir = sessionArgs.WorkDir,
                LogFile = deadlockLogFile
            };

            ServerManager manager = new ServerManager();
            var requests = manager.ApplicationPools
                .Where(pool => pool.Name == session.Config["AppPoolName"])
                .SelectMany(pool => pool.WorkerProcesses)
                .SelectMany(wp => wp.GetRequests(0));
                //.SelectMany(wp => wp.GetRequests(10));

            DateTime start = DateTime.Now;
            int requestCount = requests.Count();
            if (requestCount >= int.Parse(session.Config["RequestCountThreshold"]))
            {
                logCount.Write(start.ToString("yyyy-MM-dd HH:mm:ss") + " Request count: " + requestCount + " >>> WARNING <<<");
                logRequests.Write(start.ToString("yyyy-MM-dd HH:mm:ss"));
                foreach (var request in requests)
                {
                    logRequests.Write(request.ClientIPAddr
                                      + " " + request.Verb
                                      + " --- https://" + request.HostName + request.Url
                                      + " --- TimeElapsed:" + request.TimeElapsed
                                      + " TimeInState:" + request.TimeInState
                                      + " TimeInModule:" + request.TimeInModule
                    );
                }
                logRequests.Write("--------------------------------");

                Deadlocks query = new Deadlocks()
                {
                    DataSource = session.Config["DBDataSource"],
                    InitialCatalog = session.Config["DBInitialCatalog"],
                    UserId = session.Config["DBUserId"],
                    Password = session.Config["DBPassword"]
                };

                DataTable deadlocks = new DataTable();
                deadlocks = query.Execute();

                foreach (DataRow dataRow in deadlocks.Rows)
                {
                    foreach (var item in dataRow.ItemArray)
                    {
                        logDeadlocks.Write(item.ToString());
                    }
                }
            }
            else
            {
                logCount.Write(start.ToString("yyyy-MM-dd HH:mm:ss") + " Request count: " + requestCount);
            }

            logConsole.Write("End.");
        }
    }
}
