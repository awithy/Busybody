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

            FakePingTest = TestAppContext.FakeTestFactory.GetTest<FakePingTest>("Ping");

            AppContext.Instance = TestAppContext;
            AppContext.Instance.EventBus.Subscribe(new EventSubscription
            {
                Name = "Test Subscription",
                EventStreamName = "All",
                Recipient = eventNotification => EventHandler.Handle(eventNotification),
            });
        }
    }
}