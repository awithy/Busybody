using System;
using System.Collections.Generic;
using System.Text;
using Busybody.Config;

namespace Busybody
{
    public interface IEmailAlertingInterface
    {
        void Alert(EmailAlert emailAlert);
    }

    public class EmailAlertingInterface : IEmailAlertingInterface
    {
        readonly Logger _log = new Logger(typeof (EmailAlertingInterface));
        static DateTime LastSend = DateTime.MinValue;
        public void Alert(EmailAlert emailAlert)
        {
            if (AppContext.Instance.Config.EmailAlertConfiguration == null)
            {
                _log.Debug("E-Mail alerting not configured.");
                return;
            }
            _log.DebugFormat("Alerting email subject:" + emailAlert.Subject);
            if ((DateTime.Now - LastSend) < TimeSpan.FromMinutes(5)) //TODO: change to messages sent in last 15
                return;
            LastSend = DateTime.Now;

            var emailConfig = AppContext.Instance.Config.EmailAlertConfiguration;
            emailConfig.Host = emailConfig.Host;
            emailConfig.FromAddress = emailConfig.FromAddress;
            emailConfig.Port = emailConfig.Port;
            emailConfig.Password = emailConfig.Password;
            var emailClient = new EmailClient(emailConfig);
            IEnumerable<string> toAddresses = new []{emailConfig.ToEmailAddress};
            var email = new Email(toAddresses, emailAlert.Subject, emailAlert.Body);
            emailClient.Send(email);
        }
    }

    public class EmailAlert
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}