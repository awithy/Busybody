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

            Parallel.ForEach(hostTestCollection, new ParallelOptions {MaxDegreeOfParallelism = 5}, hostTest =>
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                _log.TraceFormat("Running test {0} on host {1}", hostTest.HostConfig.Nickname, hostTest.TestConfig.Name);
                var test = AppContext.Instance.TestFactory.Create(hostTest.TestConfig.Name);
                var testResult = _ExecuteTestErrorOnException(test, hostTest.HostConfig, hostTest.TestConfig);
                _PublishHostTestResult(hostTest.HostConfig.Nickname, hostTest.TestConfig.Name, testResult);
            });
            _log.Trace("Test run complete");
        }

        void _PublishHostTestResult(string hostNickname, string testName, bool testResult)
        {
            var @event = new HostTestResultEvent
            {
                HostNickname = hostNickname,
                TestName = testName,
                TestResult = testResult,
            };
            AppContext.Instance.EventBus.Publish("All", @event);
        }

        bool _ExecuteTestErrorOnException(IBusybodyTest test, HostConfig hostConfig, HostTestConfig testConfig)
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
    }

    public class TestNotFoundException : Exception
    {
        public TestNotFoundException(string name) : base(string.Format("Test {0} not found.", name))
        {
        }
   }
}
