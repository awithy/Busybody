using Busybody.Events;

namespace Busybody
{
    public class AlertingEventHandler : IHandle<HostStateEvent>
    {
        public void Handle(HostStateEvent @event)
        {
            var emailInterface = AppContext.Instance.EmailAlertingInterface;
            var subject = string.Format("BB ALERT: {0}:{1}", @event.HostNickname, @event.State);
            var message = string.Format("Host {0} state changed.  New state:{1}", @event.HostNickname, @event.State);
            emailInterface.Alert(new EmailAlert {Subject = subject, Body = message});
        }
    }
}
