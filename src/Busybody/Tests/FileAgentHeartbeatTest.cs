using System;
using Busybody.Config;

namespace Busybody.Tests
{
    public class FileAgentHeartbeatTest : IBusybodyTest
    {
        public bool Execute(HostConfig host, HostTestConfig test)
        {
            var fileChannel = AppContext.Instance.FileAgentChannel;
            var heartbeatTimestamp = fileChannel.ReadHeartbeat(host.AgentId);
            var timeoutSeconds = 900;
            if (test.Parameters.ContainsKey("Timeout"))
                timeoutSeconds = int.Parse(test.Parameters["Timeout"]);
            if ((DateTime.UtcNow - heartbeatTimestamp.ToUniversalTime()).TotalSeconds > timeoutSeconds)
                return false;
            return true;
        }
    }
}