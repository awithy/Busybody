using System.Collections.Generic;
using System.Web.Http;

namespace Busybody.WebServer
{
    [Authorize]
    public class EventLogController : ApiController
    {
        public IEnumerable<EventModel> GetEvents()
        {
            return AppContext.Instance.EventLogRepository.GetEvents();
        }
    }

    public class EventModel
    {
        public string Timestamp { get; set; }
        public string EventType { get; set; }
        public string EventMessage { get; set; }
        public bool IsDanger { get; set; }
    }
}
