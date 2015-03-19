using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Busybody;
using Busybody.Config;
using FluentAssertions;
using NUnit.Framework;

namespace BusybodyTests
{
    [TestFixture]
    public class When_starting_the_daemon_and_multiple_tests_are_configured
    {
        FakeAppContext _fakeAppContext;

        [SetUp]
        public void SetUp()
        {
            _fakeAppContext = new FakeAppContextBuilder()
                .WithBasicConfiguration()
                .Build();

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
            _fakeAppContext.FakeThreading._sleeps.Count.Should().Be(60*10*2);
            _fakeAppContext.FakeThreading._sleeps[0].Should().Be(100);
        }

        [Test]
        public void It_should_rerun_the_tests_after_pausing()
        {
            var fakePingTest = (FakePingTest)_fakeAppContext.FakeTestFactory.Tests["Ping"];
            fakePingTest.ExecutedCount.Should().BeGreaterOrEqualTo(2);
        }
    }

    public class FakeAppContextBuilder
    {
        FakeAppContext _appContext;

        public FakeAppContextBuilder()
        {
            _appContext = new FakeAppContext();
        }

        public FakeAppContextBuilder WithBasicConfiguration()
        {
            var config = new ConfigBuilder()
                .WithHost("Local Machine", "127.0.0.1")
                .WithTest(new PingTestConfig())
                .BuildHostConfig()
                .BuildConfig();
            _appContext.Config = config;
            _appContext.FakeTestFactory.Tests.Add("Ping", new FakePingTest());
            return this;
        }

        public FakeAppContext Build()
        {
            return _appContext;
        }
    }

    public class FakePingTest : IBusybodyTest
    {
        public int ExecutedCount;
        public HostConfig LastHostConfig;
        public HostTestConfig LastHostTestConfig;

        public bool Execute(HostConfig host, HostTestConfig test)
        {
            ExecutedCount++;
            LastHostConfig = host;
            LastHostTestConfig = test;
            return true;
        }
    }

    public class FakeAppContext : IAppContext
    {
        public IEventLogger EventLogger { get; private set; }
        public ITestFactory TestFactory { get; private set; }
        public IThreading Threading { get; private set; }
        public BusybodyConfig Config { get; set; }
        public FakeTestFactory FakeTestFactory { get { return (FakeTestFactory)TestFactory;  } }
        public FakeThreading FakeThreading { get { return (FakeThreading) Threading; } }

        public FakeAppContext()
        {
            EventLogger = new FakeEventLogger();
            TestFactory = new FakeTestFactory();
            Threading = new FakeThreading();
        }
    }

    public class FakeThreading : IThreading
    {
        public List<int> _sleeps = new List<int>();

        public void Sleep(int m)
        {
            _sleeps.Add(m);
        }
    }

    public class FakeTestFactory : ITestFactory
    {
        public Dictionary<string, IBusybodyTest> Tests = new Dictionary<string, IBusybodyTest>();

        public IBusybodyTest Create(string name)
        {
            return Tests[name];
        }
    }

    public class FakeEventLogger : IEventLogger
    {
        public readonly List<string> Events = new List<string>();

        public void Publish(string eventText)
        {
            Events.Add(eventText);
        }
    }
}