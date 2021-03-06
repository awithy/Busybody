﻿using System;

namespace Busybody.Events
{
    public class BusybodyStartedEvent : BusybodyEvent
    {
        public DateTime StartedDateTime { get; private set; }

        public BusybodyStartedEvent(DateTime startedDateTime)
        {
            StartedDateTime = startedDateTime;
        }

        public override string ToLogString()
        {
            return string.Format("Busybody started");
        }
    }
}
