using Busybody.Config;

namespace Busybody.Tests
{
    public class AzureAgentHeartbeatTest : IBusybodyTest
    {
        public bool Execute(HostConfig host, HostTestConfig test)
        {
            return true;
        }
    }
}