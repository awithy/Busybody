using System.Collections.Generic;
using BusybodyShared;

namespace BusybodyDekstopNotificationApp
{
    public class BusybodyNotificationConfig
    {
        public int PollingInterval { get; set; }
        public AzureStorageConfig AzureStorageConfig { get; set; }
        public IEnumerable<SystemConfig> Systems { get; set; }

        public BusybodyNotificationConfig()
        {
            Systems = new SystemConfig[0];
        }
    }

    public class SystemConfig
    {
        public string Name { get; set; }
        public string SystemId { get; set; }
        public string Url { get; set; }
    }
}
