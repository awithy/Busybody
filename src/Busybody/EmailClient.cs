using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Busybody.Config;

namespace Busybody
{
    public class EmailClient
    {
        readonly EmailAlertConfiguration _configuration;
        readonly Logger _logger;

        public EmailClient(EmailAlertConfiguration emailAlertConfiguration)
        {
            _logger = new Logger(typeof(EmailClient));
            _configuration = emailAlertConfiguration;
        }

        public void Send(Email email)
        {
            var toAddress = email.ToAddresses.First();
            _logger.DebugFormat("Sending error alert email to {0}.", toAddress);
            
            var fromMailAddress = new MailAddress(_configuration.FromAddress, "Busybody");

            var smtp = new SmtpClient
                {
                    Host = _configuration.Host,
                    Port = _configuration.Port,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromMailAddress.Address, _configuration.Password)
                };

            using (var message = new MailMessage { Subject = email.Subject, Body = email.Body })
            {
                message.From = fromMailAddress;
                foreach (var address in email.ToAddresses)
                    message.To.Add(new MailAddress(address));
                try
                {
                    smtp.Send(message);
                    _logger.DebugFormat("Error alert email sent to {0}.", toAddress);
                }
                catch (Exception ex)
                {
                    new ErrorHandler().Error(ex, "Failed to send error alert email to {0}.", toAddress);
                }
            }
        }
    }

    public class Email
    {
        public IEnumerable<string> ToAddresses;
        public string Subject;
        public string Body;

        public Email(IEnumerable<string> toAddresses, string subject, string body)
        {
            ToAddresses = toAddresses;
            Subject = subject;
            Body = body;
        }
    }
}