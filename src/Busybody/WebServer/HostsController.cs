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
                Name = x.Name,
                State = x.State.ToString(),
                LastUpdate = x.LastUpdate.ToString("o"),
                LastStateChange = x.LastStateChange.ToString("o"),
                IsDanger = x.State == HostState.DOWN,
            });
            return hostModels;
        }
    }
}