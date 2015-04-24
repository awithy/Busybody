using System;
using System.Web.Http;

namespace Busybody.WebServer
{
    public class SystemStatusApiController : ApiController
    {
        public SystemStatusModel GetSystemStatus()
        {
            var systemStatusModel = _BuildSystemStatusModel(AppContext.Instance.SystemStatus);
            return systemStatusModel;
        }

        SystemStatusModel _BuildSystemStatusModel(SystemStatus systemStatus)
        {
            var startTime = systemStatus.GetStartTime();
            var uptime = (DateTime.UtcNow - startTime);
            return new SystemStatusModel
            {
                StartTime = startTime.ToString("o"),
                Uptime = string.Format("{0}d {1:00}:{2:00}:{3:00}", uptime.Days, uptime.Hours, uptime.Minutes, uptime.Seconds),
                LastUpdate = systemStatus.LastUpdate.ToString("o"),
                SystemHealth = systemStatus.GetSystemHealth().ToString(),
                UsedMemory = systemStatus.UsedMemory.ToString("f1") + " MB",
                Cpu = systemStatus.Cpu.ToString("f1") + " %",
            };
        }
    }

    public class SystemStatusModel
    {
        public string StartTime { get; set; }
        public string LastUpdate { get; set; }
        public string Uptime { get; set; }
        public string SystemHealth { get; set; }
        public string UsedMemory { get; set; }
        public string Cpu { get; set; }
    }
}
