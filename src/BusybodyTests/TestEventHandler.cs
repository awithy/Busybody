using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Busybody;
using Busybody.Events;
using FluentAssertions;

namespace BusybodyTests
{
    public class TestEventHandler : IHandle<HostStateEvent>
    {
        public static List<HostStateEvent> ReceivedHostStateEvents = new List<HostStateEvent>();

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

        public void Handle(HostStateEvent @event)
        {
            ReceivedHostStateEvents.Add(@event);
        }

        public static void Clear()
        {
            ReceivedHostStateEvents.Clear();
        }
    }
}