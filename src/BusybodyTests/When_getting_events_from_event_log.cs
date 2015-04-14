using System.Linq;
using Busybody.Events;
using BusybodyTests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace BusybodyTests
{
    [TestFixture]
    public class When_getting_events_from_event_log : EventLogTests
    {
        [SetUp]
        public void SetUp()
        {
            _testContext.TestAppContext.EventBus.Publish("All", new HostStateEvent("Host 1", HostState.DOWN));
            _testContext.TestAppContext.EventBus.Publish("All", new HostStateEvent("Host 2", HostState.DOWN));
            _testContext.TestAppContext.EventBus.Publish("All", new SystemErrorEvent("Message", "Detail"));
            _testContext.TestAppContext.EventBus.DispatchPending();
        }

        [Test]
        public void It_should_log_host_state_change_events()
        {
            _testContext.TestAppContext.EventLogRepository.GetEvents().Count(x => x.EventType == typeof(HostStateEvent).Name).Should().Be(2);
        }

        [Test]
        public void It_should_log_system_errors()
        {
            _testContext.TestAppContext.EventLogRepository.GetEvents().Count(x => x.EventType == typeof (SystemErrorEvent).Name).Should().Be(1);
        }
    }

    public class EventLogTests
    {
        protected BusybodyTestContext _testContext;

        [SetUp]
        public void BaseSetUp()
        {
            _testContext = BusybodyTestContext.Setup();
        }
    }
}
