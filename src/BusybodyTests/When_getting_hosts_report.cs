using System.Collections.Generic;
using System.Linq;
using Busybody;
using Busybody.Events;
using Busybody.WebServer;
using BusybodyTests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace BusybodyTests
{
    [TestFixture]
    public class When_getting_hosts_from_hosts_controller : HostsReportTests
    {
        IEnumerable<HostModel> _hosts;

        [SetUp]
        public void SetUp()
        {
            _hostEventHandler.Handle(_failedTestResult);
            _testContext.TestAppContext.EventBus.DispatchPending();
            _hosts = _hostsController.GetHosts();
        }

        [Test]
        public void It_should_list_the_hosts()
        {
            _hosts.Should().ContainSingle(x => x.State == HostState.DOWN.ToString());
        }

        [Test]
        public void It_should_have_the_host_location()
        {
            _hosts.Should().ContainSingle(x => x.Location == "Location 1");
        }
    }

    public class HostsReportTests
    {
        protected BusybodyTestContext _testContext;
        protected HostTestResultEvent _failedTestResult;
        protected HostEventHandler _hostEventHandler;
        protected HostsController _hostsController;

        [SetUp]
        public void BaseSetUp()
        {
            _failedTestResult = new HostTestResultEvent
            {
                HostNickname = "Local Machine",
                TestName = "Ping",
                TestResult = false,
            };
            _testContext = BusybodyTestContext.Setup();
            _hostEventHandler = new HostEventHandler();
            _hostsController = new HostsController();
        }
    }
}
