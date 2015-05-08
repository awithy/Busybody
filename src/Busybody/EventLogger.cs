using System;
using System.IO;
using Busybody.Events;

namespace Busybody
{
    public interface IEventLogger :
        IHandle<HostStateEvent>,
        IHandle<BusybodyStartedEvent>,
        IHandle<EmailAlertSentEvent>
    {
    }

    public class EventLogger : IEventLogger 
    {
        static readonly Logger _log = new Logger(typeof (EventLogger));
        static readonly object _syncLock = new object();

        void Publish(string eventText)
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

        public void Handle(HostStateEvent @event)
        {
            _Handle(@event);
        }

        public void Handle(BusybodyStartedEvent @event)
        {
            _Handle(@event);
        }

        public void Handle(EmailAlertSentEvent @event)
        {
            _Handle(@event);
        }

        void _Handle(BusybodyEvent @event)
        {
            Publish(@event.ToLogString());
        }
    }
}