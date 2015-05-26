using System;

namespace BusybodyShared
{
    public interface IAgentChannel
    {
        void Heartbeat(string agentId, DateTime timestamp);
        DateTime ReadHeartbeat(string agentId);
    }
}