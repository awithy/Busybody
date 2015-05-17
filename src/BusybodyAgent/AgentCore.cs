using System;

namespace BusybodyAgent
{
    public class AgentCore
    {
        public void Poll(DateTime timestamp)
        {
            var agentChannel = AppContext.Instance.AgentChannel;
            var agentId = AppContext.Instance.Config.AgentId;
            agentChannel.Heartbeat(agentId, timestamp);
        }
    }
}