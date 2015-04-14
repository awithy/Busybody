using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace Busybody.WebServer
{
    [Authorize]
    public class EventLogController : ApiController
    {
        public HttpResponseMessage GetEventLog()
        {
            return WebServerHelpers.GetPage("eventLog.html");
        }
    }

    [Authorize]
    public class EventLogApiController : ApiController
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
