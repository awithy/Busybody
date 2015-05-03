using System;
using System.Linq;
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
    public class Given_alert_config_and_first_DOWN_host_state_event_handled : AlertingTestsBase
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
    public class Given_alert_config_and_first_UP_host_state_event_handled : AlertingTestsBase
    {
        [SetUp]
        public void SetUp()
        {
            _hostStateEvent.State = HostState.UP;
            _alertingEventHandler.Handle(_hostStateEvent);
        }

        [Test]
        public void It_should_not_alert_on_first_UP()
        {
            _fakeEmailInterface.EmailAlerts.Should().BeEmpty();
        }
    }

    [TestFixture]
    public class Given_a_host_state_event_UP_for_host_that_is_already_UP : AlertingTestsBase
    {
        [SetUp]
        public void SetUp()
        {
            _hostStateEvent.State = HostState.UP;
            _alertingEventHandler.Handle(_hostStateEvent);
            _alertingEventHandler.Handle(_hostStateEvent);
        }

        [Test]
        public void It_should_not_alert()
        {
            _fakeEmailInterface.EmailAlerts.Should().BeEmpty();
        }
    }

    [TestFixture]
    public class Given_a_host_that_goes_down_then_up_in_short_time_period : AlertingTestsBase
    {
        [SetUp]
        public void SetUp()
        {
            _hostStateEvent.State = HostState.UP;
            _alertingEventHandler.Handle(_hostStateEvent);
            _hostStateEvent.State = HostState.DOWN;
            _alertingEventHandler.Handle(_hostStateEvent);
            _hostStateEvent.State = HostState.UP;
            _alertingEventHandler.Handle(_hostStateEvent);
        }

        [Test]
        public void It_should_send_an_alert_for_each_state_change()
        {
            _fakeEmailInterface.EmailAlerts.First().Subject.Should().Contain("DOWN");
            _fakeEmailInterface.EmailAlerts.Skip(1).First().Subject.Should().Contain("UP");
            _fakeEmailInterface.EmailAlerts.Count.Should().Be(2);
        }
    }

    [TestFixture]
    public class Given_a_bunch_of_emails_in_a_short_period_of_time : AlertingTestsBase
    {
        [SetUp]
        public void SetUp()
        {
            //Ten from this batch (some will be throttled)
            for (var i = 0; i < 10; i++)
            {
                _hostStateEvent.State = HostState.UP;
                _alertingEventHandler.Handle(_hostStateEvent);
                _hostStateEvent.State = HostState.DOWN;
                _alertingEventHandler.Handle(_hostStateEvent);
                _alertingEventHandler.Handle(new SystemErrorEvent("Message", "Detail") { Timestamp = DateTime.UtcNow });
            }

            //Then two more
            _hostStateEvent.Timestamp = DateTime.UtcNow.AddMinutes(31);
            _hostStateEvent.State = HostState.UP;
            _alertingEventHandler.Handle(_hostStateEvent);
            _hostStateEvent.State = HostState.DOWN;
            _alertingEventHandler.Handle(_hostStateEvent);
        }

        [Test]
        public void It_should_throttle_to_10_alerts_and_then_a_30_minute_break()
        {
            _fakeEmailInterface.EmailAlerts.Count.Should().Be(12);
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
    public class Given_alert_config_and_alerting_is_disabled_in_config : AlertingTestsBase
    {
        [SetUp]
        public void SetUp()
        {
            _testContext.TestAppContext.Config.EmailAlertConfiguration.Enabled = false;
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
            _hostStateEvent.Timestamp = DateTime.UtcNow;
            _alertingEventHandler = new AlertingEventHandler();
            _testContext = BusybodyTestContext.Setup();
            _fakeEmailInterface = _testContext.TestAppContext.FakeEmailAlertingInterface;
            _testContext.TestAppContext.Config.EmailAlertConfiguration.Enabled = true;
        }
    }
}
