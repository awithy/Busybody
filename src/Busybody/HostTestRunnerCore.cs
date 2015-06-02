using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Busybody.Config;
using BusybodyShared;

namespace Busybody
{
    public class HostTestRunnerCore
    {
        readonly Logger _log = new Logger(typeof (HostTestRunnerCore));

        public void RunHostTests(CancellationToken cancellationToken)
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
                var testResult = _ExecuteTestErrorOnExceptionOrTimeout(test, hostTest.HostConfig, hostTest.TestConfig);
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

        bool _ExecuteTestErrorOnExceptionOrTimeout(IBusybodyTest test, HostConfig hostConfig, HostTestConfig testConfig)
        {
            try
            {
                var result = false;
                var thread = new Thread(() => result = _ExecuteTestErrorOnException(test, hostConfig, testConfig));
                thread.Start();
                var timedOut = thread.Join(TimeSpan.FromMinutes(5));
                if (!timedOut)
                    throw new HostTestTimedOutException(testConfig.Name);
                return result;
            }
            catch (HostTestTimedOutException ex)
            {
                new ErrorHandler().Error(ex, string.Format("Exception of type {0} thrown during test execution", ex.GetType().Name));
                return false;
            }
        }

        bool _ExecuteTestErrorOnException(IBusybodyTest test, HostConfig hostConfig, HostTestConfig testConfig)
        {
            try
            {
                return test.Execute(hostConfig, testConfig);
            }
            catch (Exception ex)
            {
                new ErrorHandler().Error(ex, string.Format("Exception of type {0} thrown during test execution", ex.GetType().Name));
                return false;
            }
        }
    }

    internal class HostTestTimedOutException : Exception
    {
        public HostTestTimedOutException(string name) : base("Host test " + name + " timed out while executing and was aborted.")
        {
        }
    }

    public class TestNotFoundException : Exception
    {
        public TestNotFoundException(string name) : base(string.Format("Test {0} not found.", name))
        {
        }
   }
}
