using Busybody;
using Busybody.Events;
using NUnit.Framework;

namespace BusybodyTests
{
    [TestFixture]
    public class test_failed : Given_a_host_test_result
    {

        [SetUp]
        public void SetUp()
        {
            var hostTestResultEvent = new HostTestResultEvent
            {
                HostNickname = "Nickname",
                TestName = "Ping",
                TestResult = false,
            };
            _hostEventHandler.Handle(hostTestResultEvent);
            _testContext.TestAppContext.EventBus.DispatchPending();
        }

        [Test]
        public void It_should_raise_a_host_state_changed_event()
        {
            _testContext.EventHandler.AssertSingleEventReceived<HostStateEvent>(x => true);

        }
    }

    [TestFixture]
    public class Given_a_host_test_result
    {
        protected TestContext _testContext;
        protected HostEventHandler _hostEventHandler;

        [SetUp]
        public void BaseSetUp()
        {
            _testContext = TestContext.Setup();
            _hostEventHandler = new HostEventHandler();
        }
    }
}
