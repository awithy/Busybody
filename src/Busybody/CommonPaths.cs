using System;
using System.IO;
using System.Linq;

namespace Busybody
{
    public static class CommonPaths
    {
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
    }
}
