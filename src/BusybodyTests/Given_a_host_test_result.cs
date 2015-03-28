using Busybody;
using Busybody.Events;
using NUnit.Framework;

namespace BusybodyTests
{
    [TestFixture]
    public class Given_a_host_test_result_and_test_failed : Given_a_host_test_result
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
        public void It_should_raise_a_host_state_changed_event_DOWN()
        {
            _testContext.EventHandler.AssertSingleEventReceived<HostStateEvent>(x => x.State == HostState.DOWN);
        }
    }

    [TestFixture]
    public class Given_a_host_test_result_passed_and_the_host_is_UP : Given_a_host_test_result
    {
        [SetUp]
        public void SetUp()
        {
            var hostTestResultEvent = new HostTestResultEvent
            {
                HostNickname = "Nickname",
                TestName = "Ping",
                TestResult = true,
            };
            _hostEventHandler.Handle(hostTestResultEvent);
            _ClearEvents();

            //Do
            _hostEventHandler.Handle(hostTestResultEvent);
            _testContext.TestAppContext.EventBus.DispatchPending();
        }

        [Test]
        public void It_should_not_raise_a_host_state_changed_event()
        {
            _testContext.EventHandler.AssertNoEventsReceived<HostStateEvent>();
        }
    }

    [TestFixture]
    public class Given_a_host_test_result_passed_and_the_host_is_DOWN : Given_a_host_test_result
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
            _ClearEvents();

            //Do
            hostTestResultEvent.TestResult = true;
            _hostEventHandler.Handle(hostTestResultEvent);
            _testContext.TestAppContext.EventBus.DispatchPending();
        }

        [Test]
        public void It_should_raise_a_host_state_changed_event_UP()
        {
            _testContext.EventHandler.AssertSingleEventReceived<HostStateEvent>(x => x.State == HostState.UP);
        }
    }

    [TestFixture]
    public class Given_a_host_test_result_failed_and_the_host_is_UP : Given_a_host_test_result
    {
        [SetUp]
        public void SetUp()
        {
            var hostTestResultEvent = new HostTestResultEvent
            {
                HostNickname = "Nickname",
                TestName = "Ping",
                TestResult = true,
            };
            _hostEventHandler.Handle(hostTestResultEvent);
            _ClearEvents();

            //Do
            hostTestResultEvent.TestResult = false;
            _hostEventHandler.Handle(hostTestResultEvent);
            _testContext.TestAppContext.EventBus.DispatchPending();
        }

        [Test]
        public void It_should_raise_a_host_state_changed_event_DOWN()
        {
            _testContext.EventHandler.AssertSingleEventReceived<HostStateEvent>(x => x.State == HostState.DOWN);
        }
    }

    [TestFixture]
    public class Given_a_host_test_result_failed_and_the_host_is_DOWN : Given_a_host_test_result
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
            _ClearEvents();

            //Do
            hostTestResultEvent.TestResult = false;
            _hostEventHandler.Handle(hostTestResultEvent);
            _testContext.TestAppContext.EventBus.DispatchPending();
        }

        [Test]
        public void It_should_raise_a_host_state_changed_event_DOWN()
        {
            _testContext.EventHandler.AssertNoEventsReceived<HostStateEvent>();
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

        protected void _ClearEvents()
        {
            _testContext.TestAppContext.EventBus.DispatchPending();
            _testContext.EventHandler.Clear();
        }
    }
}
