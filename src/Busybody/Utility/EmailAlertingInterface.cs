using System.Collections.Generic;
using Busybody.Events;

namespace Busybody.Utility
{
    public interface IEmailAlertingInterface
    {
        void Alert(EmailAlert emailAlert);
    }

    public class EmailAlertingInterface : IEmailAlertingInterface
    {
        readonly Logger _log = new Logger(typeof (EmailAlertingInterface));
        public void Alert(EmailAlert emailAlert)
        {
            _log.TraceFormat("Sending e-mail alert with subject:{0}",  emailAlert.Subject);
            var emailConfig = AppContext.Instance.Config.EmailAlertConfiguration;
            emailConfig.Host = emailConfig.Host;
            emailConfig.FromAddress = emailConfig.FromAddress;
            emailConfig.Port = emailConfig.Port;
            emailConfig.Password = emailConfig.Password;
            var emailClient = new EmailClient(emailConfig);
            IEnumerable<string> toAddresses = new []{emailConfig.ToAddress};
            var email = new Email(toAddresses, emailAlert.Subject, emailAlert.Body);
            _log.InfoFormat("Sending e-mail alert to:{0} with subject:{1}",  emailConfig.ToAddress, emailAlert.Subject);
            emailClient.Send(email);
            AppContext.Instance.EventBus.Publish("All", new EmailAlertSentEvent(emailConfig.ToAddress, emailAlert.Subject));
        }
    }

    public class EmailAlert
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}