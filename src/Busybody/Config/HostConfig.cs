using System.Collections.Generic;

namespace Busybody.Config
{
    public class HostConfig
    {
        public string Hostname { get; set; }
        public string Nickname { get; set; }
        public List<HostTestConfig> Tests { get; set; }

        public HostConfig()
        {
            Tests = new List<HostTestConfig>();
        }
    }
}