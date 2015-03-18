using System;
using System.Collections.Generic;
using Busybody;
using Busybody.Config;
using NUnit.Framework;

namespace BusybodyTests
{
    [TestFixture]
    public class When_starting_the_daemon_and_multiple_tests_are_configured
    {
        [SetUp]
        public void SetUp()
        {
            AppContext.Instance = new FakeAppContextBuilder()
                .WithBasicConfiguration()
                .Build();
            
            var daemon = new BusybodyDaemon();
            daemon.Start();
        }

        [Test]
        public void It_should_run_each_test_once()
        {
            

        }

        [Test]
        public void It_should_pause_between_tests()
        {
            
        }

        [Test]
        public void It_should_rerun_the_tests_after_pausing()
        {
            
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
            return this;
        }

        public FakeAppContext Build()
        {
            return _appContext;
        }
    }

    public class FakeAppContext : IAppContext
    {
        public IEventLogger EventLogger { get; private set; }
        public BusybodyConfig Config { get; set; }

        public FakeAppContext()
        {
            EventLogger = new FakeEventLogger();
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