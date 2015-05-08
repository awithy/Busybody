using System.Collections.Generic;
using Busybody;
using Busybody.Events;

namespace BusybodyTests.Fakes
{
    public class FakeEventLogger : IEventLogger
    {
        public readonly List<string> Events = new List<string>();

        public void Handle(HostStateEvent @event)
        {
            Events.Add(@event.ToLogString());
        }

        public void Handle(BusybodyStartedEvent @event)
        {
            Events.Add(@event.ToLogString());
        }

        public void Handle(EmailAlertSentEvent @event)
        {
            Events.Add(@event.ToLogString());
        }
    }
}