using Busybody.Config;
using Busybody.Utility;

namespace Busybody
{
    public interface IAppContext
    {
        ITestFactory TestFactory { get; }
        IEventBus EventBus { get; }
        IEmailAlertingInterface EmailAlertingInterface { get; }
        ISystemStatusWriter SystemStatusWriter { get; }
        BusybodyConfig Config { get; set; }
        HostRepository HostRepository { get; }
        SystemStatus SystemStatus { get; }
        EventLogRepository EventLogRepository { get; set; }
    }

    public class AppContext : IAppContext
    {
        public static IAppContext Instance;

        public BusybodyConfig Config { get; set; }
        public ISystemStatusWriter SystemStatusWriter { get; private set; }
        public IEventLogger EventLogger { get; private set; }
        public ITestFactory TestFactory { get; private set; }
        public IEventBus EventBus { get; private set; }
        public IEmailAlertingInterface EmailAlertingInterface { get; set; }
        public HostRepository HostRepository { get; set; }
        public SystemStatus SystemStatus { get; set; }
        public EventLogRepository EventLogRepository { get; set; }

        public AppContext()
        {
            EventLogger = new EventLogger();
            TestFactory = new TestFactory();
            EventBus = new EventBus();
            EmailAlertingInterface = new EmailAlertingInterface();
            HostRepository = new HostRepository();
            SystemStatus = new SystemStatus();
            SystemStatusWriter = new SystemStatusWriter();
            EventLogRepository = new EventLogRepository();
        }
    }
}