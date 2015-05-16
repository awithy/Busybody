﻿using System;
using System.IO;
using System.Reflection;

namespace BusybodyAgent
{
    public static class CommonPaths
    {
        public static string CurrentDirectory()
        {
            return Environment.CurrentDirectory;
        }

        public static string CurrentConfigFilePath()
        {
            var currentConfigFilePath = Path.Combine(CurrentDirectory(), "BusybodyAgent.cfg");
            if(File.Exists(currentConfigFilePath))
                return currentConfigFilePath;
            currentConfigFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "busybodyAgent.cfg");
            if(File.Exists(currentConfigFilePath))
                return currentConfigFilePath;
            throw new ConfigurationFileNotFoundException();
        }

        public static string EventLogFilePath()
        {
            return Path.Combine(BusybodyData(), "Events.log");
        }

        public static string RandomName()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + Guid.NewGuid().ToString("N").Substring(0, 5);
        }

        public static string BusybodyData()
        {
            if (AppContext.Instance == null || AppContext.Instance.Config == null)
                return null;
            return AppContext.Instance.Config.DataDirectory;
        }

        public static string LogsPath()
        {
            return Path.Combine(BusybodyData(), "Logs");
        }
    }

    public class DataDirectoryNullException : Exception
    {
    }

    public class ConfigurationFileNotFoundException : Exception
    {
    }
}