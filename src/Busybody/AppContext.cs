using System;
using Busybody.Config;
using Busybody.Utility;
using BusybodyShared;

namespace Busybody
{
    public interface IAppContext
    {
        ITestFactory TestFactory { get; }
        IEventBus EventBus { get; }
        IEmailAlertingInterface EmailAlertingInterface { get; }
        ISystemStatusWriter SystemStatusWriter { get; }
        IAgentChannel AzureAgentChannel { get; }
        IAgentChannel FileAgentChannel { get; }
        BusybodyConfig Config { get; set; }
        HostRepository HostRepository { get; }
        SystemStatus SystemStatus { get; }
        EventLogRepository EventLogRepository { get; set; }
        DateTime StartTime { get; }
    }

    public class AppContext : IAppContext
    {
        public DateTime StartTime { get; private set; }
        public static IAppContext Instance;
        public IAgentChannel FileAgentChannel { get; private set; }
        public BusybodyConfig Config { get; set; }
        public ISystemStatusWriter SystemStatusWriter { get; private set; }
        public IAgentChannel AzureAgentChannel { get; private set; }
        public IEventLogger EventLogger { get; private set; }
        public ITestFactory TestFactory { get; private set; }
        public IEventBus EventBus { get; private set; }
        public IEmailAlertingInterface EmailAlertingInterface { get; set; }
        public SystemStatus SystemStatus { get; set; }
        public HostRepository HostRepository { get; set; }
        public EventLogRepository EventLogRepository { get; set; }

        public AppContext(BusybodyConfig config)
        {
            Config = config;
            StartTime = DateTime.UtcNow;
            EventLogger = new EventLogger();
            TestFactory = new TestFactory();
            EventBus = new EventBus();
            EmailAlertingInterface = new EmailAlertingInterface();
            HostRepository = new HostRepository();
            SystemStatus = new SystemStatus();
            SystemStatusWriter = new SystemStatusWriter();
            EventLogRepository = new EventLogRepository();

            if (Config.AzureAgentChannelConfig != null)
                AzureAgentChannel = new AzureAgentChannel(Config.AzureAgentChannelConfig);
            else
                AzureAgentChannel = new NullAgentChannel();

            if (Config.FileAgentChannelConfig != null)
                FileAgentChannel = new FileAgentChannel(Config.FileAgentChannelConfig);
            else
                FileAgentChannel = new NullAgentChannel();
        }
    }
}