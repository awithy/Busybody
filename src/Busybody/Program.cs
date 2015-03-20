using System;
using System.IO;
using System.Linq;
using System.Threading;
using Busybody.Config;

namespace Busybody
{
    class Program
    {
        static int Main(string[] args)
        {
            //TODO: Handle unhandled exceptions

            if (args.Contains("-d") || args.Contains("--debug"))
                LogSetup.EnableConsoleDebug();

            var busybodyTempPath = CommonPaths.BusybodyTemp();
            Directory.CreateDirectory(busybodyTempPath);
            _SetupLogging();

            var log = new Logger(typeof (Program));
            try
            {
                log.Info("Starting Busybody v0.1");

                AppContext.Instance = new AppContext();
                var configFilePath = CommonPaths.CurrentConfigFilePath();
                AppContext.Instance.Config = new ConfigFileReader().ReadFromFile(configFilePath);
                var busybodyDaemon = new BusybodyDaemon();
                busybodyDaemon.Start();

                log.Info("Startup complete");

                while(true)
                {
                    var key = Console.ReadKey();
                    if (key.Key == ConsoleKey.Q)
                    { 
                        busybodyDaemon.Stop();
                        return 0;
                    }

                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                log.Error("Unexpected " + ex.GetType().Name + " occurred.  Aborting.  " + Environment.NewLine + ex);
                return -1;
            }
        }

        static void _SetupLogging()
        {
            var logsDirectory = CommonPaths.LogsPath();
            Directory.CreateDirectory(logsDirectory);
            var logFilePath = CommonPaths.LogFilePath("Debug");
            LogSetup.Setup(logFilePath);
        }
    }
}