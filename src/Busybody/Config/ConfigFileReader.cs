using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Busybody.Config
{
    //Todo: CsvHelper
    public class ConfigFileReader
    {
        static Logger _log = new Logger(typeof(ConfigFileReader));

        public BusybodyConfig ReadFromFile(string filePath)
        {
            _log.Debug("Reading config from " + filePath);
            var hostConfigs = new List<HostConfig>();
            using (var streamReader = new StreamReader(filePath))
            {
                var line = streamReader.ReadLine();
                while (line != null)
                {
                    line = line.Trim('\t');
                    if (line.ToLower().StartsWith("host,"))
                    {
                        var hostConfig = _ParseHostLine(line);
                        hostConfigs.Add(hostConfig);
                    }
                    else if (line.ToLower().StartsWith("hosttest,"))
                    {
                        var testConfig = _ParseTestConfig(line);
                        hostConfigs.Single(x => x.Nickname.ToLower() == testConfig.HostNickname.ToLower()).Tests.Add(testConfig);
                    }
                    line = streamReader.ReadLine();
                }
            }
            var busybodyConfig = new BusybodyConfig();
            busybodyConfig.Hosts.AddRange(hostConfigs);
            return busybodyConfig;
        }

        HostTestConfig _ParseTestConfig(string line)
        {
            var fields = line.Split(new[] {','});
            var testName = fields[2];
            switch (testName)
            {
                case "Ping":
                    return new PingTestConfig
                    {
                        HostNickname = fields[1],
                    };
                default:
                    throw new TestConfigNotFoundException(testName);
            }
        }

        HostConfig _ParseHostLine(string line)
        {
            var fields = line.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            var hostConfig = new HostConfig
            {
                Nickname = fields[1],
                Hostname = fields[2],
            };
            return hostConfig;
        }
    }

    public class TestConfigNotFoundException : Exception
    {
        public TestConfigNotFoundException(string testName) : base("No test " + testName + " found.")
        {
        }
    }

}