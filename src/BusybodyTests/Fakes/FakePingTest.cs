using Busybody;
using Busybody.Config;

namespace BusybodyTests.Fakes
{
    public class FakePingTest : IBusybodyTest
    {
        public int ExecutedCount;
        public HostConfig LastHostConfig;
        public HostTestConfig LastHostTestConfig;

        public bool Execute(HostConfig host, HostTestConfig test)
        {
            ExecutedCount++;
            LastHostConfig = host;
            LastHostTestConfig = test;
            return true;
        }
    }
}