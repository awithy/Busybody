using System.Collections.Generic;

namespace Busybody.Config
{
    public class HostTestConfig
    {
        public virtual string Name { get; set; }
        public Dictionary<string, string> Parameters = new Dictionary<string, string>();

        public HostTestConfig(string name)
        {
            Name = name;
        }
    }
}