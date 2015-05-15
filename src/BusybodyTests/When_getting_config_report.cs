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
        ConfigModel _actualConfig;

        [SetUp]
        public void SetUp()
        {
            _actualConfig = _configController.GetConfig();
            _emailConfig = _actualConfig.EmailAlertingConfig;
        }

        [Test]
        public void It_should_return_the_basic_configuration()
        {
            _actualConfig.DataDirectory.Should().Be(_config.DataDirectory);
            _actualConfig.ListeningUris.Should().Be(_config.ListeningUrls);
            _actualConfig.PollingInterval.Should().Be(_config.PollingInterval);
        }

        [Test]
        public void It_should_return_the_email_configuration()
        {
            _emailConfig.From.Should().Be(_config.EmailAlertConfiguration.FromAddress);
            _emailConfig.Recipients.Should().Be(_config.EmailAlertConfiguration.ToAddress);
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
