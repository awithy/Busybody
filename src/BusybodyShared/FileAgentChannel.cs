using System;
using System.IO;

namespace BusybodyShared
{
    public class FileAgentChannel : IAgentChannel
    {
        readonly FileAgentChannelConfig _config;

        public FileAgentChannel(FileAgentChannelConfig config)
        {
            _config = config;
        }

        public void Heartbeat(string agentId, DateTime timestamp)
        {
            var heartbeatFilePath = Path.Combine(_config.DirectoryPath, "bb-agent-heartbeat");
            if (!Directory.Exists(_config.DirectoryPath))
                Directory.CreateDirectory(_config.DirectoryPath);
            File.WriteAllText(heartbeatFilePath, timestamp.ToString("o"));
        }

        public DateTime ReadHeartbeat(string agentId)
        {
            var heartbeatFilePath = Path.Combine(_config.DirectoryPath, "bb-agent-heartbeat");
            if (!Directory.Exists(_config.DirectoryPath))
                Directory.CreateDirectory(_config.DirectoryPath);
            var fileContents = File.ReadAllText(heartbeatFilePath);
            var timestamp = DateTime.Parse(fileContents);
            return timestamp;
        }
    }
}