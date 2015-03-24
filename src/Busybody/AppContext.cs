using Busybody.Config;

namespace Busybody
{
    public class AppContext : IAppContext
    {
        public static IAppContext Instance;

        public BusybodyConfig Config { get; set; }
        public IEventLogger EventLogger { get; private set; }
        public ITestFactory TestFactory { get; private set; }
        public IThreading Threading { get; private set; }
        public IEventBus EventBus { get; private set; }
        public IEmailAlertingInterface EmailAlertingInterface { get; set; }

        public AppContext()
        {
            EventLogger = new EventLogger();
            TestFactory = new TestFactory();
            Threading = new Threading();
            EventBus = new EventBus();
            EmailAlertingInterface = new EmailAlertingInterface();
        }
    }

    public interface IAppContext
    {
        IEventLogger EventLogger { get; }
        ITestFactory TestFactory { get; }
        IThreading Threading { get; }
        IEventBus EventBus { get; }
        IEmailAlertingInterface EmailAlertingInterface { get; }
        BusybodyConfig Config { get; set; }
    }
}