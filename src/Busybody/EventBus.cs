using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace Busybody
{
    public interface IEventBus
    {
        void Publish(string eventStreamName, BusybodyEvent @event);
        void Subscribe(EventSubscription subscription);
        void DispatchPending();
        void RegisterHandler(string streamName, Type handlerType);
    }

    public class EventBus : IEventBus
    {
        static Logger _log = new Logger(typeof (EventBus));
        static object _pendingSyncLock = new object(); 
        Dictionary<string, List<BusybodyEvent>> _pendingEvents = new Dictionary<string, List<BusybodyEvent>>(); 

        static object _subscriptionSyncLock = new object(); 
        Dictionary<string, EventSubscription> _subscriptionsByName = new Dictionary<string, EventSubscription>();

        ConcurrentDictionary<string, List<HandlerRegistration>> _handlerRegistrations = new ConcurrentDictionary<string, List<HandlerRegistration>>();

        static object _dispatchLock = new object();

        public void Publish(string eventStreamName, BusybodyEvent @event)
        {
            _log.Trace("Publishing event " + @event);
            lock (_pendingSyncLock)
            {
                if (!_pendingEvents.ContainsKey(eventStreamName))
                    _pendingEvents.Add(eventStreamName, new List<BusybodyEvent>());
                _pendingEvents[eventStreamName].Add(@event);
            }
        }

        public void Subscribe(EventSubscription subscription)
        {
            _log.Trace("Subscribe to event " + subscription.Name);
            lock (_subscriptionSyncLock)
            {
                if (_subscriptionsByName.ContainsKey(subscription.Name))
                    _log.Warn("Subscription " + subscription.Name + " subscribed already.");
                _subscriptionsByName.Add(subscription.Name, subscription);
            }
        }

        public void DispatchPending()
        {
            _log.Trace("Dispatching events");

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
            }

            lock (_dispatchLock)
            {
                foreach (var streamName in pendingCopy.Keys)
                {
                    if (_handlerRegistrations.ContainsKey(streamName))
                    {
                        var handlers = _handlerRegistrations[streamName];

                        if (!pendingCopy.ContainsKey(streamName))
                            continue;
                        var eventsInStream1 = pendingCopy[streamName];
                        foreach (var @event in eventsInStream1)
                        {
                            var handlerMethods = handlers.Select(x =>
                                new {Type = x.HandlerType, Method = x.HandlerType.GetMethod("Handle", new[] {@event.GetType()})})
                                .Where(x => x.Method != null)
                                .ToArray();

                            foreach (var handlerMethod in handlerMethods)
                            {
                                var instance = Activator.CreateInstance(handlerMethod.Type);
                                try
                                {
                                    handlerMethod.Method.Invoke(instance, new object[] {@event});
                                }
                                catch (Exception ex)
                                {
                                    _log.ErrorFormat(ex, "Error handling event of type " + @event.GetType().Name + " by event handler " + handlerMethod.Type.Name);
                                    Thread.Sleep(TimeSpan.FromMinutes(5));
                                }
                            }
                        }
                    }

                    var subscriptionsToStream = subscriptionsCopy.Values.Where(x => x.EventStreamName == streamName);
                    foreach (var subscription in subscriptionsToStream)
                    {
                        if (!pendingCopy.ContainsKey(streamName))
                            continue;
                        var eventsInStream = pendingCopy[streamName];
                        foreach (var @event in eventsInStream)
                        {
                            subscription.Recipient.Invoke(new EventNotification{ Event = @event });
                        }
                    }
                }
            }

            _log.Trace("Dispatching events complete");
        }

        public void RegisterHandler(string streamName, Type handlerType)
        {
            var registration = new HandlerRegistration
            {
                StreamName = streamName,
                HandlerType = handlerType,
            };

            _handlerRegistrations.AddOrUpdate(streamName, 
                _ => new List<HandlerRegistration>() {registration}, 
                (n, l) =>
                {
                    l.Add(registration);
                    return l;
                });
        }
    }

    public class StreamNotFoundException : Exception
    {
        public StreamNotFoundException(string streamName) : base("Could not find stream " + streamName)
        {
        }
    }

    public class HandlerRegistration
    {
        public string StreamName { get; set; }
        public Type HandlerType { get; set; }
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
        public virtual string ToLogString()
        {
            return "<Not Used>";
        }
    }
}