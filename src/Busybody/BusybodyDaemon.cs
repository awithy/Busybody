using System;
using System.Threading;
using Busybody.Config;

namespace Busybody
{
    public class BusybodyDaemon
    {
        static Logger _log = new Logger(typeof(BusybodyDaemon));
        BusybodyConfig _config;
        EventLogger _eventLogger;
        readonly ManualResetEvent _startedEvent = new ManualResetEvent(false);
        readonly ManualResetEvent _stoppedEvent = new ManualResetEvent(false);
        bool _stopFlag;
        bool _stopped;

        public void Start()
        {
            _config = AppContext.Instance.Config;
            _eventLogger = new EventLogger();

            _SubscribeTextEventLogger();

            _StartMonitoring();

            _WaitForStart();

            _eventLogger.Publish("Startup complete");
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
            _log.Trace("Monitoring thread started");
            _RunHostTests();
            AppContext.Instance.EventBus.DispatchPending();
            _startedEvent.Set();
            _Sleep();

            while (!_stopFlag)
            {
                _RunHostTests();
                AppContext.Instance.EventBus.DispatchPending();
                _Sleep();
            }
            _stopped = true;
        }

        void _Sleep()
        {
            if (_stopFlag)
                return;
            for (var i = 0; i < 10 * 60 * 2; i++)
            {
                AppContext.Instance.Threading.Sleep(100);
                if (_stopFlag)
                    return;
            }
        }

        void _RunHostTests()
        {
            _log.Trace("Running host test");
            foreach (var host in _config.Hosts)
            {
                _log.Debug("Checking host " + host.Nickname);
                var allPassed = true;
                foreach (var testConfig in host.Tests)
                {
                    _log.Trace("Running test " + testConfig.Name);
                    var test = AppContext.Instance.TestFactory.Create(testConfig.Name);
                    allPassed = allPassed & test.Execute(host, testConfig);

                    var hostState = allPassed ? HostState.UP : HostState.DOWN;
                    _PublishHostStateEvent(host, hostState);
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
                Recipient = e => _eventLogger.Publish(e.Event.ToLogString()),
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