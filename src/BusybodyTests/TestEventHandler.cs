using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Busybody;
using Busybody.Events;
using FluentAssertions;

namespace BusybodyTests
{
    public class TestEventHandler : IHandle<HostStateEvent>, IHandle<HostTestResultEvent>
    {
        public static List<HostStateEvent> ReceivedHostStateEvents = new List<HostStateEvent>();
        public static List<BusybodyEvent> ReceivedEvents = new List<BusybodyEvent>();

        public void Handle(HostStateEvent @event)
        {
            ReceivedHostStateEvents.Add(@event);
            ReceivedEvents.Add(@event);
        }

        public void Handle(HostTestResultEvent @event)
        {
            ReceivedEvents.Add(@event);
        }

        public bool WaitForHostStateEvents(int count)
        {
            var startTime = DateTime.Now;
            while (ReceivedHostStateEvents.Count < count)
            {
                Thread.Sleep(100);
                if ((DateTime.Now - startTime).TotalSeconds > 5)
                    return false;
            }
            return true;
        }

        public void AssertSingleHostStateReceived(HostState hostState)
        {
            ReceivedHostStateEvents
                .Should()
                .ContainSingle(x => x.State == hostState);
        }

        public void AssertMultipleHostStateReceived(params HostState[] hostStates)
        {
            ReceivedHostStateEvents
                .Select(x => x.State)
                .Should()
                .BeEquivalentTo(hostStates);
        }

        public static void Clear()
        {
            ReceivedHostStateEvents.Clear();
        }

        public void AssertEventReceived<T>(Predicate<T> predicate) where T : BusybodyEvent
        {
            ReceivedEvents
                .Where(x => x.GetType() == typeof (T))
                .SingleOrDefault(x => predicate((T) x))
                .Should()
                .NotBeNull();
        }
    }
}