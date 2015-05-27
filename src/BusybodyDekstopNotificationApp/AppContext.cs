using BusybodyShared;

namespace BusybodyDekstopNotificationApp
{
    public class AppContext
    {
        public static AppContext Instance { get; set; }
        public BusybodyNotificationConfig Config { get; set; }
        public AzureStatusRepository AzureStatusRepository { get; set; }
        public SystemStatusProvider SystemStatusProvider { get; set; }

        public AppContext(BusybodyNotificationConfig config)
        {
            Config = config;
            SystemStatusProvider = new SystemStatusProvider();
            AzureStatusRepository = new AzureStatusRepository();
        }
    }
}
