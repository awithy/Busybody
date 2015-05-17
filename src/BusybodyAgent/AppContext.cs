using System;
using BusybodyShared;

namespace BusybodyAgent
{
    public interface IAppContext
    {
        DateTime StartTime { get; }
        BusybodyAgentConfig Config { get; set;  }
        IAgentChannel AgentChannel { get; set; }
    }

    public class AppContext : IAppContext
    {
        public DateTime StartTime { get; private set; }
        public static IAppContext Instance;
        public BusybodyAgentConfig Config { get; set; }
        public IAgentChannel AgentChannel { get; set; }

        public AppContext(BusybodyAgentConfig config)
        {
            Config = config;
            StartTime = DateTime.UtcNow;

            switch (config.AgentChannelType)
            {
                case "File":
                    AgentChannel = new FileAgentChannel(config.FileAgentChannelConfig);
                    break;
                case "Azure":
                    AgentChannel = new AzureAgentChannel(config.AzureAgentChannelConfig);
                    break;
                default:
                    AgentChannel = new NullAgentChannel();
                    break;
            }
        }
    }
}