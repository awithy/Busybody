using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Busybody;
using Busybody.Config;
using FluentAssertions;
using NUnit.Framework;

namespace BusybodyTests
{
    [TestFixture]
    public class Given_an_alive_host_ping_test_when_console_started
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

                var debugLogPath = CommonPaths.LogFilePath("Debug");
                if (File.Exists(debugLogPath))
                    File.Delete(debugLogPath);

                _testEventLogReader = new TestEventLogReader();
                _testEventLogReader.ClearEventLog();

                using (var busybodyConsoleRunner = new BusybodyConsoleRunner(testDirectory.RootPath))
                {
                    _testEventLogReader.WaitForEvent("Startup complete");
                }
            }
        }

        [Test]
        public void The_log_file_should_not_contain_any_errors()
        {
            var logFilePath = Path.Combine(CommonPaths.BusybodyTemp(), "Logs", "Debug.log");
            var logFileContents = File.ReadAllText(logFilePath);
            logFileContents.Should().NotContain("ERROR");
        }

        [Test]
        public void A_log_file_should_be_written()
        {
            var logFilePath = Path.Combine(CommonPaths.BusybodyTemp(), "Logs", "Debug.log");
            Assert.That(File.Exists(logFilePath));
        }

        [Test]
        public void An_event_should_be_published_with_the_host_status_up()
        {
            _testEventLogReader.WaitForEvent("Host: Local Machine, State: Up");
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

        public void ClearEventLog()
        {
            if(File.Exists(_eventLogFilePath))
                File.Delete(_eventLogFilePath);
        }

        //This crap is just tempoary until I do this properly
        public void WaitForEvent(string text)
        {
            var startTime = DateTime.Now;
            while (true)
            {
                if(File.Exists(_eventLogFilePath))
                {
                    var allText = File.ReadAllText(_eventLogFilePath);
                    var containsText = allText.Contains(text);
                    if (containsText)
                        return;
                }
                var timeSinceStart = DateTime.Now - startTime;
                if (timeSinceStart > TimeSpan.FromSeconds(5))
                    Assert.Fail("Failed waiting for <<" + text + ">>");
                Thread.Sleep(500);
            }
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
                //For now I'd prefer to see what's up with the test directories in event of test failure.  Warning: Disk space could be an issue.
                //Directory.Delete(RootPath, true);
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


    public class HostConfigBuilder
    {
        readonly ConfigBuilder _configBuilder;
        readonly string _nickname;
        readonly string _hostname;
        List<HostTestConfig> Tests { get; set; }

        public HostConfigBuilder(ConfigBuilder configBuilder, string nickname, string hostname)
        {
            Tests = new List<HostTestConfig>();
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
                Tests = Tests,
            };
            _configBuilder.Hosts.Add(hostConfig);
            return _configBuilder;
        }

        public HostConfigBuilder WithTest(HostTestConfig hostTestConfig)
        {
            hostTestConfig.HostNickname = _nickname;
            Tests.Add(hostTestConfig);
            return this;
        }
    }
}