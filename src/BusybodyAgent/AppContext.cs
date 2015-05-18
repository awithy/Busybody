using System;
using BusybodyShared;

namespace BusybodyAgent
{
    public interface IAppContext
    {
        DateTime StartTime { get; }
        BusybodyAgentConfig Config { get; set;  }
        IAgentChannel FileAgentChannel { get; set; }
        IAgentChannel AzureAgentChannel { get; set; }
    }

    public class AppContext : IAppContext
    {
        public DateTime StartTime { get; private set; }
        public static IAppContext Instance;
        public BusybodyAgentConfig Config { get; set; }
        public IAgentChannel FileAgentChannel { get; set; }
        public IAgentChannel AzureAgentChannel { get; set; }

        public AppContext(BusybodyAgentConfig config)
        {
            Config = config;
            StartTime = DateTime.UtcNow;

            if (Config.FileAgentChannelConfig != null)
                FileAgentChannel = new FileAgentChannel(Config.FileAgentChannelConfig);
            else
                FileAgentChannel = new NullAgentChannel();

            if (Config.AzureAgentChannelConfig != null)
                AzureAgentChannel = new AzureAgentChannel(Config.AzureAgentChannelConfig);
            else
                AzureAgentChannel = new NullAgentChannel();
        }
    }
}