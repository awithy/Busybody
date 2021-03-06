﻿using System;
using BusybodyAgent;
using BusybodyShared;

namespace BusybodyTests.Helpers
{
    public class BusybodyAgentTestContext
    {
        public AgentCore AgentCore { get; set; }
        public TestAgentAppContext AppContext { get; set; }
        public FakeAgentChannel FakeAzureAgentChannel { get { return (FakeAgentChannel) AppContext.AzureAgentChannel; } }
        public FakeAgentChannel FakeFileAgentChannel { get { return (FakeAgentChannel) AppContext.FileAgentChannel; } }

        public static BusybodyAgentTestContext Setup()
        {
            var config = new BusybodyAgentConfig
            {
                AgentId = "agentid",
                AzureAgentChannelConfig = new AzureAgentChannelConfig(),
                FileAgentChannelConfig = new FileAgentChannelConfig(),
            };
            var appContext = new TestAgentAppContext(config);
            BusybodyAgent.AppContext.Instance = appContext;
            return new BusybodyAgentTestContext
            {
                AgentCore = new AgentCore(),
                AppContext = appContext,
            };
        }
    }

    public class TestAgentAppContext : IAppContext
    {
        public BusybodyAgentConfig Config { get; private set; }
        public IAgentChannel FileAgentChannel { get; private set; }
        public IAgentChannel AzureAgentChannel { get; private set; }

        public TestAgentAppContext(BusybodyAgentConfig config)
        {
            Config = config;
            FileAgentChannel = new FakeAgentChannel();
            AzureAgentChannel = new FakeAgentChannel();
        }
    }
}