using System;
using System.Linq;
using Busybody.Events;
using BusybodyShared;

namespace Busybody
{
    public class AzureStatusWriter
    {
        public static readonly Logger _log = new Logger(typeof(AzureStatusWriter));

        public void Write(DateTime timestamp)
        {
            if (AppContext.Instance.Config.AzureStorageConfig == null)
                return;
            var config = AppContext.Instance.Config.AzureStorageConfig;

            _log.Trace("Writing Azure Status");

            var systemId = AppContext.Instance.Config.SystemId;
            var hosts = AppContext.Instance.HostRepository.GetHosts().ToArray();
            var upHosts = hosts.Count(x => x.State == HostState.UP);
            var downHosts = hosts.Count(x => x.State == HostState.DOWN);

            var timestampString = timestamp.ToString("o");
            var azureStatus = new AzureStatus
            {
                SystemId = systemId,
                Timestamp = timestampString,
                UpHosts = upHosts,
                DownHosts = downHosts,
            };

            var azureStatusRepository = new AzureStatusRepository();
            _log.TraceFormat("Azure status systemId:{0}, up hosts:{1}, down hosts:{2}, timestamp:{3}", systemId, upHosts, downHosts, timestampString);
            azureStatusRepository.Write(config, azureStatus);
        }
    }
}