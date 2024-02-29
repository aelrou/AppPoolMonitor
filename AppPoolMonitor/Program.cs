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

            Log logConsole = new Log()
            {
                WorkDir = sessionArgs.WorkDir,
                LogFile = "console.log"
            };
            logConsole.Delete();

            DateTime start = DateTime.Now;
            logConsole.Write(start.ToString("yyyy-MM-dd HH:mm:ss"));

            MainConfig session = new MainConfig(sessionArgs.WorkDir, logConsole);

            Log logCount = new Log()
            {
                WorkDir = sessionArgs.WorkDir,
                LogFile = "requestCount.log"
            };
            logCount.Rotate();

            Log logRequests = new Log()
            {
                WorkDir = sessionArgs.WorkDir,
                LogFile = "requestList.log"
            };
            logRequests.Rotate();

            Log logDeadlocks = new Log()
            {
                WorkDir = sessionArgs.WorkDir,
                LogFile = "deadlockList.log"
            };
            logDeadlocks.Rotate();

            ServerManager manager = new ServerManager();
            var requests = manager.ApplicationPools
                .Where(pool => pool.Name == session.Config["AppPoolName"].GetValue(0).ToString())
                .SelectMany(pool => pool.WorkerProcesses)
                .SelectMany(wp => wp.GetRequests(0));
                //.SelectMany(wp => wp.GetRequests(10));

            int requestCount = requests.Count();
            if (requestCount >= int.Parse(session.Config["RequestCountThreshold"].GetValue(0).ToString()))
            {
                logCount.Write(start.ToString("yyyy-MM-dd HH:mm:ss") + " Request count: " + requestCount + " >>> WARNING <<<");
                logRequests.Write(start.ToString("yyyy-MM-dd HH:mm:ss"));
                foreach (var request in requests)
                {
                    bool ignore = false;
                    foreach (var url in session.Config["UrlIgnoreList"])
                    {
                        if (request.Url.Contains(url))
                        {
                            ignore = true;
                            break;
                        }
                    }
                    if (ignore == false)
                    {
                        logRequests.Write(request.ClientIPAddr
                                          + " " + request.Verb
                                          + " --- https://" + request.HostName + request.Url
                                          + " --- TimeElapsed:" + request.TimeElapsed
                                          + " TimeInState:" + request.TimeInState
                                          + " TimeInModule:" + request.TimeInModule
                        );

                    }
                }
                logRequests.Write("--------------------------------");

                Deadlocks query = new Deadlocks()
                {
                    DataSource = session.Config["DBDataSource"].GetValue(0).ToString(),
                    InitialCatalog = session.Config["DBInitialCatalog"].GetValue(0).ToString(),
                    UserId = session.Config["DBUserId"].GetValue(0).ToString(),
                    Password = session.Config["DBPassword"].GetValue(0).ToString()
                };

                DataTable deadlocks = new DataTable();
                deadlocks = query.Execute();

                logDeadlocks.Write(start.ToString("yyyy-MM-dd HH:mm:ss"));
                foreach (DataRow dataRow in deadlocks.Rows)
                {
                    foreach (var item in dataRow.ItemArray)
                    {
                        logDeadlocks.Write(item.ToString());
                    }
                }
                logDeadlocks.Write("--------------------------------");
            }
            else
            {
                logCount.Write(start.ToString("yyyy-MM-dd HH:mm:ss") + " Request count: " + requestCount);
            }

            logConsole.Write("End.");
        }
    }
}
