using System;
using Busybody;
using Busybody.Config;
using Busybody.Utility;
using BusybodyShared;

namespace BusybodyTests.Fakes
{
    public class FakeAppContext : IAppContext
    {
        public IEventLogger EventLogger { get; private set; }
        public ITestFactory TestFactory { get; private set; }
        public IEventBus EventBus { get; private set; }
        public IEmailAlertingInterface EmailAlertingInterface { get; private set; }
        public ISystemStatusWriter SystemStatusWriter { get; private set; }
        public IAgentChannel AzureAgentChannel { get; private set; }
        public IAgentChannel FileAgentChannel { get; private set; }
        public SystemStatus SystemStatus { get; private set; }
        public BusybodyConfig Config { get; set; }
        public HostRepository HostRepository { get; set; }
        public EventLogRepository EventLogRepository { get; set; }
        public DateTime StartTime { get; set; }
        public FakeTestFactory FakeTestFactory { get { return (FakeTestFactory)TestFactory;  } }
        public FakeEmailAlertingInterface FakeEmailAlertingInterface { get { return (FakeEmailAlertingInterface) EmailAlertingInterface; }}
        public FakeSystemStatusWriter FakeSystemStatusWriter { get { return (FakeSystemStatusWriter) SystemStatusWriter; } }

        public FakeAppContext()
        {
            StartTime = DateTime.UtcNow;
            EventLogger = new FakeEventLogger();
            TestFactory = new FakeTestFactory();
            EventBus = new EventBus();
            EmailAlertingInterface = new FakeEmailAlertingInterface();
            HostRepository = new HostRepository();
            SystemStatus = new SystemStatus();
            SystemStatusWriter = new FakeSystemStatusWriter();
            EventLogRepository = new EventLogRepository();
            AzureAgentChannel = new NullAgentChannel();
            FileAgentChannel = new NullAgentChannel();
        }
    }
}