using Busybody;
using Busybody.Config;

namespace BusybodyTests.Fakes
{
    public class FakeAppContext : IAppContext
    {
        public IEventLogger EventLogger { get; private set; }
        public ITestFactory TestFactory { get; private set; }
        public IEventBus EventBus { get; private set; }
        public IEmailAlertingInterface EmailAlertingInterface { get; private set; }
        public ISystemStatusWriter SystemStatusWriter { get; private set; }
        public BusybodyConfig Config { get; set; }
        public HostRepository HostRepository { get; set; }
        public SystemMonitorData SystemMonitorData { get; private set; }
        public EventLogRepository EventLogRepository { get; set; }
        public FakeTestFactory FakeTestFactory { get { return (FakeTestFactory)TestFactory;  } }
        public FakeEmailAlertingInterface FakeEmailAlertingInterface { get { return (FakeEmailAlertingInterface) EmailAlertingInterface; }}
        public FakeSystemStatusWriter FakeSystemStatusWriter { get { return (FakeSystemStatusWriter) SystemStatusWriter; } }

        public FakeAppContext()
        {
            EventLogger = new FakeEventLogger();
            TestFactory = new FakeTestFactory();
            EventBus = new EventBus();
            EmailAlertingInterface = new FakeEmailAlertingInterface();
            HostRepository = new HostRepository();
            SystemMonitorData = new SystemMonitorData();
            SystemStatusWriter = new FakeSystemStatusWriter();
            EventLogRepository = new EventLogRepository();
        }
    }
}