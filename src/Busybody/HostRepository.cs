using System.Collections.Concurrent;
using Busybody.Config;

namespace Busybody
{
    public class HostRepository
    {
        public ConcurrentDictionary<string, Host> Hosts = new ConcurrentDictionary<string, Host>();

        public Host GetOrCreateHost(HostConfig hostConfig)
        {
            return Hosts.AddOrUpdate(hostConfig.Nickname, n => new Host(hostConfig), (n, existingHost) => existingHost);
        }

        public void UpdateHost(Host host)
        {
            Hosts.AddOrUpdate(host.Name, n => host, (n, existingHost) => host);
        }

        public bool Exists(string hostNickname)
        {
            return Hosts.ContainsKey(hostNickname);
        }
    }
}