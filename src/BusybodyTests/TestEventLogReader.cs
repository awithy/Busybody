using System;
using System.IO;
using System.Threading;
using Busybody;
using NUnit.Framework;

namespace BusybodyTests
{
    public class TestEventLogReader
    {
        string _eventLogFilePath;

        public TestEventLogReader()
        {
            _eventLogFilePath = CommonPaths.EventLogFilePath();
        }

        public void ClearEventLog()
        {
            if(File.Exists(_eventLogFilePath))
                File.Delete(_eventLogFilePath);
        }

        //This crap is just tempoary until I do a proper event store or alerting
        public bool WaitForEvent(string text)
        {
            //Thread.Sleep(30000);
            var startTime = DateTime.Now;
            while (true)
            {
                if(File.Exists(_eventLogFilePath))
                {
                    var allText = File.ReadAllText(_eventLogFilePath);
                    var containsText = allText.Contains(text);
                    if (containsText)
                        return true;
                }
                var timeSinceStart = DateTime.Now - startTime;
                if (timeSinceStart > TimeSpan.FromSeconds(5))
                    return false;
                Thread.Sleep(500);
            }
        }
    }
}