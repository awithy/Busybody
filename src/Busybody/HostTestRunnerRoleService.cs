using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Busybody.Config;
using Busybody.Events;

namespace Busybody
{
    public class HostTestRunnerRoleService : PollerBase
    {
        readonly Logger _log = new Logger(typeof(HostTestRunnerRoleService));
        readonly HostRepository _hostRepository = new HostRepository();

        public override string Name
        {
            get { return "Host Test Runner Role Service"; }
        }

        public override TimeSpan Period
        {
            get { return TimeSpan.FromSeconds(AppContext.Instance.Config.PollingInterval); }
        }

        protected override Task _OnPoll(CancellationToken cancellationToken)
        {
            return Task.Run(() => _RunHostTests(cancellationToken), cancellationToken);
        }

        void _RunHostTests(CancellationToken cancellationToken)
        {
            _log.Trace("Running host test");
            if (cancellationToken.IsCancellationRequested)
                return;
            var config = AppContext.Instance.Config;

            var hostTestCollection = config.Hosts.SelectMany(hostConfig => hostConfig.Tests.Select(testConfig => new { HostConfig = hostConfig, TestConfig = testConfig }));
            _log.TraceFormat("{0} tests to run", hostTestCollection.Count());
            var hostResults = new ConcurrentDictionary<string, bool>();

            Parallel.ForEach(hostTestCollection, new ParallelOptions {MaxDegreeOfParallelism = 5}, hostTest =>
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                _log.TraceFormat("Running test {0} on host {1}", hostTest.HostConfig.Nickname, hostTest.TestConfig.Name);
                var test = AppContext.Instance.TestFactory.Create(hostTest.TestConfig.Name);
                var testResult = _ExecuteTestWithoutThrowing(test, hostTest.HostConfig, hostTest.TestConfig);
                var hostCombinedResult = hostResults.GetOrAdd(hostTest.HostConfig.Nickname, name => true);
                hostCombinedResult = hostCombinedResult && testResult;
                hostResults.TryUpdate(hostTest.HostConfig.Nickname, hostCombinedResult, false);

                var hostState = hostCombinedResult ? HostState.UP : HostState.DOWN;
                var host = _hostRepository.GetOrCreateHost(hostTest.HostConfig.Nickname);
                if (cancellationToken.IsCancellationRequested)
                    return;

                if (cancellationToken.IsCancellationRequested)
                    return;

                if (hostState != host.State)
                {
                    host.State = hostState;
                    _log.DebugFormat("Host <{0}> state changed. New state:{1}", hostTest.HostConfig.Nickname, hostState);
                    _PublishHostStateEvent(hostTest.HostConfig, hostState);
                    _hostRepository.UpdateHost(host);
                }
            });
            _log.Trace("Test run complete");
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

    public class TestNotFoundException : Exception
    {
        public TestNotFoundException(string name) : base(string.Format("Test {0} not found.", name))
        {
        }
   }
}
