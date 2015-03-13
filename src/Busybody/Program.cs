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
            _SetupLogging(busybodyTempPath);

            var log = new Logger(typeof (Program));
            try
            {
                log.Debug("Starting");

                var eventLogger = new EventLogger();
                eventLogger.Publish("Started successfully");

                log.Info("Started successfully");
                return 0;
            }
            catch (Exception ex)
            {
                log.Error("Unexpected " + ex.GetType().Name + " occurred.  Aborting.");
                return -1;
            }
        }

        static void _SetupLogging(string busybodyTempPath)
        {
            var logsDirectory = Path.Combine(busybodyTempPath, "Logs");
            Directory.CreateDirectory(logsDirectory);
            var logFilePath = Path.Combine(busybodyTempPath, "Logs", "Debug.log");
            LogSetup.Setup(logFilePath);
        }
    }
}