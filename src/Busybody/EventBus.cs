using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Busybody.Events;
using BusybodyShared;

namespace Busybody
{
    public interface IEventBus
    {
        void Publish(string eventStreamName, BusybodyEvent @event);
        void DispatchPending();
        void DispatchPending(CancellationToken cancellationToken);
        void RegisterHandler(string streamName, Type handlerType, InstanceMode instanceMode = InstanceMode.PerCall);
    }

    public class EventBus : IEventBus
    {
        static Logger _log = new Logger(typeof (EventBus));
        static object _pendingSyncLock = new object(); 
        Dictionary<string, List<BusybodyEvent>> _pendingEvents = new Dictionary<string, List<BusybodyEvent>>(); 

        ConcurrentDictionary<string, List<HandlerRegistration>> _handlerRegistrations = new ConcurrentDictionary<string, List<HandlerRegistration>>();
        Dictionary<string, object> _instanceCache = new Dictionary<string, object>();

        static object _dispatchLock = new object();

        public void Publish(string eventStreamName, BusybodyEvent @event)
        {
            _log.Trace("Publishing event " + @event);
            if (@event.Timestamp == DateTime.MinValue)
                @event.Timestamp = DateTime.UtcNow;

            lock (_pendingSyncLock)
            {
                if (!_pendingEvents.ContainsKey(eventStreamName))
                    _pendingEvents.Add(eventStreamName, new List<BusybodyEvent>());
                _pendingEvents[eventStreamName].Add(@event);
            }
        }

        public void DispatchPending()
        {
            DispatchPending(CancellationToken.None);
        }

        //This could use a BIT of clean up
        public void DispatchPending(CancellationToken cancellationToken)
        {
            _log.Trace("Dispatching events");

            Dictionary<string, List<BusybodyEvent>> pendingEvents;
            lock (_pendingSyncLock)
            {
                pendingEvents = _pendingEvents;
                _pendingEvents = new Dictionary<string, List<BusybodyEvent>>();
            }

            lock (_dispatchLock)
            {
                foreach (var streamName in pendingEvents.Keys)
                {
                    if (!_handlerRegistrations.ContainsKey(streamName)) 
                        continue;

                    if (!pendingEvents.ContainsKey(streamName))
                        continue;

                    var handlers = _handlerRegistrations[streamName];

                    var eventsInStream = pendingEvents[streamName];
                    foreach (var @event in eventsInStream)
                    {
                        var handlerMethods = handlers.Select(x =>
                            new {HandlerRegistration = x, Type = x.HandlerType, Method = x.HandlerType.GetMethod("Handle", new[] {@event.GetType()})})
                            .Where(x => x.Method != null)
                            .ToArray();

                        foreach (var handlerMethod in handlerMethods)
                        {
                            var instance = _instanceCache.ContainsKey(handlerMethod.Type.Name)
                                ? _instanceCache[handlerMethod.Type.Name]
                                : Activator.CreateInstance(handlerMethod.Type);

                            if (!_instanceCache.ContainsKey(handlerMethod.Type.Name))
                            {
                                if (handlerMethod.HandlerRegistration.InstanceMode == InstanceMode.Singleton)
                                    _instanceCache.Add(handlerMethod.Type.Name, instance);
                            }

                            var tries = 0;
                            while (true)
                            {
                                try
                                {
                                    handlerMethod.Method.Invoke(instance, new object[] {@event});
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    tries++;
                                    if (tries >= 5)
                                    {
                                        new ErrorHandler().Error(ex, "Error handling event of type " + @event.GetType().Name + " by event handler " + handlerMethod.Type.Name);
                                        break;
                                    }
                                    _log.WarnFormat("Error handling event of type " + @event.GetType().Name + " by event handler " + handlerMethod.Type.Name);
                                    _log.Debug(ex.ToString());
                                    Thread.Sleep(TimeSpan.FromSeconds(15));
                                }
                            }
                        }
                    }
                }
            }

            _log.Trace("Dispatching events complete");
        }

        public void RegisterHandler(string streamName, Type handlerType, InstanceMode instanceMode = InstanceMode.PerCall)
        {
            var registration = new HandlerRegistration
            {
                StreamName = streamName,
                HandlerType = handlerType,
                InstanceMode = instanceMode,
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

    public class HandlerRegistration
    {
        public string StreamName { get; set; }
        public Type HandlerType { get; set; }
        public InstanceMode InstanceMode { get; set; }
    }
}