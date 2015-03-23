using System;
using System.Threading;
using Busybody.Config;
using Busybody.Events;

namespace Busybody
{
    public class BusybodyDaemon
    {
        readonly Logger _log = new Logger(typeof(BusybodyDaemon));
        readonly ManualResetEvent _stoppedEvent = new ManualResetEvent(false);
        readonly HostTestRunner _hostTestRunner = new HostTestRunner();
        bool _stopFlag;
        bool _stopped;

        public void Start()
        {
            _SubscribeTextEventLogger();

            _StartMonitoring();

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
                while (!_stopFlag)
                {
                    _RunHostTestsAndCatch();
                    AppContext.Instance.EventBus.DispatchPending();
                    _Sleep();
                }
                _stopped = true;
            }
            catch (Exception ex)
            {
                _log.Critical("Monitoring thread experienced critical exception", ex);
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

        void _RunHostTestsAndCatch()
        {
            try
            {
                _RunHostTests();
            }
            catch (Exception ex)
            {
                _log.ErrorFormat(ex, "Exception of type {0} thrown while running host tests.", ex.GetType().Name);
                throw;
            }
        }

        void _RunHostTests()
        {
            _hostTestRunner.RunHostTests();
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
    }

    public class TimedOutWaitingForStartException : Exception
    {
    }

    public class TestNotFoundException : Exception
    {
        public TestNotFoundException(string name) : base(string.Format("Test {0} not found.", name))
        {
        }
   }
}