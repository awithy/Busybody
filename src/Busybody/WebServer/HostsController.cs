using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Busybody.Events;

namespace Busybody.WebServer
{
    public class HostsController : ApiController
    {
        [Authorize]
        public IEnumerable<HostModel> GetHosts()
        {
            var hosts = AppContext.Instance.HostRepository.GetHosts();
            var hostModels = hosts.Select(x => new HostModel
            {
                Id = x.Id,
                Name = x.Name,
                State = _CalculateUiState(x),
                LastUpdate = x.LastUpdate.ToString("o"),
                LastStateChange = x.LastStateChange.ToString("o"),
                IsDanger = x.State == HostState.DOWN,
                Location = x.Location,
            });
            return hostModels;
        }

        [Authorize]
        public HostModel GetHostById(string id)
        {
            if (!AppContext.Instance.HostRepository.ExistsById(id))
                return null;

            var host = AppContext.Instance.HostRepository.GetHostById(id);
            var hostConfig = AppContext.Instance.Config.Hosts.Single(x => x.Nickname == host.Name);
            var hostModel = new HostModel
            {
                Id = host.Id,
                Name = host.Name,
                Hostname = hostConfig.Hostname,
                State = _CalculateUiState(host),
                LastUpdate = host.LastUpdate.ToString("o"),
                LastStateChange = host.LastStateChange.ToString("o"),
                IsDanger = host.State == HostState.DOWN,
                Location = host.Location,
            };
            return hostModel;
        }

        string _CalculateUiState(Host host)
        {
            if (host.State == HostState.DOWN)
                return "DOWN";

            var differenceBetweenStartTimeAndHostLastStateChange = host.LastStateChange - AppContext.Instance.StartTime;
            var didTheHostLastChangeStateAtStart = differenceBetweenStartTimeAndHostLastStateChange.TotalMinutes < 5;
            if (didTheHostLastChangeStateAtStart)
                return "UP";
            
            return "WARN";
        }
    }
}