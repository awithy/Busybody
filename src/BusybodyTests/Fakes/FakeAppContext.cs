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
        public BusybodyConfig Config { get; set; }
        public FakeTestFactory FakeTestFactory { get { return (FakeTestFactory)TestFactory;  } }
        public FakeEmailAlertingInterface FakeEmailAlertingInterface { get { return (FakeEmailAlertingInterface) EmailAlertingInterface; }}

        public FakeAppContext()
        {
            EventLogger = new FakeEventLogger();
            TestFactory = new FakeTestFactory();
            EventBus = new EventBus();
            EmailAlertingInterface = new FakeEmailAlertingInterface();
        }
    }
}