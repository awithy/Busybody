using Busybody;
using Busybody.Config;
using BusybodyTests.Fakes;

namespace BusybodyTests.Helpers
{
    public class BusybodyTestContext
    {
        public TestEventHandler EventHandler { get; set; }
        public FakePingTest FakePingTest { get; set; }
        public FakeAppContext TestAppContext { get; set; }
        public BusybodyDaemon Daemon { get; set; }

        public static BusybodyTestContext Setup()
        {
            var fakeAppContext = new FakeAppContextBuilder()
                .WithBasicConfiguration()
                .Build();
            return Setup(fakeAppContext);
        }

        public static BusybodyTestContext Setup(BusybodyConfig busybodyConfig)
        {
            var fakeAppContext = new FakeAppContextBuilder()
                .WithConfig(busybodyConfig)
                .Build();
            return Setup(fakeAppContext);
        }

        public static BusybodyTestContext Setup(FakeAppContext fakeAppContext)
        {
            var testContext = new BusybodyTestContext
            {
                EventHandler = new TestEventHandler(),
                Daemon = new BusybodyDaemon(),
                TestAppContext = fakeAppContext,
            };

            testContext.TestAppContext.EventBus.RegisterHandler("All", typeof (TestEventHandler));
            testContext.TestAppContext.EventBus.RegisterHandler("All", typeof (HostEventHandler));
            testContext.TestAppContext.EventBus.RegisterHandler("All", typeof (EventLogRepository));
            testContext.FakePingTest = testContext.TestAppContext.FakeTestFactory.GetTest<FakePingTest>("Ping");
            testContext.TestAppContext.EventLogRepository.ClearEvents();
            testContext.EventHandler.Clear();
            AppContext.Instance = testContext.TestAppContext;
            return testContext;
        }
    }
}