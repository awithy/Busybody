using System;
using System.Collections.Generic;
using BusybodyShared;

namespace BusybodyTests.Helpers
{
    public class FakeAgentChannel : IAgentChannel
    {
        public Dictionary<string, DateTime> LastHeartbeats = new Dictionary<string, DateTime>();

        public void Heartbeat(string agentId, DateTime timestamp)
        {
            if(!LastHeartbeats.ContainsKey(agentId))
                LastHeartbeats.Add(agentId, timestamp);
            else
                LastHeartbeats[agentId] = timestamp;
        }

        public DateTime ReadHeartbeat(string agentId)
        {
            if (!LastHeartbeats.ContainsKey(agentId))
                return default(DateTime);
            return LastHeartbeats[agentId];
        }
    }
}