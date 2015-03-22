using System.IO;
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
            using (var testDirectory = new TestDirectory())
            {
                _BuildAndWriteConfigFile(testDirectory.FilePathFor(SharedConstants.BusybodyConfigFileName));

                _ClearLogs();

                _testEventLogReader = new TestEventLogReader();
                _testEventLogReader.ClearEventLog();

                using (var busybodyConsoleRunner = new BusybodyConsoleRunner(testDirectory.RootPath))
                {
                    busybodyConsoleRunner.Start();
                    _testEventLogReader.WaitForEvent("Startup complete");
                }
            }
        }

        [Test]
        public void The_log_file_should_not_contain_any_errors()
        {
            var logFilePath = Path.Combine(CommonPaths.BusybodyData(), "Logs", "Trace.log");
            var logFileContents = File.ReadAllText(logFilePath);
            logFileContents.Should().NotContain("ERROR");
        }

        [Test]
        public void A_log_file_should_be_written()
        {
            var logFilePath = Path.Combine(CommonPaths.BusybodyData(), "Logs", "Trace.log");
            Assert.That(File.Exists(logFilePath));
        }

        [Test]
        public void An_event_should_be_published_with_the_host_status_up()
        {
            _testEventLogReader.WaitForEvent("Host: Local Machine, State: UP");
        }

        static void _ClearLogs()
        {
            var debugLogPath = CommonPaths.LogFilePath("Trace");
            if (File.Exists(debugLogPath))
                File.Delete(debugLogPath);
        }

        static void _BuildAndWriteConfigFile(string configFilePath)
        {
            var config = new ConfigBuilder()
                .WithPollingInterval(1)
                .WithHost("Local Machine", "127.0.0.1")
                .WithTest(new HostTestConfig("Ping"))
                .BuildHostConfig()
                .BuildConfig();

            AppContext.Instance = new AppContext();
            AppContext.Instance.Config = config;
            config.WriteToFile(configFilePath);
        }

        //Todo: Figure out how to run with ReSharper shadow-copy DLLs
    }
}