using System;
using Busybody.Events;

namespace Busybody
{
    public class AlertingEventHandler : IHandle<HostStateEvent>
    {
        readonly Logger _log = new Logger(typeof(AlertingEventHandler));
        DateTime _lastAlert = DateTime.MinValue;

        public void Handle(HostStateEvent @event)
        {
            if (!_ValidEmailConfig())
            {
                _log.Debug("Invalid alerting e-mail configuration");
                return;
            }
            if ((DateTime.Now - _lastAlert) < TimeSpan.FromMinutes(5))
            {
                _log.Debug("Throttling alerts to no more than 1 every 5 minutes");
                return;
            }
            _lastAlert = DateTime.Now;

            var emailInterface = AppContext.Instance.EmailAlertingInterface;
            var subject = string.Format("BB ALERT: {0}:{1}", @event.HostNickname, @event.State);
            var message = string.Format("Host {0} state changed.  New state:{1}", @event.HostNickname, @event.State);
            emailInterface.Alert(new EmailAlert {Subject = subject, Body = message});
        }

        bool _ValidEmailConfig()
        {
            if (AppContext.Instance.Config.EmailAlertConfiguration == null)
                return false;
            var config = AppContext.Instance.Config.EmailAlertConfiguration;
            if (string.IsNullOrEmpty(config.Host))
                return false;
            if (string.IsNullOrEmpty(config.FromAddress))
                return false;
            if (string.IsNullOrEmpty(config.ToEmailAddress))
                return false;
            if (string.IsNullOrEmpty(config.Password))
                return false;
            if (config.Port == 0)
                return false;
            return true;
        }
    }
}