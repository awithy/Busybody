using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading;
using Busybody;
using FluentAssertions;
using NUnit.Framework;

namespace BusybodyTests
{
    [TestFixture]
    public class Given_an_alive_host_and_a_ping_test_configured_when_console_started
    {
        TestEventLogReader _testEventLogReader;

        [SetUp]
        public void Execute()
        {
            var config = new ConfigBuilder()
                .WithHost("Local Machine", "127.0.0.1")
                .WithTest(new PingTestConfig())
                .BuildHostConfig()
                .BuildConfig();

            using (var testDirectory = new TestDirectory())
            {
                var configFilePath = testDirectory.FilePathFor(SharedConstants.BusybodyConfigFileName);
                config.WriteToFile(configFilePath);

                _testEventLogReader = new TestEventLogReader();
                _testEventLogReader.ClearLog();

                using (var busybodyConsoleRunner = new BusybodyConsoleRunner(testDirectory.RootPath))
                {
                    _testEventLogReader.WaitForText("Started successfully");
                }
            }
        }

        [Test]
        public void It_should_start_successfully_with_no_errors()
        {
            _testEventLogReader.AnyErrors().Should().BeFalse();
        }

        [Test]
        public void A_log_file_should_be_written()
        {
            var logFilePath = Path.Combine(CommonPaths.BusybodyTemp(), "Logs", "Debug.log");
            Assert.That(File.Exists(logFilePath));
        }

        //Todo: How to clean up after every test
        //Todo: Figure out how to run with ReSharper shadow-copy DLLs
    }

    public class TestEventLogReader
    {
        string _eventLogFilePath;

        public TestEventLogReader()
        {
            _eventLogFilePath = CommonPaths.EventLogFilePath();
        }

        public void ClearLog()
        {
            if(File.Exists(_eventLogFilePath))
                File.Delete(_eventLogFilePath);
        }

        public void WaitForText(string text)
        {
            var startTime = DateTime.Now;
            while (true)
            {
                if(File.Exists(_eventLogFilePath))
                { 
                    var containsText = File.ReadAllText(_eventLogFilePath).Contains(text);
                    if (containsText)
                        return;
                }
                var timeSinceStart = DateTime.Now - startTime;
                if (timeSinceStart > TimeSpan.FromSeconds(5))
                    Assert.Fail("Failed waiting for <<" + text + ">>");
                Thread.Sleep(500);
            }
        }

        public bool AnyErrors()
        {
            return File.ReadAllLines(_eventLogFilePath).Contains("ERROR");
        }
    }

    public class BusybodyConsoleRunner : IDisposable
    {
        readonly Process _process;

        public BusybodyConsoleRunner(string workingDirectory)
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var consoleExePath = Path.Combine(assemblyDirectory, "Busybody.exe");
            var processStartInfo = new ProcessStartInfo(consoleExePath)
            {
                WorkingDirectory = workingDirectory,
            };
            _process = Process.Start(processStartInfo);
        }

        public void Dispose()
        {
            try
            {
                _process.Kill();
            }
            catch // Intentional
            {
            }
        }
    }

    public class TestDirectory : IDisposable
    {
        public string RootPath { get; private set; }

        public TestDirectory()
        {
            RootPath = Path.Combine(Path.GetTempPath(), "Busybody", CommonPaths.RandomName());
            Directory.CreateDirectory(RootPath);
            Debug.Write(RootPath);
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(RootPath, true);
            }
            catch // Intentional
            {
            }
        }

        public string FilePathFor(string configFileName)
        {
            return Path.Combine(RootPath, configFileName);
        }
    }

    public class ConfigBuilder
    {
        public List<HostConfig> Hosts { get; set;  }

        public ConfigBuilder()
        {
            Hosts = new List<HostConfig>();
        }

        public HostConfigBuilder WithHost(string nickname, string hostname)
        {
            var hostConfigBuilder = new HostConfigBuilder(this, nickname, hostname);
            return hostConfigBuilder;
        }

        public BusybodyConfig BuildConfig()
        {
            return new BusybodyConfig
            {
                Hosts = Hosts,
            };
        }
    }

    public class BusybodyConfig
    {
        public List<HostConfig> Hosts = new List<HostConfig>();

        public BusybodyConfig()
        {
            Hosts = new List<HostConfig>();
        }

        public void WriteToFile(string filename)
        {
            var configFileWriter = new ConfigFileWriter();
            configFileWriter.Write(this, filename);
        }
    }

    public class ConfigFileWriter
    {
        public void Write(BusybodyConfig busybodyConfig, string filename)
        {
            var sb = new StringBuilder();
            sb.AppendLine("#Busybody config");
            sb.AppendLine("Hosts");
            foreach (var host in busybodyConfig.Hosts)
            {
                sb.AppendLine("Host");
                sb.AppendLine("\tNickname " + host.Nickname);
                sb.AppendLine("\tHostname " + host.Hostname);
            }
        }
    }


    public class HostConfigBuilder
    {
        readonly ConfigBuilder _configBuilder;
        readonly string _nickname;
        readonly string _hostname;
        List<TestConfig> Tests { get; set; }

        public HostConfigBuilder(ConfigBuilder configBuilder, string nickname, string hostname)
        {
            Tests = new List<TestConfig>();
            _configBuilder = configBuilder;
            _nickname = nickname;
            _hostname = hostname;
        }

        public ConfigBuilder BuildHostConfig()
        {
            var hostConfig = new HostConfig
            {
                Nickname = _nickname,
                Hostname = _hostname,
            };
            _configBuilder.Hosts.Add(hostConfig);
            return _configBuilder;
        }

        public HostConfigBuilder WithTest(TestConfig testConfig)
        {
            Tests.Add(testConfig);
            return this;
        }
    }

    public class HostConfig
    {
        public string Hostname { get; set; }
        public string Nickname { get; set; }
    }

    public class TestConfig
    {
    }

    public class PingTestConfig : TestConfig
    {
    }
}