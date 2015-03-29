using Busybody;
using Busybody.Config;
using Busybody.Events;
using BusybodyTests.Fakes;
using BusybodyTests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace BusybodyTests
{
    [TestFixture]
    public class Given_alert_config_and_first_host_state_event_handled : AlertingTestsBase
    {
        [SetUp]
        public void SetUp()
        {
            _alertingEventHandler.Handle(_hostStateEvent);
        }

        [Test]
        public void It_should_alert()
        {
            _fakeEmailInterface.EmailAlerts.Should().ContainSingle();
        }
    }

    [TestFixture]
    public class Given_alert_config_and_multiple_alerts : AlertingTestsBase
    {
        [SetUp]
        public void SetUp()
        {
            _alertingEventHandler.Handle(_hostStateEvent);
            _alertingEventHandler.Handle(_hostStateEvent);
        }

        [Test]
        public void It_should_only_alert_at_most_every_5_minutes()
        {
            _fakeEmailInterface.EmailAlerts.Should().ContainSingle();
        }
    }

    [TestFixture]
    public class Given_no_email_configured_and_host_state_event_handled : AlertingTestsBase
    {
        [SetUp]
        public void SetUp()
        {
            _testContext.TestAppContext.Config.EmailAlertConfiguration = null;
            _alertingEventHandler.Handle(_hostStateEvent);
        }

        [Test]
        public void It_should_not_alert()
        {
            _fakeEmailInterface.EmailAlerts.Should().BeEmpty();
        }
    }

    [TestFixture]
    public class AlertingTestsBase
    {
        protected AlertingEventHandler _alertingEventHandler;
        protected BusybodyTestContext _testContext;
        protected HostStateEvent _hostStateEvent;
        protected FakeEmailAlertingInterface _fakeEmailInterface;

        [SetUp]
        public void BaseSetUp()
        {
            _hostStateEvent = new HostStateEvent("Nickname", HostState.DOWN);
            _alertingEventHandler = new AlertingEventHandler();
            _testContext = BusybodyTestContext.Setup();
            _fakeEmailInterface = _testContext.TestAppContext.FakeEmailAlertingInterface;
            _testContext.TestAppContext.Config.EmailAlertConfiguration = _DummyEmailConfiguration();
        }

        EmailAlertConfiguration _DummyEmailConfiguration()
        {
            return new EmailAlertConfiguration
            {
                Host = "host",
                Port = 123,
                FromAddress = "a@a.com",
                ToEmailAddress = "b@b.com",
                Password = "password",
            };
        }
    }
}
