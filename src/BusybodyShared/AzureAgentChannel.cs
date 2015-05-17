using System;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BusybodyShared
{
    public class AzureAgentChannel : IAgentChannel
    {
        AzureAgentChannelConfig _config;
        Uri _storageUri;
        StorageCredentials _storageCredentials;
        Uri _containerAddress;

        public AzureAgentChannel(AzureAgentChannelConfig config)
        {
            _config = config;
            _storageUri = new Uri(string.Format("https://{0}.blob.core.windows.net/", _config.AccountName));
            _storageCredentials = new StorageCredentials(_config.AccountName, _config.AccountKey);
            _containerAddress = new Uri(_storageUri, "bb-agent-heartbeats");
        }

        public void Heartbeat(string agentId, DateTime timestamp)
        {
            var container = new CloudBlobContainer(_containerAddress, _storageCredentials);
            container.CreateIfNotExists();
            var heartbeatBlobUri = new Uri(_containerAddress, agentId);
            var cloudBlob = new CloudBlockBlob(heartbeatBlobUri, _storageCredentials);
            cloudBlob.UploadText(timestamp.ToString("o"));
        }

        public DateTime ReadHeartbeat(string agentId)
        {
            return default(DateTime);
        }
    }
}
