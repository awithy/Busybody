using Busybody.Events;

namespace Busybody
{
    public class HostEventHandler : IHandle<HostTestResultEvent>
    {
        static Logger _log = new Logger(typeof(HostEventHandler));

        public void Handle(HostTestResultEvent @event)
        {
            _log.Trace("HostTestResultEvent received for host:" + @event.HostNickname + ", result:" + @event.TestResult);
            var hostRepository = AppContext.Instance.HostRepository;
            var isInitialState = !hostRepository.Exists(@event.HostNickname);
            var host = hostRepository.GetOrCreateHost(@event.HostNickname);
            var newState = @event.TestResult ? HostState.UP : HostState.DOWN;
            if (host.State != newState)
            {
                _log.DebugFormat("Host <{0}> state changed. New state:{1}", host.Name, newState);
                host.State = newState;
                hostRepository.UpdateHost(host);
                var hostStateEvent = new HostStateEvent(host.Name, newState, isInitialState);
                AppContext.Instance.EventBus.Publish("All", hostStateEvent);
                _log.Trace("HostStateChanged event published");
            }
        }
    }
}