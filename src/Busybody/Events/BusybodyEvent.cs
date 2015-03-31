using System;

namespace Busybody.Events
{
    public class BusybodyEvent
    {
        public DateTime Timestamp { get; set; }

        public virtual string ToLogString()
        {
            return "<Not Used>";
        }
    }
}