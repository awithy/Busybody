using System;
using System.Collections.Generic;
using System.Linq;

namespace Busybody
{
    public class EventBus
    {
        static Logger _log = new Logger(typeof (EventBus));
        static object _pendingSyncLock = new object(); 
        Dictionary<string, List<BusybodyEvent>> _pendingEvents = new Dictionary<string, List<BusybodyEvent>>(); 

        static object _subscriptionSyncLock = new object(); 
        Dictionary<string, EventSubscription> _subscriptionsByName = new Dictionary<string, EventSubscription>();

        static object _dispatchLock = new object();

        public void Publish(string eventStreamName, BusybodyEvent @event)
        {
            _log.Debug("Publishing event " + @event);
            lock (_pendingSyncLock)
            {
                if (!_pendingEvents.ContainsKey(eventStreamName))
                    _pendingEvents.Add(eventStreamName, new List<BusybodyEvent>());
                _pendingEvents[eventStreamName].Add(@event);
            }
        }

        public void Subscribe(EventSubscription subscription)
        {
            _log.Debug("Subscribe to event " + subscription.Name);
            lock (_subscriptionSyncLock)
            {
                if (_subscriptionsByName.ContainsKey(subscription.Name))
                    _log.Warn("Subscription " + subscription.Name + " subscribed already.");
                _subscriptionsByName.Add(subscription.Name, subscription);
            }
        }

        public void DispatchPending()
        {
            _log.Debug("Dispatching events");

            Dictionary<string, List<BusybodyEvent>> pendingCopy;
            lock (_pendingSyncLock)
            {
                pendingCopy = _pendingEvents;
                _pendingEvents = new Dictionary<string, List<BusybodyEvent>>();
            }

            Dictionary<string, EventSubscription> subscriptionsCopy;
            lock (_subscriptionSyncLock)
            {
                subscriptionsCopy = _subscriptionsByName;
                _subscriptionsByName = new Dictionary<string, EventSubscription>();
            }

            lock (_dispatchLock)
            {
                foreach (var stream in pendingCopy.Keys)
                {
                    var subscriptionsToStream = subscriptionsCopy.Values.Where(x => x.EventStreamName == stream);
                    foreach (var subscription in subscriptionsToStream)
                    {
                        if (!pendingCopy.ContainsKey(stream))
                            continue;
                        var eventsInStream = pendingCopy[stream];
                        foreach (var @event in eventsInStream)
                        {
                            subscription.Recipient.Invoke(new EventNotification{ Event = @event });
                        }
                    }
                }
            }

            _log.Debug("Dispatching events complete");
        }
    }

    public class EventSubscription
    {
        public string Name { get; set; }
        public string EventStreamName { get; set; }
        public Action<EventNotification> Recipient { get; set; }
    }

    public class EventNotification
    {
        public BusybodyEvent Event { get; set; }
    }

    public class BusybodyEvent
    {
    }
}