using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Busybody;
using Busybody.Events;
using BusybodyTests.Fakes;
using FluentAssertions;
using NUnit.Framework;

namespace BusybodyTests
{
    [TestFixture]
    public class When_starting_the_daemon : Daemon_up_down_tests
    {
        [SetUp]
        public void SetUp()
        {
            _testContext.TestAppContext.Config.PollingInterval = 2 * 60;

            _testContext.Daemon.Start();
            _testContext.FakePingTest.WaitForNumberOfExecutions(2);
        }

        [Test]
        public void It_should_run_each_test_once()
        {
            _testContext.FakePingTest.ExecutedCount.Should().BeGreaterOrEqualTo(1);
        }

        [Test]
        public void It_should_pause_between_tests()
        {
            _testContext.TestAppContext.FakeThreading._sleeps.Count.Should().BeGreaterOrEqualTo(60*10*2);
            _testContext.TestAppContext.FakeThreading._sleeps[0].Should().Be(100);
        }

        [Test]
        public void It_should_rerun_the_tests_after_pausing()
        {
            _testContext.FakePingTest.ExecutedCount.Should().BeGreaterOrEqualTo(2);
        }
    }

    [TestFixture]
    public class When_starting_the_daemon_and_test_is_configured_and_host_is_down : Daemon_up_down_tests
    {

        [SetUp]
        public void SetUp()
        {
            _testContext.FakePingTest.StubResult(false);
            _testContext.Daemon.Start();
            _testContext.FakePingTest.WaitForNumberOfExecutions(1);
        }

        [Test]
        public void It_should_raise_event_that_host_is_down()
        {
            _testContext.EventHandler.AssertSingleHostStateReceived(HostState.DOWN);
        }
    }

    [TestFixture]
    public class When_starting_the_daemon_and_test_is_configured_and_host_is_up : Daemon_up_down_tests
    {

        [SetUp]
        public void SetUp()
        {
            _testContext.FakePingTest.StubResult(true);
            _testContext.Daemon.Start();
            _testContext.FakePingTest.WaitForNumberOfExecutions(1);
        }

        [Test]
        public void It_should_raise_event_that_host_is_up()
        {
            _testContext.EventHandler.AssertSingleHostStateReceived(HostState.UP);
        }
    }

    [TestFixture]
    public class When_starting_the_daemon_and_test_is_configured_and_host_is_down_then_up : Daemon_up_down_tests
    {

        [SetUp]
        public void SetUp()
        {
            _testContext.FakePingTest.StubResult(new[] {false, true});
            _testContext.Daemon.Start();
            _testContext.EventHandler.WaitForNumberOfEventsOfType<HostStateEvent>(2);
        }

        [Test]
        public void It_should_raise_two_events()
        {
            _testContext.EventHandler.AssertMultipleHostStateReceived(HostState.DOWN, HostState.UP);
        }
    }

    [TestFixture]
    public class When_starting_the_daemon_and_test_is_configured_and_host_is_up_and_polled_multiple_times : Daemon_up_down_tests
    {

        [SetUp]
        public void SetUp()
        {
            _testContext.FakePingTest.StubResult(new[] {true, true});
            _testContext.Daemon.Start();
            _testContext.FakePingTest.WaitForNumberOfExecutions(2);
        }

        [Test]
        public void It_should_only_raise_one_event()
        {
            _testContext.EventHandler.AssertSingleHostStateReceived(HostState.UP);
        }
    }

    public class Daemon_up_down_tests
    {
        protected TestContext _testContext;

        [SetUp]
        public void BaseSetUp()
        {
            _testContext = new TestContext();
        }

        [TearDown]
        public void TearDown()
        {
            _testContext.Daemon.Stop();
        }
    }

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

    public class TestEventHandler
    {
        public readonly List<EventNotification> ReceivedEventNotifications = new List<EventNotification>();

        public void Handle(EventNotification eventNotification)
        {
            ReceivedEventNotifications.Add(eventNotification);
        }

        public void WaitForNumberOfEventsOfType<T>(int count) where T : BusybodyEvent
        {
            WaitForNumberOfEventsMatching(count, e => e.GetType() == typeof (T));
        }

        public void WaitForNumberOfEventsMatching(int count, Predicate<BusybodyEvent> pred)
        {
            TestUtility.WaitFor(() => ReceivedEventNotifications.Count(x => pred(x.Event)) >= count);
        }

        public void AssertSingleHostStateReceived(HostState hostState)
        {
            ReceivedEventNotifications
                .Select(x => x.Event as HostStateEvent)
                .Where(x => x != null)
                .Should()
                .ContainSingle(x => x.State == hostState);
        }

        public void AssertMultipleHostStateReceived(params HostState[] hostStates)
        {
            ReceivedEventNotifications
                .Where(x => x.Event is HostStateEvent)
                .Select(x => x.Event as HostStateEvent)
                .Select(x => x.State)
                .Should()
                .BeEquivalentTo(hostStates);
        }
    }

    public static class TestUtility
    {
        public static void WaitFor(Func<bool> func)
        {
            while (true)
            {
                var cnt = 0;
                if (!func())
                    Thread.Sleep(100);
                else
                    return;
                if (cnt++ > 20)
                    Assert.Fail("Timed out waiting");
            }
        }
    }
}