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
            var state = AppContext.Instance.HostRepository.GetHosts().All(x => x.State == HostState.UP)
                ? AzureStatusState.UP
                : AzureStatusState.DOWN;

            var timestampString = timestamp.ToString("o");
            var azureStatus = new AzureStatus
            {
                SystemId = systemId,
                Timestamp = timestampString,
                State = state.ToString(),
            };

            var azureStatusRepository = new AzureStatusRepository();
            _log.TraceFormat("Azure status systemId:{0}, state:{1}, timestamp:{2}", systemId, state.ToString(), timestampString);
            azureStatusRepository.Write(config, azureStatus);
        }
    }
}