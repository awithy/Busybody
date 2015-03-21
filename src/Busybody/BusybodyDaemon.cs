using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Busybody.Config;

namespace Busybody
{
    public class HostRepository
    {
        public ConcurrentDictionary<string, Host> Hosts = new ConcurrentDictionary<string, Host>();

        public Host GetOrCreateHost(string name)
        {
            return Hosts.AddOrUpdate(name, n => new Host {Name = n}, (n, existingHost) => existingHost);
        }

        public void UpdateHost(Host host)
        {
            Hosts.AddOrUpdate(host.Name, n => host, (n, existingHost) => host);
        }
    }

    public class Host
    {
        public string Name { get; set; }
        public HostState State { get; set; }
    }

    public class BusybodyDaemon
    {
        static Logger _log = new Logger(typeof(BusybodyDaemon));
        BusybodyConfig _config;
        HostRepository _hostRepository = new HostRepository();
        readonly ManualResetEvent _startedEvent = new ManualResetEvent(false);
        readonly ManualResetEvent _stoppedEvent = new ManualResetEvent(false);
        bool _stopFlag;
        bool _stopped;

        public void Start()
        {
            _config = AppContext.Instance.Config;

            _SubscribeTextEventLogger();

            _StartMonitoring();

            _WaitForStart();

            AppContext.Instance.EventBus.Publish("All", new StartupCompleteEvent());
            _log.Info("Busybody started");
        }

        public void Stop()
        {
            _log.Info("Stopping");

            _stopFlag = true;
            while (!_stopped)
                Thread.Sleep(100);
            _stoppedEvent.Set();

            _log.Info("Stopped");
        }

        void _StartMonitoring()
        {
            _log.Trace("Starting Monitoring");
            var threadStart = new ThreadStart(_StartMonitoringThreadStart);
            var thread = new Thread(threadStart)
            {
                IsBackground = true
            };
            thread.Start();
            _log.Trace("Monitoring thread started");
        }

        void _StartMonitoringThreadStart()
        {
            _log.Trace("Monitoring thread entered");
            try
            {
                _RunHostTestsAndSwallow();
                AppContext.Instance.EventBus.DispatchPending();
                _startedEvent.Set();
                _Sleep();

                while (!_stopFlag)
                {
                    _RunHostTestsAndSwallow();
                    AppContext.Instance.EventBus.DispatchPending();
                    _Sleep();
                }
                _stopped = true;
            }
            catch (Exception ex)
            {
                _log.Critical("Monitoring thread experienced exception: " + ex);
                throw;
            }
        }

        void _Sleep()
        {
            if (_stopFlag)
                return;
            var pollingInterval = AppContext.Instance.Config.PollingInterval;
            for (var i = 0; i < 10 * pollingInterval; i++)
            {
                AppContext.Instance.Threading.Sleep(100);
                if (_stopFlag)
                    return;
            }
        }

        void _RunHostTestsAndSwallow()
        {
            try
            {
                _RunHostTests();
            }
            catch (Exception ex)
            {
                _log.Error("Error running host tests " + ex);
                //Intentional swallow
            }
        }

        void _RunHostTests()
        {
            _log.Trace("Running host test");
            foreach (var hostConfig in _config.Hosts)
            {
                _log.Debug("Checking host " + hostConfig.Nickname);
                var host = _hostRepository.GetOrCreateHost(hostConfig.Nickname);
                var allPassed = true;
                foreach (var testConfig in hostConfig.Tests)
                {
                    _log.Trace("Running test " + testConfig.Name);
                    var test = AppContext.Instance.TestFactory.Create(testConfig.Name);
                    allPassed = allPassed & test.Execute(hostConfig, testConfig);

                    var hostState = allPassed ? HostState.UP : HostState.DOWN;
                    if (hostState != host.State)
                    {
                        host.State = hostState;
                        _PublishHostStateEvent(hostConfig, hostState);
                        _hostRepository.UpdateHost(host);
                    }
                }
            }
            _log.Trace("Test run complete");
        }

        static void _PublishHostStateEvent(HostConfig host, HostState hostState)
        {
            AppContext.Instance.EventBus.Publish("All", new HostStateEvent(host.Nickname, hostState));
        }

        void _SubscribeTextEventLogger()
        {
            var eventSubscription = new EventSubscription
            {
                EventStreamName = "All",
                Name = "Event Logger",
                Recipient = e => AppContext.Instance.EventLogger.Publish(e.Event.ToLogString()),
            };
            AppContext.Instance.EventBus.Subscribe(eventSubscription);
        }

        void _WaitForStart()
        {
            var result = _startedEvent.WaitOne(TimeSpan.FromMinutes(5));
            if (!result)
                throw new TimedOutWaitingForStartException();
        }
    }

    public class TimedOutWaitingForStartException : Exception
    {
    }

    public class TestNotFoundException : Exception
    {
        public TestNotFoundException(string name) : base("Test " + name + " not found.")
        {
        }
   }
}