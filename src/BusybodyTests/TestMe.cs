using System.Text;
using Busybody.Config;
using Busybody.Utility;
using NUnit.Framework;

namespace BusybodyTests
{
    [TestFixture]
    public class TestMe
    {
        [Test]
        public void slkdfj()
        {
            var emailConfig = new EmailAlertConfiguration
            {
                Enabled = true,
                FromAddress = "twgautomated@gmail.com",
                Host = "smtp.gmail.com",
                Password = "RWyfxaAw",
                Port = 587,
                ToAddress = "awithy@msn.com"
            };

            var emailClient = new EmailClient(emailConfig);
            emailClient.Send(new Email(new[] {emailConfig.ToAddress}, "subject", "body"));
        }
    }
}
