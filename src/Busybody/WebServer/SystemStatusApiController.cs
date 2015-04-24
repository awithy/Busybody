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
            return new SystemStatusModel
            {
                StartTime = systemMonitorData.GetStartTime().ToString("o"),
            };
        }
    }

    public class SystemStatusModel
    {
        public string StartTime { get; set; }
        public string UpTime { get; set; }
    }
}
