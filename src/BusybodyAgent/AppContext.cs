using System;
using BusybodyShared;

namespace BusybodyAgent
{
    public interface IAppContext
    {
        BusybodyAgentConfig Config { get;  }
        IAgentChannel FileAgentChannel { get; }
        IAgentChannel AzureAgentChannel { get; }
    }

    public class AppContext : IAppContext
    {
        public static IAppContext Instance;
        public BusybodyAgentConfig Config { get; private set; }
        public IAgentChannel FileAgentChannel { get; private set; }
        public IAgentChannel AzureAgentChannel { get; private set; }

        public AppContext(BusybodyAgentConfig config)
        {
            Config = config;

            if (Config.FileAgentChannelConfig != null)
                FileAgentChannel = new FileAgentChannel(Config.FileAgentChannelConfig);
            else
                FileAgentChannel = new NullAgentChannel();

            if (Config.AzureAgentChannelConfig != null)
                AzureAgentChannel = new AzureAgentChannel(Config.AzureStorageConfig);
            else
                AzureAgentChannel = new NullAgentChannel();
        }
    }
}