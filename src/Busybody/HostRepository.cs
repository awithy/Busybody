using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<Host> GetHosts()
        {
            return Hosts.Values.ToArray();
        }

        public Host GetHostById(string id)
        {
            return Hosts.Values.Single(x => x.Id == id);
        }

        public bool ExistsById(string id)
        {
            return Hosts.Values.Any(x => x.Id == id);
        }
    }
}