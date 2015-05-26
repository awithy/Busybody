using System;
using BusybodyShared;

namespace BusybodyAgent
{
    public class AgentCore
    {
        Logger _log = new Logger(typeof(AgentCore));

        public void Heartbeat(DateTime timestamp)
        {
            _log.Debug("Polling");
            var agentId = AppContext.Instance.Config.AgentId;
            AppContext.Instance.FileAgentChannel.Heartbeat(agentId, timestamp);
            AppContext.Instance.AzureAgentChannel.Heartbeat(agentId, timestamp);
        }
    }
}