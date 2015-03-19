using Busybody;
using Busybody.Config;

namespace BusybodyTests.Fakes
{
    public class FakeAppContext : IAppContext
    {
        public IEventLogger EventLogger { get; private set; }
        public ITestFactory TestFactory { get; private set; }
        public IThreading Threading { get; private set; }
        public IEventBus EventBus { get; private set; }
        public BusybodyConfig Config { get; set; }
        public FakeTestFactory FakeTestFactory { get { return (FakeTestFactory)TestFactory;  } }
        public FakeThreading FakeThreading { get { return (FakeThreading) Threading; } }

        public FakeAppContext()
        {
            EventLogger = new FakeEventLogger();
            TestFactory = new FakeTestFactory();
            Threading = new FakeThreading();
            EventBus = new EventBus();
        }
    }
}