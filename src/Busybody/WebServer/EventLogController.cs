using System.Collections;
using System.Collections.Generic;
using System.Web.Http;

namespace Busybody.WebServer
{
    public class EventLogController : ApiController
    {
        public IEnumerable<EventModel> GetEvents()
        {
            return AppContext.Instance.EventLogRepository.GetEvents();
        }
    }

    public class EventModel
    {
        public string EventType { get; set; }
        public string EventMessage { get; set; }
        public bool IsDanger { get; set; }
    }
}
