using System;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace BusybodyShared
{
    public class AzureStatusRepository
    {
        readonly Logger _log = new Logger(typeof(AzureStatusRepository));

        public void Write(AzureStorageConfig azureStorageConfig, AzureStatus azureStatus)
        {
            try
            {
                _WriteInner(azureStorageConfig, azureStatus);
            }
            catch (Exception ex)
            {
                _log.Debug(ex.GetType().Name + " thrown while writing Azure status.  Swallowing exception.  Detail:" + ex);
            }
        }

        void _WriteInner(AzureStorageConfig config, AzureStatus azureStatus)
        {
            _log.Trace("_WriteInner");
            var cloudBlob = _GetStatusBlob(config, azureStatus.SystemId);
            var json = JsonConvert.SerializeObject(azureStatus, Formatting.Indented);
            cloudBlob.UploadText(json);
        }

        static CloudBlockBlob _GetStatusBlob(AzureStorageConfig config, string systemId)
        {
            var _storageUri = new Uri(string.Format("https://{0}.blob.core.windows.net/", config.AccountName));
            var _storageCredentials = new StorageCredentials(config.AccountName, config.AccountKey);
            var _containerName = "bb-status";
            var _containerAddress = new Uri(_storageUri, _containerName);
            var container = new CloudBlobContainer(_containerAddress, _storageCredentials);
            container.CreateIfNotExists();
            var blobUri = new Uri(_storageUri, _containerName + "/" + systemId);
            var cloudBlob = new CloudBlockBlob(blobUri, _storageCredentials);
            return cloudBlob;
        }

        public AzureStatus GetStatus(AzureStorageConfig config, string systemId)
        {
            var cloudBlob = _GetStatusBlob(config, systemId);
            var contents = cloudBlob.DownloadText();
            var deserialized = JsonConvert.DeserializeObject<AzureStatus>(contents);
            return deserialized;
        }
    }

    public class AzureStatus
    {
        public string SystemId { get; set; }
        public string Timestamp { get; set; }
        public int UpHosts { get; set; }
        public int DownHosts { get; set; }
    }

    public enum AzureStatusState
    {
        Unknown,
        UP,
        DOWN,
    }
}