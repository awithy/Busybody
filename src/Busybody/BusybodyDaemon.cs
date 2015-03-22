using System;
using System.Threading;
using Busybody.Config;
using Busybody.Events;

namespace Busybody
{
    public class BusybodyDaemon
    {
        static Logger _log = new Logger(typeof(BusybodyDaemon));
        readonly HostRepository _hostRepository = new HostRepository();
        readonly ManualResetEvent _startedEvent = new ManualResetEvent(false);
        readonly ManualResetEvent _stoppedEvent = new ManualResetEvent(false);
        BusybodyConfig _config;
        bool _stopFlag;
        bool _stopped;

        public void Start()
        {
            _config = AppContext.Instance.Config;

            _SubscribeTextEventLogger();

            _StartMonitoring();

            _WaitforStartupToComplete();

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
                _RunHostTestsAndCatch();
                AppContext.Instance.EventBus.DispatchPending();
                _startedEvent.Set();
                _Sleep();

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
            _log.Trace("Running host test");
            foreach (var hostConfig in _config.Hosts)
            {
                _log.DebugFormat("Checking host {0}", hostConfig.Nickname);
                var host = _hostRepository.GetOrCreateHost(hostConfig.Nickname);
                var allPassed = true;
                foreach (var testConfig in hostConfig.Tests)
                {
                    _log.TraceFormat("Running test {0}", testConfig.Name);
                    var test = AppContext.Instance.TestFactory.Create(testConfig.Name);
                    var execute = _ExecuteTestWithoutThrowing(test, hostConfig, testConfig);
                    allPassed = allPassed & execute;

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

        static bool _ExecuteTestWithoutThrowing(IBusybodyTest test, HostConfig hostConfig, HostTestConfig testConfig)
        {
            try
            {
                return test.Execute(hostConfig, testConfig);
            }
            catch (Exception ex)
            {
                _log.ErrorFormat(ex, string.Format("Exception of type {0} thrown during test execution", ex.GetType().Name));
                return false;
            }
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

        void _WaitforStartupToComplete()
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
        public TestNotFoundException(string name) : base(string.Format("Test {0} not found.", name))
        {
        }
   }
}