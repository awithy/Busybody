using System.Collections.Generic;
using Busybody;

namespace BusybodyTests.Fakes
{
    public class FakeEventLogger : IEventLogger
    {
        public readonly List<string> Events = new List<string>();

        public void Publish(string eventText)
        {
            Events.Add(eventText);
        }
    }
}