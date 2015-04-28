using System;
using System.Collections.Generic;
using System.Linq;
using Busybody.Config;
using Busybody.Events;

namespace Busybody
{
    public class Host
    {
        public string Id { get; private set; }
        public string Name { get; set; }
        public HostState State { get; set; }
        public Dictionary<string,HostTest> Tests { get; set; }
        public bool IsInitialState { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime LastStateChange { get; set; }
        public string Location { get; set; }

        public Host(HostConfig hostConfig)
        {
            Id = Guid.NewGuid().ToString("N");
            Name = hostConfig.Nickname;
            Location = hostConfig.Location;
            Tests = new Dictionary<string, HostTest>();
            foreach (var test in hostConfig.Tests)
                Tests.Add(test.Name, new HostTest(test));
            IsInitialState = true;
        }

        public void HandleTestResult(HostTestResultEvent @event)
        {
            var hostTest = Tests[@event.TestName];
            hostTest.HandleResult(@event);

            var newState = Tests.Values.Any(x => x.State == HostTestState.Fail)
                ? HostState.DOWN
                : HostState.UP;

            if (newState != State)
            {
                State = newState;
                var hostStateEvent = new HostStateEvent(Name, State, IsInitialState);
                AppContext.Instance.EventBus.Publish("All", hostStateEvent);
                IsInitialState = false;
                LastStateChange = @event.Timestamp;
            }

            LastUpdate = @event.Timestamp;
        }
    }

    public class HostTest
    {
        public string Name { get; set; }
        public int NumberOfFailures { get; set; }
        public int AllowableFailures { get; set; }
        public HostTestState State { get; set; }

        public HostTest(HostTestConfig test)
        {
            Name = test.Name;
            AllowableFailures = test.AllowableFailures;
        }

        public void HandleResult(HostTestResultEvent @event)
        {
            if (@event.TestResult)
            {
                NumberOfFailures = 0;
                State = HostTestState.Pass;
            }
            else
            {
                NumberOfFailures++;
                if (NumberOfFailures > AllowableFailures)
                    State = HostTestState.Fail;
            }
        }
    }

    public enum HostTestState
    {
        Unknown,
        Pass,
        Fail,
    }
}