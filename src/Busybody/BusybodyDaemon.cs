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

            _StartMonitoring();

            _startedEvent.WaitOne(TimeSpan.FromMinutes(5));

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
            var threadStart = new ThreadStart(_StartMonitoringThreadStart);
            var thread = new Thread(threadStart);
            thread.IsBackground = true;
            thread.Start();
        }

        void _StartMonitoringThreadStart()
        {
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
            foreach (var host in _config.Hosts)
            {
                _log.Debug("Checking host " + host.Nickname);
                var allPassed = true;
                foreach (var testConfig in host.Tests)
                {
                    var test = AppContext.Instance.TestFactory.Create(testConfig.Name);
                    allPassed = allPassed & test.Execute(host, testConfig);

                    if (allPassed)
                    {
                        _eventLogger.Publish("Host: " + host.Nickname + ", State: Up");
                        AppContext.Instance.EventBus.Publish("All", new HostStateEvent("Host: " + host.Nickname + ", State: Up"));
                    }
                    else
                    {
                        AppContext.Instance.EventBus.Publish("All", new HostStateEvent("Host: " + host.Nickname + ", State: Down"));
                    }
                }
            }
        }
    }

    public class TestNotFoundException : Exception
    {
        public TestNotFoundException(string name) : base("Test " + name + " not found.")
        {
        }
   }
}