using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Busybody;
using Busybody.Events;
using FluentAssertions;

namespace BusybodyTests.Helpers
{
    public class TestEventHandler : IHandle<HostStateEvent>, IHandle<HostTestResultEvent>
    {
        public static BlockingCollection<BusybodyEvent> ReceivedEvents = new BlockingCollection<BusybodyEvent>();

        public void Handle(HostStateEvent @event)
        {
            ReceivedEvents.Add(@event);
        }

        public void Handle(HostTestResultEvent @event)
        {
            ReceivedEvents.Add(@event);
        }

        public bool WaitForHostStateEvents(int count)
        {
            var startTime = DateTime.Now;
            while (_GetEventsOfType<HostStateEvent>().Count() < count)
            {
                Thread.Sleep(100);
                if ((DateTime.Now - startTime).TotalSeconds > 5)
                    return false;
            }
            return true;
        }

        public void AssertSingleHostStateReceived(HostState hostState)
        {
            _GetEventsOfType<HostStateEvent>()
                .Should()
                .ContainSingle(x => x.State == hostState);
        }

        public void AssertMultipleHostStateReceived(params HostState[] hostStates)
        {
            _GetEventsOfType<HostStateEvent>()
                .Select(x => x.State)
                .Should()
                .BeEquivalentTo(hostStates);
        }

        public void AssertSingleEventReceived<T>(Predicate<T> predicate) where T : BusybodyEvent
        {
            ReceivedEvents
                .Where(x => x.GetType() == typeof (T))
                .SingleOrDefault(x => predicate((T) x))
                .Should()
                .NotBeNull();
        }

        IEnumerable<T> _GetEventsOfType<T>() where T : BusybodyEvent
        {
            var receivedEvents = ReceivedEvents.ToArray();
            return receivedEvents
                .Where(x => x.GetType() == typeof (T))
                .Select(x => (T) x);
        }

        public void Clear()
        {
            ReceivedEvents = new BlockingCollection<BusybodyEvent>();
        }

        public void AssertNoEventsReceived<T>() where T : BusybodyEvent
        {
            _GetEventsOfType<T>().Should().BeEmpty();
        }
    }
}