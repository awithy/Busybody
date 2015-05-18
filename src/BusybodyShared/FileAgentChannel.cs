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
            var heartbeatDirectory = Path.Combine(_config.DirectoryPath, "bb-agent-heartbeat");
            if (!Directory.Exists(heartbeatDirectory))
                Directory.CreateDirectory(heartbeatDirectory);
            var heartbeatFilePath = Path.Combine(heartbeatDirectory, agentId);
            File.WriteAllText(heartbeatFilePath, timestamp.ToString("o"));
        }

        public DateTime ReadHeartbeat(string agentId)
        {
            _log.Debug("Reading heartbeat for " + agentId);
            var heartbeatDirectory = Path.Combine(_config.DirectoryPath, "bb-agent-heartbeat");
            if (!Directory.Exists(heartbeatDirectory))
                Directory.CreateDirectory(heartbeatDirectory);
            var heartbeatFilePath = Path.Combine(heartbeatDirectory, agentId);
            if (!File.Exists(heartbeatFilePath))
                return default(DateTime);
            var fileContents = File.ReadAllText(heartbeatFilePath);
            var timestamp = DateTime.Parse(fileContents);
            _log.Debug("Heartbeat timestamp for " + agentId + " " + timestamp);
            return timestamp;
        }
    }
}