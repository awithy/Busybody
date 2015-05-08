using System.Collections.Concurrent;
using System.Collections.Generic;
using Busybody.Events;
using Busybody.WebServer;

namespace Busybody
{
    public class EventLogRepository : 
        IHandle<HostStateEvent>,
        IHandle<SystemErrorEvent>,
        IHandle<EmailAlertSentEvent>,
        IHandle<BusybodyStartedEvent>
    {
        static BlockingCollection<EventModel> _events = new BlockingCollection<EventModel>(new ConcurrentQueue<EventModel>());

        public void Handle(HostStateEvent @event)
        {
            if (@event.State == HostState.DOWN)
            {
                _AddDanger(@event);
            }
            else
            {
                _Add(@event);
            }
        }

        public void Handle(SystemErrorEvent @event)
        {
            _AddDanger(@event);
        }

        public void Handle(EmailAlertSentEvent @event)
        {
            _Add(@event);
        }
        public void Handle(BusybodyStartedEvent @event)
        {
            _Add(@event);
        }

        void _AddDanger(BusybodyEvent @event)
        {
            _events.Add(new EventModel
            {
                Timestamp = @event.Timestamp.ToString("o"),
                EventType = @event.GetType().Name,
                EventMessage = @event.ToLogString(),
                IsDanger = true,
            });

            _ClearOldEvents();
        }

        static void _Add(BusybodyEvent @event)
        {
            _events.Add(new EventModel
            {
                Timestamp = @event.Timestamp.ToString("o"),
                EventType = @event.GetType().Name,
                EventMessage = @event.ToLogString(),
            });

            _ClearOldEvents();
        }


        public IEnumerable<EventModel> GetEvents()
        {
            return _events.ToArray();
        }

        static void _ClearOldEvents()
        {
            var count = _events.Count;
            for (var i = 0; i < count - 500; i++)
            {
                _events.Take();
            }
        }

        public void ClearEvents()
        {
            _events = new BlockingCollection<EventModel>();
        }
    }
}