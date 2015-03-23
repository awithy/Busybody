using System;
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
            foreach (var hostConfig in config.Hosts)
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
