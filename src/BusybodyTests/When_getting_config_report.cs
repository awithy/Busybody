using Busybody;
using Busybody.Config;
using Busybody.WebServer;
using BusybodyTests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace BusybodyTests
{
    [TestFixture]
    public class When_getting_config_report : ConfigReportTests
    {
        EmailAlertingConfigModel _emailConfig;

        [SetUp]
        public void SetUp()
        {
            _emailConfig = _configController.GetConfig().EmailAlertingConfig;
        }

        [Test]
        public void It_should_return_the_email_configuration()
        {
            _emailConfig.From.Should().Be(_config.EmailAlertConfiguration.FromAddress);
            _emailConfig.Recipients.Should().Be(_config.EmailAlertConfiguration.ToEmailAddress);
            _emailConfig.IsPasswordSet.Should().Be(true);
            _emailConfig.SmtpHostname.Should().Be(_config.EmailAlertConfiguration.Host);
            _emailConfig.SmtpPort.Should().Be(_config.EmailAlertConfiguration.Port);
            _emailConfig.Enabled.Should().Be(false);
        }
    }

    public class ConfigReportTests
    {
        protected BusybodyTestContext _testContext;
        protected BusybodyConfig _config;
        protected ConfigController _configController;

        [SetUp]
        public void BaseSetUp()
        {
            _testContext = BusybodyTestContext.Setup();
            _configController = new ConfigController();
            _config = AppContext.Instance.Config;
        }
    }
}
