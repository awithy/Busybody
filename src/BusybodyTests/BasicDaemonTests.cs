using System.Threading;
using Busybody;
using BusybodyTests.Fakes;
using FluentAssertions;
using NUnit.Framework;

namespace BusybodyTests
{
    [TestFixture]
    public class When_starting_the_daemon
    {
        FakeAppContext _fakeAppContext;

        [SetUp]
        public void SetUp()
        {
            _fakeAppContext = new FakeAppContextBuilder()
                .WithBasicConfiguration()
                .Build();

            _fakeAppContext.Config.PollingInterval = 2 * 60;

            AppContext.Instance = _fakeAppContext;
            
            var daemon = new BusybodyDaemon();
            daemon.Start();

            _WaitFor2Tests();

            daemon.Stop();
        }

        void _WaitFor2Tests()
        {
            var waits = 0;
            while (true)
            {
                var fakePingTest = (FakePingTest) _fakeAppContext.FakeTestFactory.Tests["Ping"];
                var count = fakePingTest.ExecutedCount;
                if (count >= 2)
                    return;
                Thread.Sleep(100);
                if (waits > 100)
                    Assert.Fail("Timed out waiting for 2 rounds of tests to finish");
            }
        }

        [Test]
        public void It_should_run_each_test_once()
        {
            var fakePingTest = (FakePingTest)_fakeAppContext.FakeTestFactory.Tests["Ping"];
            fakePingTest.ExecutedCount.Should().BeGreaterOrEqualTo(1);
        }

        [Test]
        public void It_should_pause_between_tests()
        {
            _fakeAppContext.FakeThreading._sleeps.Count.Should().BeGreaterOrEqualTo(60*10*2);
            _fakeAppContext.FakeThreading._sleeps[0].Should().Be(100);
        }

        [Test]
        public void It_should_rerun_the_tests_after_pausing()
        {
            var fakePingTest = (FakePingTest)_fakeAppContext.FakeTestFactory.Tests["Ping"];
            fakePingTest.ExecutedCount.Should().BeGreaterOrEqualTo(2);
        }
    }

    [TestFixture]
    public class When_starting_the_daemon_and_test_is_configured_and_host_is_down : Daemon_up_down_tests
    {

        [SetUp]
        public void SetUp()
        {
            _SetupContext();

            _fakePingTest.StubResult(false);
            
            var daemon = new BusybodyDaemon();
            daemon.Start();
            daemon.Stop();
        }

        [Test]
        public void It_should_alert_that_the_test_failed()
        {
            _receivedText.Should().Be("Host: Local Machine, State: DOWN");
        }
    }

    [TestFixture]
    public class When_starting_the_daemon_and_test_is_configured_and_host_is_up : Daemon_up_down_tests
    {

        [SetUp]
        public void SetUp()
        {
            _SetupContext();

            _fakePingTest.StubResult(true);
            
            var daemon = new BusybodyDaemon();
            daemon.Start();
            daemon.Stop();
        }

        [Test]
        public void It_should_alert_that_the_test_failed()
        {
            _receivedText.Should().Be("Host: Local Machine, State: UP");
        }
    }

    public class Daemon_up_down_tests
    {
        protected FakeAppContext _fakeAppContext;
        protected FakePingTest _fakePingTest;
        protected string _receivedText;

        protected void _SetupContext()
        {
            _fakeAppContext = new FakeAppContextBuilder()
                .WithBasicConfiguration()
                .Build();

            _receivedText = "";
            AppContext.Instance = _fakeAppContext;
            AppContext.Instance.EventBus.Subscribe(new EventSubscription
            {
                Name = "Test Subscription",
                EventStreamName = "All",
                Recipient = s => _receivedText = ((HostStateEvent) s.Event).StateText,
            });

            _fakePingTest = (FakePingTest) _fakeAppContext.FakeTestFactory.Tests["Ping"];
        }
    }
}