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

        public static TestContext Setup()
        {
            var testContext = new TestContext
            {
                EventHandler = new TestEventHandler(),
                Daemon = new BusybodyDaemon(),
                TestAppContext = new FakeAppContextBuilder()
                    .WithBasicConfiguration()
                    .Build(),
            };

            testContext.TestAppContext.EventBus.RegisterHandler("All", typeof(TestEventHandler));
            testContext.FakePingTest = testContext.TestAppContext.FakeTestFactory.GetTest<FakePingTest>("Ping");
            testContext.EventHandler.Clear();
            AppContext.Instance = testContext.TestAppContext;
            return testContext;
        }
    }
}