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
            _stopFlag = true;
            _stoppedEvent.Set();
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
            _startedEvent.Set();
            _Sleep();

            while (true)
            {
                _RunHostTests();
                _Sleep();
            }
        }

        void _Sleep()
        {
            if (_stopFlag)
                return;
            for (int i = 0; i < 10 * 60 * 2; i++)
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
                    test.Execute(host, testConfig);

                    if (allPassed)
                        _eventLogger.Publish("Host: " + host.Nickname + ", State: Up");
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