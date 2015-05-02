using System;

namespace Busybody.Events
{
    public class SystemStartedEvent : BusybodyEvent
    {
        public DateTime StartedDateTime { get; private set; }

        public SystemStartedEvent(DateTime startedDateTime)
        {
            StartedDateTime = startedDateTime;
        }
    }
}
