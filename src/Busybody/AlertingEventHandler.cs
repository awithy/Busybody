using System;
using System.Collections.Generic;
using System.Text;
using Busybody.Events;

namespace Busybody
{
    public class AlertingEventHandler : IHandle<HostStateEvent>, IHandle<SystemErrorEvent>
    {
        readonly Logger _log = new Logger(typeof(AlertingEventHandler));
        readonly List<string> _downHosts = new List<string>();
        readonly List<DateTime> _recentAlertTimestamps = new List<DateTime>();

        public void Handle(HostStateEvent @event)
        {
            _log.Trace("Handling HostStateEvent");

            if (_downHosts.Contains(@event.HostNickname))
            {
                if (@event.State == HostState.UP)
                    _downHosts.Remove(@event.HostNickname);
            }
            else
            {
                if (@event.State == HostState.UP)
                {
                    _log.Debug("Skipping sending alert for UP host when host not down.");
                    return;
                }
                _downHosts.Add(@event.HostNickname);
            }

            if (!_ValidEmailConfig())
            {
                _log.Debug("Invalid alerting e-mail configuration");
                return;
            }

            if (_ShouldThrottle(@event.Timestamp))
            {
                _log.Debug("Throttling alerts to no more than 1 every 5 minutes");
                return;
            }

            _recentAlertTimestamps.Add(@event.Timestamp);

            var emailInterface = AppContext.Instance.EmailAlertingInterface;
            var subject = string.Format("BB ALERT: {0}:{1}", @event.HostNickname, @event.State);
            var message = string.Format("Host {0} state changed.  New state:{1}", @event.HostNickname, @event.State);
            emailInterface.Alert(new EmailAlert {Subject = subject, Body = message});
        }

        bool _ShouldThrottle(DateTime newestEvent)
        {
            Predicate<DateTime> pred = dt =>
            {
                var minutes = (newestEvent - dt).TotalMinutes;
                return minutes > 30;
            };
            _recentAlertTimestamps.RemoveAll(pred);
            return (_recentAlertTimestamps.Count >= 10);
        }

        public void Handle(SystemErrorEvent @event)
        {
            _log.Debug("Handling SystemErrorEvent");

            if (!_ValidEmailConfig())
            {
                _log.Debug("Invalid alerting e-mail configuration");
                return;
            }

            if (_ShouldThrottle(@event.Timestamp))
            {
                _log.Debug("Throttling alerts to no more than 1 every 5 minutes");
                return;
            }

            _recentAlertTimestamps.Add(@event.Timestamp);

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