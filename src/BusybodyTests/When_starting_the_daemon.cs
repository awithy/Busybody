using System;
using Busybody.Config;
using Busybody.Events;
using BusybodyTests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace BusybodyTests
{
    [Category("LongRunning")]
    [TestFixture]
    public class When_starting_the_daemon_and_running_host_tests : Daemon_up_down_tests
    {
        [SetUp]
        public void SetUp()
        {
            _testContext.TestAppContext.Config.PollingInterval = 1;

            _testContext.Daemon.Start();
            _testContext.FakePingTest.WaitForNumberOfExecutions(2);
        }

        [Test]
        public void It_should_run_each_test_once()
        {
            _testContext.FakePingTest.ExecutedCount.Should().BeGreaterOrEqualTo(1);
        }

        [Category("TestMe")]
        [Test]
        public void It_should_rerun_the_tests_after_pausing()
        {
            _testContext.FakePingTest.ExecutedCount.Should().BeGreaterOrEqualTo(2);
        }

        [Test]
        public void It_should_start_the_system_monitor_role_service()
        {
            _testContext.TestAppContext.FakeSystemStatusWriter.LastStatusText.Should().Contain("Busybody");
        }
    }

    [Category("LongRunning")]
    [TestFixture]
    public class When_starting_the_daemon_and_test_is_configured_and_host_is_down : Daemon_up_down_tests
    {
        bool _emailReceived;

        [SetUp]
        public void SetUp()
        {
            _testContext.FakePingTest.StubResult(false);
            _testContext.Daemon.Start();
            _testContext.FakePingTest.WaitForNumberOfExecutions(1);
            _testContext.TestAppContext.Config.EmailAlertConfiguration = new EmailAlertConfiguration {Host = "host", FromAddress = "a@a.com", Password = "password", Port = 123, ToEmailAddress = "b@b.com"};
            _emailReceived = _testContext.TestAppContext.FakeEmailAlertingInterface.WaitForEmails(1);
        }

        [Test]
        public void It_should_raise_event_that_host_is_down()
        {
            _testContext.EventHandler.WaitForHostStateEvents(1);
            _testContext.EventHandler.AssertSingleHostStateReceived(HostState.DOWN);
        }

        [Test]
        public void It_should_send_an_email_alert()
        {
            _emailReceived.Should().BeTrue();
        }
    }

    [Category("LongRunning")]
    [TestFixture]
    public class When_starting_the_daemon_and_test_is_configured_and_host_is_up : Daemon_up_down_tests
    {

        [SetUp]
        public void SetUp()
        {
            _testContext.FakePingTest.StubResult(true);
            _testContext.Daemon.Start();
        }

        [Test]
        public void It_should_raise_event_that_host_is_up()
        {
            _testContext.EventHandler.WaitForHostStateEvents(1);
            _testContext.EventHandler.AssertSingleHostStateReceived(HostState.UP);
        }
    }

    [Category("LongRunning")]
    [TestFixture]
    public class When_starting_the_daemon_and_test_is_configured_and_host_is_down_then_up : Daemon_up_down_tests
    {

        [SetUp]
        public void SetUp()
        {
            _testContext.FakePingTest.StubResult(new[] {false, true});
            _testContext.Daemon.Start();
        }

        [Test]
        public void It_should_raise_two_events()
        {
            _testContext.EventHandler.WaitForHostStateEvents(2);
            _testContext.EventHandler.AssertMultipleHostStateReceived(HostState.DOWN, HostState.UP);
        }
    }

    [Category("LongRunning")]
    [TestFixture]
    public class When_starting_the_daemon_and_test_is_configured_and_host_is_up_and_polled_multiple_times : Daemon_up_down_tests
    {
        [SetUp]
        public void SetUp()
        {
            _testContext.FakePingTest.StubResult(new[] {true, true});
            _testContext.Daemon.Start();
        }

        [Test]
        public void It_should_only_raise_one_event()
        {
            _testContext.EventHandler.WaitForHostStateEvents(1).Should().BeTrue();
            _testContext.EventHandler.AssertSingleHostStateReceived(HostState.UP);
        }
    }

    public class Daemon_up_down_tests
    {
        protected BusybodyTestContext _testContext;

        [SetUp]
        public void BaseSetUp()
        {
            _testContext = BusybodyTestContext.Setup();
        }

        [TearDown]
        public void TearDown()
        {
            _testContext.Daemon.Stop();
        }
    }
}