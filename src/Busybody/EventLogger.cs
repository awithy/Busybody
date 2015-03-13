using System;
using System.IO;

namespace Busybody
{
    public class EventLogger
    {
        static readonly object _syncLock = new object();

        public void Publish(string eventText)
        {
            var eventLogFilePath = CommonPaths.EventLogFilePath();
            lock(_syncLock)
                using (var streamWriter = File.AppendText(eventLogFilePath))
                {
                    var dateTimeString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    streamWriter.WriteLine("[" + dateTimeString + "] " + eventText);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
        }
    }
}