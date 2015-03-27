using Busybody;
using BusybodyTests.Fakes;

namespace BusybodyTests
{
    public class TestContext
    {
        public TestEventHandler EventHandler { get; set; }
        public FakePingTest FakePingTest { get; set; }
        public FakeAppContext TestAppContext { get; set; }
        public BusybodyDaemon Daemon { get; set; }

        public TestContext()
        {
            EventHandler = new TestEventHandler();
            TestAppContext = new FakeAppContext();
            Daemon = new BusybodyDaemon();

            TestAppContext = new FakeAppContextBuilder()
                .WithBasicConfiguration()
                .Build();

            TestAppContext.EventBus.RegisterHandler("All", typeof(TestEventHandler));

            FakePingTest = TestAppContext.FakeTestFactory.GetTest<FakePingTest>("Ping");

            TestEventHandler.ReceivedHostStateEvents.Clear();

            AppContext.Instance = TestAppContext;
        }
    }
}