using System;
using Busybody;

namespace BusybodyTests.Fakes
{
    public class FakeSystemStatusWriter : ISystemStatusWriter
    {
        public string LastStatusText { get; set; }

        public void Write(string systemStatusText)
        {
            LastStatusText = systemStatusText;
        }

        public void WaitForString(string systemStatusText, TimeSpan timeSpan)
        {
            var startTime = DateTime.Now;
            while (true)
            {
                if ((DateTime.Now - startTime) > timeSpan)
                    throw new Exception("Timed out waiting for string");

                if (LastStatusText == null)
                    continue;

                if (LastStatusText.Contains(systemStatusText))
                    return;
            }
        }
    }
}