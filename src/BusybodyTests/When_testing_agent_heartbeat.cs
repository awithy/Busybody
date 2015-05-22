using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Busybody.Config;
using Busybody.WebServer;
using BusybodyTests.Fakes;
using BusybodyTests.Helpers;
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
            //_testContext.RunTests();
            //_testContext.ProcessEvents();
            //var hosts = new HostsController().GetHosts();
        }

        //[Test]
        //public void The_
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
            //var fakeAppContext = new FakeAppContextBuilder()
                //.WithConfig()
                //.Build();
            _testContext = BusybodyTestContext.Setup();
            _agentTestContext = BusybodyAgentTestContext.Setup();
            _testContext.TestAppContext.AzureAgentChannel = _agentTestContext.FakeAzureAgentChannel;
            _testContext.TestAppContext.FileAgentChannel = _agentTestContext.FakeFileAgentChannel;
        }
    }
}