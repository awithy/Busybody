using System;
using System.Web.Http;

namespace Busybody.WebServer
{
    public class SystemStatusApiController : ApiController
    {
        public SystemStatusModel GetSystemStatus()
        {
            var systemStatusModel = _BuildSystemStatusModel(AppContext.Instance.SystemMonitorData);
            return systemStatusModel;
        }

        SystemStatusModel _BuildSystemStatusModel(SystemMonitorData systemMonitorData)
        {
            var startTime = systemMonitorData.GetStartTime();
            var uptime = (DateTime.UtcNow - startTime);
            return new SystemStatusModel
            {
                StartTime = startTime.ToString("o"),
                Uptime = string.Format("{0}d {1:00}:{2:00}:{3:00}", uptime.Days, uptime.Hours, uptime.Minutes, uptime.Seconds),
            };
        }
    }

    public class SystemStatusModel
    {
        public string StartTime { get; set; }
        public string Uptime { get; set; }
    }
}
