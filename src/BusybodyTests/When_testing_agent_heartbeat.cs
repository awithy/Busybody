using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Busybody;
using Busybody.Config;
using Busybody.Events;
using Busybody.WebServer;
using BusybodyTests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace BusybodyTests
{
    [TestFixture]
    public class When_testing_agent_heartbeat_and_the_heartbeat_is_within_timeout_period : AgentHeartbeatTests
    {
        [SetUp]
        public void SetUp()
        {
            var timestamp = DateTime.UtcNow;
            _agentTestContext.AgentCore.Heartbeat(timestamp);
            new HostTestRunnerCore().RunHostTests(new CancellationToken());
            _testContext.TestAppContext.EventBus.DispatchPending();
        }

        [Test]
        public void A_successful_host_test_result_event_should_be_raised()
        {
            _testContext.EventHandler.AssertSingleEventReceived<HostTestResultEvent>(x => x.TestResult);
        }

        [Test]
        public void The_host_should_be_up_in_the_UI()
        {
            var host = new HostsController().GetHosts().HostGroups.First().Hosts.First();
            host.State.Should().Be(HostState.UP.ToString());
        }
    }

    [TestFixture]
    public class When_testing_agent_heartbeat_and_the_heartbeat_is_outside_timeout_period : AgentHeartbeatTests
    {
        [SetUp]
        public void SetUp()
        {
            var timestamp = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(20));
            _agentTestContext.AgentCore.Heartbeat(timestamp);
            new HostTestRunnerCore().RunHostTests(new CancellationToken());
            _testContext.TestAppContext.EventBus.DispatchPending();
        }

        [Test]
        public void A_failed_host_test_result_event_should_be_raised()
        {
            _testContext.EventHandler.AssertSingleEventReceived<HostTestResultEvent>(x => x.TestResult == false);
        }

        [Test]
        public void The_host_should_be_down_in_the_UI()
        {
            var host = new HostsController().GetHosts().HostGroups.First().Hosts.First();
            host.State.Should().Be(HostState.DOWN.ToString());
        }
    }

    public class AgentHeartbeatTests
    {
        protected BusybodyTestContext _testContext;
        protected BusybodyAgentTestContext _agentTestContext;

        [SetUp]
        public void BaseSetUp()
        {
            var busybodyConfig = new BusybodyConfig
            {
                Hosts = new List<HostConfig>
                {
                    new HostConfig
                    {
                        Nickname = "host",
                        AgentId = "agentid",
                        Tests = new List<HostTestConfig>
                        {
                            new HostTestConfig("AzureAgentHeartbeat")
                        },
                    },   
                },
            };
            _testContext = BusybodyTestContext.Setup(busybodyConfig);
            _agentTestContext = BusybodyAgentTestContext.Setup();
            _testContext.TestAppContext.AzureAgentChannel = _agentTestContext.FakeAzureAgentChannel;
            _testContext.TestAppContext.FileAgentChannel = _agentTestContext.FakeFileAgentChannel;
        }
    }
}