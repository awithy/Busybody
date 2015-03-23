using System;
using System.IO;

namespace Busybody
{
    public interface IEventLogger
    {
        void Publish(string eventText);
    }

    public class EventLogger : IEventLogger
    {
        static readonly Logger _log = new Logger(typeof (EventLogger));
        static readonly object _syncLock = new object();

        public void Publish(string eventText)
        {
            var eventLogFilePath = CommonPaths.EventLogFilePath();
            var dateTimeString = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var formattedEvent = "[" + dateTimeString + "] " + eventText;
            _log.Trace("Logging event: " + formattedEvent);

            lock(_syncLock)
                using (var streamWriter = File.AppendText(eventLogFilePath))
                {
                    streamWriter.WriteLine(formattedEvent);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
        }
    }
}