﻿using System;
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
            return Path.Combine(BusybodyData(), "Events.log");
        }

        public static string WebRoot()
        {
            return AppContext.Instance.Config.WebRoot;
        }

        public static string WebContentPath()
        {
            return WebRoot();
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

    public class ConfigurationFileNotFoundException : Exception
    {
    }
}
