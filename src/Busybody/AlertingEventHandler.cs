using System;
using System.Text;
using Busybody.Events;

namespace Busybody
{
    public class AlertingEventHandler : IHandle<HostStateEvent>, IHandle<SystemErrorEvent>
    {
        readonly Logger _log = new Logger(typeof(AlertingEventHandler));
        DateTime _lastHostAlert = DateTime.MinValue;
        DateTime _lastSystemErrorAlert = DateTime.MinValue;

        public void Handle(HostStateEvent @event)
        {
            _log.Debug("Handling HostStateEvent");
            if (!_ValidEmailConfig())
            {
                _log.Debug("Invalid alerting e-mail configuration");
                return;
            }
            if ((DateTime.Now - _lastHostAlert) < TimeSpan.FromMinutes(5))
            {
                _log.Debug("Throttling alerts to no more than 1 every 5 minutes");
                return;
            }
            _lastHostAlert = DateTime.Now;

            var emailInterface = AppContext.Instance.EmailAlertingInterface;
            var subject = string.Format("BB ALERT: {0}:{1}", @event.HostNickname, @event.State);
            var message = string.Format("Host {0} state changed.  New state:{1}", @event.HostNickname, @event.State);
            emailInterface.Alert(new EmailAlert {Subject = subject, Body = message});
        }

        public void Handle(SystemErrorEvent @event)
        {
            _log.Debug("Handling SystemErrorEvent");
            if (!_ValidEmailConfig())
            {
                _log.Debug("Invalid alerting e-mail configuration");
                return;
            }
            if ((DateTime.Now - _lastSystemErrorAlert) < TimeSpan.FromMinutes(5))
            {
                _log.Debug("Throttling alerts to no more than 1 every 5 minutes");
                return;
            }

            _lastSystemErrorAlert = DateTime.Now;

            var emailInterface = AppContext.Instance.EmailAlertingInterface;
            var subject = "BB ALERT: System Error";
            var sb = new StringBuilder();
            sb.AppendLine("BB ALERT: System Error");
            sb.AppendLine();
            sb.AppendLine("Message: " + @event.Message);
            sb.AppendLine();
            sb.AppendLine("Detail: " + @event.Detail);
            emailInterface.Alert(new EmailAlert {Subject = subject, Body = sb.ToString()});
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