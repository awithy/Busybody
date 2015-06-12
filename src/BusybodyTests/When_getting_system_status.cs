using System;
using Busybody;
using Busybody.WebServer;
using BusybodyTests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace BusybodyTests
{
    [TestFixture]
    public class When_getting_system_status : SystemStatusTests
    {
        SystemStatusModel _systemStatus;

        [SetUp]
        public void SetUp()
        {
            _systemStatus = SystemStatusController.GetSystemStatus();
        }

        [Test]
        public void It_should_include_the_start_time()
        {
            DateTime.Parse(_systemStatus.StartTime).ToUniversalTime().Day.Should().Be(DateTime.UtcNow.Day);
        }
    }

    public class SystemStatusTests
    {
        protected BusybodyTestContext _testContext;
        protected SystemStatusController SystemStatusController;

        [SetUp]
        public void BaseSetUp()
        {
            _testContext = BusybodyTestContext.Setup();
            SystemStatusController = new SystemStatusController();
            AppContext.Instance.SystemStatus.UpdateHealth();
        }
    }
}
