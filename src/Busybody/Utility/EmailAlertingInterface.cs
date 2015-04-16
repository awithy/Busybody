using System.Collections.Generic;

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
            IEnumerable<string> toAddresses = new []{emailConfig.ToEmailAddress};
            var email = new Email(toAddresses, emailAlert.Subject, emailAlert.Body);
            _log.InfoFormat("Sending e-mail alert to:{0} with subject:{1}",  emailConfig.ToEmailAddress, emailAlert.Subject);
            emailClient.Send(email);
        }
    }

    public class EmailAlert
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}