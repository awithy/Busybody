using System;
using System.IO;
using System.Reflection;

namespace Busybody
{
    public static class CommonPaths
    {
        public static string CurrentDirectory()
        {
            return Environment.CurrentDirectory;
        }

        public static string CurrentConfigFilePath()
        {
            var currentConfigFilePath = Path.Combine(CurrentDirectory(), "Busybody.cfg");
            if(File.Exists(currentConfigFilePath))
                return currentConfigFilePath;
            currentConfigFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "busybody.cfg");
            if(File.Exists(currentConfigFilePath))
                return currentConfigFilePath;
            throw new ConfigurationFileNotFoundException();
        }

        public static string EventLogFilePath()
        {
            return Path.Combine(BusybodyTemp(), "Events.log");
        }

        public static string RandomName()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Guid.NewGuid().ToString("N").Substring(0, 5);
        }

        public static string BusybodyTemp()
        {
            return Path.Combine(Path.GetTempPath(), "Busybody");
        }

        public static string LogsPath()
        {
            return Path.Combine(BusybodyTemp(), "Logs");
        }

        public static string LogFilePath(string logLevel)
        {
            return Path.Combine(LogsPath(), logLevel + ".log");
        }
    }

    public class ConfigurationFileNotFoundException : Exception
    {
    }
}
