using System;
using System.Linq;
using Busybody.Events;

namespace Busybody
{
    public class HostEventHandler : IHandle<HostTestResultEvent>
    {
        static readonly Logger _log = new Logger(typeof(HostEventHandler));

        public void Handle(HostTestResultEvent @event)
        {
            _log.Trace("HostTestResultEvent received for host:" + @event.HostNickname + ", result:" + @event.TestResult);
            var hostRepository = AppContext.Instance.HostRepository;
            var hostConfig = AppContext.Instance.Config.Hosts.Single(x => x.Nickname == @event.HostNickname);
            var host = hostRepository.GetOrCreateHost(hostConfig);
            host.HandleTestResult(@event);
            hostRepository.UpdateHost(host);
        }
    }
}