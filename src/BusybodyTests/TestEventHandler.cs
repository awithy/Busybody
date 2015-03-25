using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Busybody;
using Busybody.Events;
using FluentAssertions;
using NUnit.Framework;

namespace BusybodyTests
{
    public class TestEventHandler
    {
        public readonly List<EventNotification> ReceivedEventNotifications = new List<EventNotification>();
        readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        public void Handle(EventNotification eventNotification)
        {
            ReceivedEventNotifications.Add(eventNotification);
            _autoResetEvent.Set();
        }

        public bool WaitForNumberOfEventsOfType<T>(int count) where T : BusybodyEvent
        {
            while (ReceivedEventNotifications.Count(x => x.Event.GetType() == typeof(T)) < count)
            {
                var result = _autoResetEvent.WaitOne(TimeSpan.FromSeconds(5));
                if (!result)
                    return false;
            }
            return true;
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
}