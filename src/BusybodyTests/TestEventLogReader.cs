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

        //This crap is just tempoary until I do this properly
        public void WaitForEvent(string text)
        {
            var startTime = DateTime.Now;
            while (true)
            {
                if(File.Exists(_eventLogFilePath))
                {
                    var allText = File.ReadAllText(_eventLogFilePath);
                    var containsText = allText.Contains(text);
                    if (containsText)
                        return;
                }
                var timeSinceStart = DateTime.Now - startTime;
                if (timeSinceStart > TimeSpan.FromSeconds(5))
                    Assert.Fail("Failed waiting for <<" + text + ">>");
                Thread.Sleep(500);
            }
        }
    }
}