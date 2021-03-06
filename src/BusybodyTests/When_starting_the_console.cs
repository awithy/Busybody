﻿using System.IO;
using Busybody;
using Busybody.Config;
using BusybodyTests.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace BusybodyTests
{
    [Category("LongRunning")]
    [TestFixture]
    public class Given_an_alive_host_ping_test_when_console_started
    {
        TestEventLogReader _testEventLogReader;
        bool _upEventReceived;

        [SetUp]
        public void Execute()
        {
            using (var testDirectory = new TestDirectory())
            {
                ConsoleContextHelper.BuildAndWriteConfigFile(testDirectory.FilePathFor(SharedConstants.BusybodyConfigFileName));

                TestUtility.DeleteDirectoryWithRetries(CommonPaths.LogsPath());

                _testEventLogReader = new TestEventLogReader();
                _testEventLogReader.ClearEventLog();

                using (var busybodyConsoleRunner = new BusybodyConsoleRunner(testDirectory.RootPath))
                {
                    busybodyConsoleRunner.Start();
                    _upEventReceived = _testEventLogReader.WaitForEvent("Host: Local Machine, State: UP");
                }
            }
        }

        [Category("TestMe")]
        [Test]
        public void The_log_file_should_not_contain_any_errors()
        {
            var logFilePath = Path.Combine(CommonPaths.BusybodyData(), "Logs", "Info.log");
            var logFileContents = File.ReadAllText(logFilePath);
            logFileContents.Should().NotContain("ERROR");
        }

        [Test]
        public void Log_files_should_be_written()
        {
            Assert.That(File.Exists(Path.Combine(CommonPaths.BusybodyData(), "Logs", "Info.log")));
            Assert.That(File.Exists(Path.Combine(CommonPaths.BusybodyData(), "Logs", "Debug.log")));
            Assert.That(File.Exists(Path.Combine(CommonPaths.BusybodyData(), "Logs", "Trace.log")));
        }

        [Test]
        public void An_event_should_be_published_with_the_host_status_up()
        {
            _upEventReceived.Should().BeTrue();
        }
    }

    [Category("LongRunning")]
    [TestFixture]
    public class When_starting_the_console_with_a_config_file_location_parameter
    {
        TestEventLogReader _testEventLogReader;

        [SetUp]
        public void Execute()
        {
            using (var testDirectory = new TestDirectory())
            {
                var configFilePath = Path.GetTempFileName();
                ConsoleContextHelper.BuildAndWriteConfigFile(configFilePath);

                TestUtility.DeleteDirectoryWithRetries(CommonPaths.LogsPath());

                _testEventLogReader = new TestEventLogReader();
                _testEventLogReader.ClearEventLog();

                using (var busybodyConsoleRunner = new BusybodyConsoleRunner(testDirectory.RootPath, configFilePath))
                {
                    busybodyConsoleRunner.Start();
                    _testEventLogReader.WaitForEvent("Startup complete");
                }
            }
        }

        [Test]
        public void The_log_file_should_not_contain_any_errors()
        {
            var logFilePath = Path.Combine(CommonPaths.BusybodyData(), "Logs", "Info.log");
            var logFileContents = File.ReadAllText(logFilePath);
            logFileContents.Should().NotContain("ERROR");
        }

        [Test]
        public void An_event_should_be_published_with_the_host_status_up()
        {
            _testEventLogReader.WaitForEvent("Host: Local Machine, State: UP");
        }
    }

    public static class ConsoleContextHelper
    {
        public static void BuildAndWriteConfigFile(string configFilePath)
        {
            var config = new ConfigBuilder()
                .WithPollingInterval(1)
                .WithHost("Local Machine", "127.0.0.1", "Location 1")
                .WithTest(new HostTestConfig("Ping"))
                .BuildHostConfig()
                .BuildConfig();

            AppContext.Instance = new AppContext(config);
            config.WriteToFile(configFilePath);
        }
    }
}