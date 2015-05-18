using System;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BusybodyShared
{
    public class AzureAgentChannel : IAgentChannel
    {
        AzureAgentChannelConfig _config;
        Uri _storageUri;
        readonly StorageCredentials _storageCredentials;
        readonly Uri _containerAddress;
        readonly Logger _log = new Logger(typeof(AzureAgentChannel));
        string _containerName;

        public AzureAgentChannel(AzureAgentChannelConfig config)
        {
            _config = config;
            _storageUri = new Uri(string.Format("https://{0}.blob.core.windows.net/", _config.AccountName));
            _storageCredentials = new StorageCredentials(_config.AccountName, _config.AccountKey);
            _containerName = "bb-agent-heartbeats";
            _containerAddress = new Uri(_storageUri, _containerName);
        }

        public void Heartbeat(string agentId, DateTime timestamp)
        {
            try
            {
                _HearbeatInner(agentId, timestamp);
            }
            catch (Exception ex)
            {
                _log.Debug(ex.GetType().Name + " thrown while heartbeating.  Swallowing exception.  Detail:" + ex);
            }
        }

        void _HearbeatInner(string agentId, DateTime timestamp)
        {
            _log.Debug("Heartbeat Inner " + agentId + " " + timestamp);
            var container = new CloudBlobContainer(_containerAddress, _storageCredentials);
            container.CreateIfNotExists();
            var heartbeatBlobUri = new Uri(_storageUri, _containerName + "/" + agentId);
            var cloudBlob = new CloudBlockBlob(heartbeatBlobUri, _storageCredentials);
            cloudBlob.UploadText(timestamp.ToString("o"));
        }

        public DateTime ReadHeartbeat(string agentId)
        {
            _log.Debug("Reading heartbeat for " + agentId);
            try
            {
                return _ReadHeartbeatInner(agentId); 
            }
            catch (Exception ex)
            {
                _log.Debug(ex.GetType().Name + " thrown while reading Azure heartbeat.  Swallowing exception.  Detail:" + ex);
                return default(DateTime);
            }
        }

        DateTime _ReadHeartbeatInner(string agentId)
        {
            var container = new CloudBlobContainer(_containerAddress, _storageCredentials);
            container.CreateIfNotExists();
            var heartbeatBlobUri = new Uri(_storageUri, _containerName + "/" + agentId);
            var cloudBlob = new CloudBlockBlob(heartbeatBlobUri, _storageCredentials);
            if (cloudBlob.Exists())
            {
                var blobContents = cloudBlob.DownloadText();
                var timestamp = DateTime.Parse(blobContents).ToUniversalTime();
                return timestamp;
            }
            return default(DateTime);
        }
    }
}