using Busybody;
using Busybody.Events;
using BusybodyTests.Helpers;
using NUnit.Framework;

namespace BusybodyTests
{
    [TestFixture]
    public class Given_an_empty_repo_and_a_failed_test_result_is_received : HostTestResultBase
    {

        [SetUp]
        public void SetUp()
        {
            _hostEventHandler.Handle(_failedTestResult);
            _testContext.TestAppContext.EventBus.DispatchPending();
        }

        [Test]
        public void It_should_raise_an_event_that_the_host_is_down()
        {
            _testContext.EventHandler.AssertSingleEventReceived<HostStateEvent>(x => x.State == HostState.DOWN);
        }

        [Test]
        public void The_initial_state_should_be_set_to_true()
        {
            _testContext.EventHandler.AssertSingleEventReceived<HostStateEvent>(x => x.IsInitialState);
        }
    }

    [TestFixture]
    public class Given_a_host_that_is_up_and_a_successful_test_result_is_received : HostTestResultBase
    {
        [SetUp]
        public void SetUp()
        {
            _hostEventHandler.Handle(_successfulTestResult);
            _ClearEvents();

            //Do
            _hostEventHandler.Handle(_successfulTestResult);
            _testContext.TestAppContext.EventBus.DispatchPending();
        }

        [Test]
        public void It_should_not_raise_a_host_state_changed_event()
        {
            _testContext.EventHandler.AssertNoEventsReceived<HostStateEvent>();
        }
    }

    [TestFixture]
    public class Given_a_host_that_is_down_and_a_successful_test_result_is_received : HostTestResultBase
    {
        [SetUp]
        public void SetUp()
        {
            _hostEventHandler.Handle(_failedTestResult);
            _ClearEvents();

            //Do
            _hostEventHandler.Handle(_successfulTestResult);
            _testContext.TestAppContext.EventBus.DispatchPending();
        }

        [Test]
        public void It_should_raise_a_host_state_changed_event_UP()
        {
            _testContext.EventHandler.AssertSingleEventReceived<HostStateEvent>(x => x.State == HostState.UP);
        }

        [Test]
        public void The_initial_state_should_be_set_to_false()
        {
            _testContext.EventHandler.AssertSingleEventReceived<HostStateEvent>(x => !x.IsInitialState);
        }
    }

    [TestFixture]
    public class Given_a_host_that_is_up_and_a_failed_test_result_is_received : HostTestResultBase
    {
        [SetUp]
        public void SetUp()
        {
            _hostEventHandler.Handle(_successfulTestResult);
            _ClearEvents();

            //Do
            _hostEventHandler.Handle(_failedTestResult);
            _testContext.TestAppContext.EventBus.DispatchPending();
        }

        [Test]
        public void It_should_raise_a_host_state_changed_event_DOWN()
        {
            _testContext.EventHandler.AssertSingleEventReceived<HostStateEvent>(x => x.State == HostState.DOWN);
        }
    }

    [TestFixture]
    public class Given_a_host_that_is_down_and_a_failed_test_result_is_received : HostTestResultBase
    {
        [SetUp]
        public void SetUp()
        {
            _hostEventHandler.Handle(_failedTestResult);
            _ClearEvents();

            //Do
            _hostEventHandler.Handle(_failedTestResult);
            _testContext.TestAppContext.EventBus.DispatchPending();
        }

        [Test]
        public void It_should_raise_a_host_state_changed_event_DOWN()
        {
            _testContext.EventHandler.AssertNoEventsReceived<HostStateEvent>();
        }
    }

    [TestFixture]
    public class HostTestResultBase
    {
        protected BusybodyTestContext _testContext;
        protected HostEventHandler _hostEventHandler;
        protected HostTestResultEvent _successfulTestResult;
        protected HostTestResultEvent _failedTestResult;

        [SetUp]
        public void BaseSetUp()
        {
            _successfulTestResult = new HostTestResultEvent
            {
                HostNickname = "Nickname",
                TestName = "Ping",
                TestResult = true,
            };
            _failedTestResult = new HostTestResultEvent
            {
                HostNickname = "Nickname",
                TestName = "Ping",
                TestResult = false,
            };

            _testContext = BusybodyTestContext.Setup();
            _hostEventHandler = new HostEventHandler();
        }

        protected void _ClearEvents()
        {
            _testContext.TestAppContext.EventBus.DispatchPending();
            _testContext.EventHandler.Clear();
        }
    }
}