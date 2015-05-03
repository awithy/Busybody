using System.Web.Http;
using Busybody.Config;

namespace Busybody.WebServer
{
    public class ConfigController : ApiController
    {
        public ConfigModel GetConfig()
        {
            var config = AppContext.Instance.Config;
            var configModel = new ConfigModel
            {
                EmailAlertingConfig = _MapEmailAlertingConfig(config.EmailAlertConfiguration),
            };
            return configModel;
        }

        EmailAlertingConfigModel _MapEmailAlertingConfig(EmailAlertConfiguration emailAlertConfiguration)
        {
            var emailAlertConfigModel = new EmailAlertingConfigModel
            {
                From = emailAlertConfiguration.FromAddress,
                Recipients = emailAlertConfiguration.ToEmailAddress,
                Enabled = emailAlertConfiguration.Enabled,
                SmtpHostname = emailAlertConfiguration.Host,
                IsPasswordSet = !string.IsNullOrEmpty(emailAlertConfiguration.Password),
                SmtpPort = emailAlertConfiguration.Port,
            };
            return emailAlertConfigModel;
        }
    }

    public class ConfigModel
    {
        public EmailAlertingConfigModel EmailAlertingConfig { get; set; }
    }

    public class EmailAlertingConfigModel
    {
        public bool Enabled { get; set; }
        public string Recipients { get; set; }
        public string From { get; set; }
        public bool IsPasswordSet { get; set; }
        public string SmtpHostname { get; set; }
        public int SmtpPort { get; set; }
    }
}