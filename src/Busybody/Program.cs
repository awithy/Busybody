using System;
using System.IO;

namespace Busybody
{
    class Program
    {
        static int Main(string[] args)
        {
            var busybodyTempPath = CommonPaths.BusybodyTemp();
            Directory.CreateDirectory(busybodyTempPath);
            _SetupLogging();

            var log = new Logger(typeof (Program));
            try
            {
                log.Debug("Starting");

                var busybodyDaemon = new BusybodyDaemon();
                busybodyDaemon.Start();

                log.Info("Startup complete");
                return 0;
            }
            catch (Exception ex)
            {
                log.Error("Unexpected " + ex.GetType().Name + " occurred.  Aborting.");
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