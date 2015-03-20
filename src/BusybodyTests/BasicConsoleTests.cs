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
            _testEventLogReader.WaitForEvent("Host: Local Machine, State: UP");
        }

        //Todo: How to clean up after every test
        //Todo: Figure out how to run with ReSharper shadow-copy DLLs
    }
}