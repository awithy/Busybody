using System;
using System.Linq;
using Busybody;
using Busybody.Events;
using Busybody.WebServer;
using BusybodyTests.Helpers;
using FluentAssertions;
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
    public class Given_a_host_test_configured_with_multiple_allowable_failures_and_a_single_failure_occurs : HostTestResultBase
    {
        [SetUp]
        public void SetUp()
        {
            _testContext.TestAppContext.Config.Hosts.First().Tests.First().AllowableFailures = 1;
            _hostEventHandler.Handle(_successfulTestResult);
            _ClearEvents();

            //Do
            _hostEventHandler.Handle(_failedTestResult);
            _testContext.TestAppContext.EventBus.DispatchPending();
        }

        [Test]
        public void It_should_not_raise_an_event()
        {
            _testContext.EventHandler.AssertNoEventsReceived<HostStateEvent>();
        }
    }

    [TestFixture]
    public class When_a_host_has_been_down_in_the_last_24_hours_but_system_has_been_up_longer : HostTestResultBase
    {
        [SetUp]
        public void SetUp()
        {
            var startTime = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1).Add(TimeSpan.FromSeconds(1)));
            _testContext.TestAppContext.StartTime = startTime;
            _failedTestResult.Timestamp = DateTime.UtcNow;
            _successfulTestResult.Timestamp = DateTime.UtcNow;

            //Do
            _hostEventHandler.Handle(_failedTestResult);
            _hostEventHandler.Handle(_successfulTestResult);
            _testContext.TestAppContext.EventBus.DispatchPending();
        }

        [Test]
        public void It_should_be_in_state_warning()
        {
            var hostController = new HostsController();
            var hosts = hostController.GetHosts().HostGroups.SelectMany(x => x.Hosts);
            hosts.First().State.Should().Be("WARN");
        }
    }

    [TestFixture]
    public class When_a_host_has_been_down_more_than_24_hours_ago_but_has_been_up_for_more_than_24_hours : HostTestResultBase
    {
        [SetUp]
        public void SetUp()
        {
            var startTime = DateTime.UtcNow.Subtract(TimeSpan.FromDays(2).Add(TimeSpan.FromSeconds(1)));
            _testContext.TestAppContext.StartTime = startTime;
            _failedTestResult.Timestamp = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1).Add(TimeSpan.FromMinutes(5)));
            _successfulTestResult.Timestamp = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1).Add(TimeSpan.FromMinutes(1)));

            //Do
            _hostEventHandler.Handle(_failedTestResult);
            _hostEventHandler.Handle(_successfulTestResult);
            _testContext.TestAppContext.EventBus.DispatchPending();
        }

        [Test]
        public void It_should_be_in_state_UP()
        {
            var hostController = new HostsController();
            var hosts = hostController.GetHosts().HostGroups.SelectMany(x => x.Hosts);
            hosts.First().State.Should().Be("UP");
        }
    }

    [TestFixture]
    public class When_a_host_has_been_down_in_the_last_24_hours_but_last_state_change_was_when_system_started : HostTestResultBase
    {
        [SetUp]
        public void SetUp()
        {
            var startTime = DateTime.UtcNow.Subtract(TimeSpan.FromHours(1));
            _testContext.TestAppContext.StartTime = startTime;
            _successfulTestResult.Timestamp = startTime.Add(TimeSpan.FromMinutes(1)); //Give it a bit to do the first state change.

            //Do
            _hostEventHandler.Handle(_successfulTestResult);
            _testContext.TestAppContext.EventBus.DispatchPending();
        }

        [Test]
        public void It_should_be_in_state_up()
        {
            var hostController = new HostsController();
            var hosts = hostController.GetHosts().HostGroups.SelectMany(x => x.Hosts);
            hosts.First().State.Should().Be("UP");
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
                HostNickname = "Local Machine",
                TestName = "Ping",
                TestResult = true,
            };
            _failedTestResult = new HostTestResultEvent
            {
                HostNickname = "Local Machine",
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