using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Busybody.Events;

namespace Busybody.WebServer
{
    public class HostsController : ApiController
    {
        [Authorize]
        public HostsByGroupModel GetHosts()
        {
            var hostGroups = new List<HostGroupModel>();
            var hosts = AppContext.Instance.HostRepository.GetHosts().ToArray();
            var groups = hosts.Select(x => x.Group).Distinct().OrderBy(x => x);
            foreach (var group in groups)
            {
                var hostsInGroup = hosts
                    .Where(x => x.Group == group)
                    .ToArray();
                var hostModelsInGroup = hostsInGroup.Select(_ConvertToHostModel);
                hostGroups.Add(new HostGroupModel
                {
                    Name = group,
                    Hosts = hostModelsInGroup,
                    State = hostsInGroup.All(x => x.State == HostState.UP) ? "UP" : hostsInGroup.Any(x => x.State == HostState.DOWN) ? "DOWN" : "WARN",
                });
            }
            return new HostsByGroupModel{ HostGroups = hostGroups };
        }

        HostModel _ConvertToHostModel(Host host)
        {
            return new HostModel
            {
                Id = host.Id,
                Name = host.Name,
                State = _CalculateUiState(host),
                LastUpdate = host.LastUpdate.ToString("o"),
                LastStateChange = host.LastStateChange.ToString("o"),
                Location = host.Location,
                Group = host.Group,
            };
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
                Location = host.Location,
                Group = host.Group,
                Tests = host.Tests.Select(x => new HostTestModel
                {
                    Name = x.Value.Name,
                    State = x.Value.State.ToString(),
                    LastTest = x.Value.LastTest.ToString("o"),
                }),
            };
            return hostModel;
        }

        string _CalculateUiState(Host host)
        {
            if (host.State == HostState.DOWN)
                return "DOWN";

            //If host last state change was near the system starting, return UP and don't warn
            var differenceBetweenStartTimeAndHostLastStateChange = host.LastStateChange - AppContext.Instance.StartTime;
            var didTheHostLastChangeStateAtStart = differenceBetweenStartTimeAndHostLastStateChange.TotalMinutes < 5;
            if (didTheHostLastChangeStateAtStart)
                return "UP";

            //If host went down > 1 day ago, don't warn
            if (host.State == HostState.UP && DateTime.UtcNow.Subtract(host.LastStateChange).TotalDays >= 1)
                return "UP";
            
            //This means host was down in the alst day -> warn
            return "WARN";
        }
    }
}