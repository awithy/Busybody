using System;
using System.IO;

namespace BusybodyShared
{
    public class FileAgentChannel : IAgentChannel
    {
        readonly FileAgentChannelConfig _config;
        readonly Logger _log = new Logger(typeof(FileAgentChannel));

        public FileAgentChannel(FileAgentChannelConfig config)
        {
            _config = config;
        }

        public void Heartbeat(string agentId, DateTime timestamp)
        {
            _log.Debug("Heartbeating " + agentId + " " + timestamp);
            var heartbeatFilePath = Path.Combine(_config.DirectoryPath, "bb-agent-heartbeat");
            if (!Directory.Exists(_config.DirectoryPath))
                Directory.CreateDirectory(_config.DirectoryPath);
            File.WriteAllText(heartbeatFilePath, timestamp.ToString("o"));
        }

        public DateTime ReadHeartbeat(string agentId)
        {
            _log.Debug("Reading heartbeat for " + agentId);
            var heartbeatFilePath = Path.Combine(_config.DirectoryPath, "bb-agent-heartbeat");
            if (!Directory.Exists(_config.DirectoryPath))
                Directory.CreateDirectory(_config.DirectoryPath);
            var fileContents = File.ReadAllText(heartbeatFilePath);
            var timestamp = DateTime.Parse(fileContents);
            _log.Debug("Heartbeat timestamp for " + agentId + " " + timestamp);
            return timestamp;
        }
    }
}