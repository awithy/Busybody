using System;
using BusybodyShared;

namespace BusybodyAgent
{
    public class NullAgentChannel : IAgentChannel
    {
        public void Heartbeat(string agentId, DateTime timestamp)
        {
        }

        public DateTime ReadHeartbeat(string agentId)
        {
            return default(DateTime);
        }
    }
}