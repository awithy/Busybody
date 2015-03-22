using System.Collections.Concurrent;

namespace Busybody
{
    public class HostRepository
    {
        public ConcurrentDictionary<string, Host> Hosts = new ConcurrentDictionary<string, Host>();

        public Host GetOrCreateHost(string name)
        {
            return Hosts.AddOrUpdate(name, n => new Host {Name = n}, (n, existingHost) => existingHost);
        }

        public void UpdateHost(Host host)
        {
            Hosts.AddOrUpdate(host.Name, n => host, (n, existingHost) => host);
        }
    }
}