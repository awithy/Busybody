using System;
using System.Collections.Generic;

namespace Busybody
{
    public interface IEmailAlertingInterface
    {
        void Alert(EmailAlert emailAlert);
    }

    public class EmailAlertingInterface : IEmailAlertingInterface
    {
        static DateTime LastSend = DateTime.MinValue;
        public void Alert(EmailAlert emailAlert)
        {
            LastSend = DateTime.Now;
            if ((DateTime.Now - LastSend) < TimeSpan.FromMinutes(5)) //TODO: change to messages sent in last 15
                return;

            var errorAlertEmailConfiguration = new ErrorAlertEmailConfiguration();
            errorAlertEmailConfiguration.Host = "";
            errorAlertEmailConfiguration.FromAddress = "";
            errorAlertEmailConfiguration.Port = 123;
            errorAlertEmailConfiguration.Password = "";
            var emailClient = new EmailClient(errorAlertEmailConfiguration);
            IEnumerable<string> toAddresses = new[] {"awithy@msn.com"};
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