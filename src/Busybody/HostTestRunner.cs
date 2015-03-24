using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Busybody.Config;
using Busybody.Events;

namespace Busybody
{
    public class HostTestRunner
    {
        readonly Logger _log = new Logger(typeof(HostTestRunner));
        readonly HostRepository _hostRepository = new HostRepository();

        public void RunHostTests()
        {
            _log.Trace("Running host test");
            var config = AppContext.Instance.Config;

            var hostTestCollection = config.Hosts.SelectMany(hostConfig => hostConfig.Tests.Select(testConfig => new { HostConfig = hostConfig, TestConfig = testConfig }));
            _log.TraceFormat("{0} tests to run", hostTestCollection.Count());
            var hostResults = new ConcurrentDictionary<string, bool>();

            Parallel.ForEach(hostTestCollection, new ParallelOptions {MaxDegreeOfParallelism = 5}, hostTest =>
            {
                _log.TraceFormat("Running test {0} on host {1}", hostTest.HostConfig.Nickname, hostTest.TestConfig.Name);
                var test = AppContext.Instance.TestFactory.Create(hostTest.TestConfig.Name);
                var testResult = _ExecuteTestWithoutThrowing(test, hostTest.HostConfig, hostTest.TestConfig);
                var hostCombinedResult = hostResults.GetOrAdd(hostTest.HostConfig.Nickname, name => true);
                hostCombinedResult = hostCombinedResult && testResult;
                hostResults.TryUpdate(hostTest.HostConfig.Nickname, hostCombinedResult, false);

                var hostState = hostCombinedResult ? HostState.UP : HostState.DOWN;
                var host = _hostRepository.GetOrCreateHost(hostTest.HostConfig.Nickname);
                if (hostState != host.State)
                {
                    host.State = hostState;
                    _log.DebugFormat("Host <{0}> state changed. New state:{1}", hostTest.HostConfig.Nickname, hostState);
                    _PublishHostStateEvent(hostTest.HostConfig, hostState);
                    _SendMailAlertIfNeeded(host);
                    _hostRepository.UpdateHost(host);
                }
            });
            _log.Trace("Test run complete");
        }

        void _SendMailAlertIfNeeded(Host host)
        {
            var emailInterface = AppContext.Instance.EmailAlertingInterface;
            var subject = string.Format("BB ALERT: {0}:{1}", host.Name, host.State);
            var message = string.Format("Host {0} state changed.  New state:{1}", host.Name, host.State);
            emailInterface.Alert(new EmailAlert {Subject = subject, Body = message});
        }

        bool _ExecuteTestWithoutThrowing(IBusybodyTest test, HostConfig hostConfig, HostTestConfig testConfig)
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
    }
}
