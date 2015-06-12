using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace Busybody.WebServer
{
    public class SystemStatusController : ApiController
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
            var systemStatusModel = new SystemStatusModel
            {
                StartTime = startTime.ToString("o"),
                Uptime = string.Format("{0}d {1:00}:{2:00}:{3:00}", uptime.Days, uptime.Hours, uptime.Minutes, uptime.Seconds),
                LastUpdate = systemStatus.LastUpdate.ToString("o"),
                SystemHealth = systemStatus.SystemHealth.ToString(),
                UsedMemory = systemStatus.UsedMemory.ToString("f1") + " MB",
                Cpu = systemStatus.Cpu.ToString("f1") + " %",
                RoleServices = systemStatus.GetRoleServiceHealthStatus()
                    .Select(x => new SystemStatusRoleServiceModel
                    {
                        Name = x.Name,
                        RoleServiceHealth = x.RoleServiceHealth.ToString(),
                        LastPoll = x.LastPoll.ToString("o"),
                        LastDuration = x.LastPollDuration.TotalSeconds.ToString("f1") + "s",
                        LastError = x.LastErrorMessage,
                    }),
            };
            return systemStatusModel;
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
        public IEnumerable<SystemStatusRoleServiceModel> RoleServices { get; set; }

        public SystemStatusModel()
        {
            RoleServices = new SystemStatusRoleServiceModel[0];
        }
    }

    public class SystemStatusRoleServiceModel
    {
        public string Name { get; set; }
        public string RoleServiceHealth { get; set; }
        public string LastPoll { get; set; }
        public string LastError { get; set; }
        public string LastDuration { get; set; }
    }
}
