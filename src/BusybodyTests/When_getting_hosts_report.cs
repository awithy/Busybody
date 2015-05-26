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
            _hosts = _hostsController.GetHosts().HostGroups.SelectMany(x => x.Hosts);
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

        [Test]
        public void It_should_have_the_host_group()
        {
            _hosts.Should().ContainSingle(x => x.Group == "Host Group");
        }
    }

    [TestFixture]
    public class When_getting_host_by_id_from_hosts_controller : HostsReportTests
    {
        HostModel _host;

        [SetUp]
        public void SetUp()
        {
            _hostEventHandler.Handle(_failedTestResult);
            _testContext.TestAppContext.EventBus.DispatchPending();
            var hosts = _hostsController.GetHosts().HostGroups.SelectMany(x => x.Hosts);
            _host = _hostsController.GetHostById(hosts.First().Id);
        }

        [Test]
        public void It_should_get_the_host()
        {
            _host.Should().NotBeNull();
        }

        [Test]
        public void It_should_list_the_host_tests()
        {
            _host.Tests.First().Name.Should().Be("Ping");
        }

        [Test]
        public void It_should_list_the_host_test_state()
        {
            _host.Tests.First().State.Should().Be("Fail");
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
