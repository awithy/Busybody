using System;
using System.Collections.Generic;

namespace Busybody.Config
{
    public class HostConfig
    {
        public string Nickname { get; set; }
        public string Hostname { get; set; }
        public string Location { get; set; }
        public string Group { get; set; }
        public string AgentId { get; set; }

        public List<HostTestConfig> Tests { get; set; }

        public HostConfig()
        {
            Tests = new List<HostTestConfig>();
        }
    }
}