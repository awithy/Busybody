using System;
using System.Collections.Generic;
using System.Diagnostics;
using BusybodyShared;

namespace BusybodyDekstopNotificationApp
{
    public class SystemStatusProvider
    {
        public IEnumerable<SystemStatus> GetSystemStatuses()
        {
            var systems = AppContext.Instance.Config.Systems;
            var storageConfig = AppContext.Instance.Config.AzureStorageConfig;
            if (storageConfig == null)
                return new SystemStatus[0];
            var azureStatusRepository = AppContext.Instance.AzureStatusRepository;
            var statuses = new List<SystemStatus>();

            foreach (var system in systems)
            {
                var newStatus = _GetStateFromAzure(storageConfig, azureStatusRepository, system);
                statuses.Add(newStatus);
            }
            return statuses;
        }

        static SystemStatus _GetStateFromAzure(AzureStorageConfig config, AzureStatusRepository azureStatusRepository, SystemConfig system)
        {
            var newStatus = new SystemStatus
            {
                Name = system.Name,
                SystemId = system.SystemId,
                Url = system.Url,
                State = AzureStatusState.DOWN,
            };
            try
            {
                var azureStatus = azureStatusRepository.GetStatus(config, system.SystemId);
                var timestamp = DateTime.Parse(azureStatus.Timestamp).ToUniversalTime();
                if (DateTime.UtcNow.Subtract(timestamp).TotalMinutes >= 5)
                    newStatus.State = AzureStatusState.DOWN;
                else
                    newStatus.State = (AzureStatusState) Enum.Parse(typeof (AzureStatusState), azureStatus.State);
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
            }
            return newStatus;
        }
    }

    public class SystemStatus
    {
        public string SystemId { get; set; }
        public string Name { get; set; }
        public AzureStatusState State { get; set; }
        public string Url { get; set; }
    }
}
