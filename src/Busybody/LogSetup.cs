using System;
using System.Diagnostics;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace Busybody
{
    public class Logger
    {
        readonly ILog _log;

        public Logger(Type type)
        {
            _log = LogManager.GetLogger(type);
        }

        public void Trace(string message)
        {
            _log.Debug(message); //TODO: Switch to using Trace level
        }

        public void Debug(string message)
        {
            _log.Debug(message);
        }

        public void Error(string message)
        {
            _log.Error(message);
        }

        public void Info(string message)
        {
            _log.Info(message);
        }

        public void Warn(string message)
        {
            _log.Warn(message);
        }
    }

    public class LogSetup
    {
        static readonly PatternLayout _layout;

        static LogSetup()
        {
            _layout = new PatternLayout("[%d{HH:mm:ss}][%level][%logger][%thread] %message%newline");
        }

        public static void Setup(string logFile, bool verboseLogging)
        {
            log4net.Config.BasicConfigurator.Configure();
            var repository = (Hierarchy) LogManager.GetRepository();
            repository.Root.RemoveAllAppenders();
            repository.Root.AddAppender(_GetFileAppender(logFile, Level.All, true));
            if (Process.GetCurrentProcess().ProcessName.ToLower().Contains("busybody")) //Mute logs when running from test runner.
                repository.Root.AddAppender(_GetConsoleAppender(Level.All, verboseLogging));
            repository.Root.Level = Level.All;
        }

        private static ManagedColoredConsoleAppender _GetConsoleAppender(Level threshhold, bool verboseLogging)
        {
            var appender = new ManagedColoredConsoleAppender();
            appender.Name = "Console Appender";
            appender.Layout = _layout;
            appender.Threshold = threshhold;
            if(verboseLogging) //Use default unless debug is enabled, and then highlight with green
                appender.AddMapping(new ManagedColoredConsoleAppender.LevelColors{ Level = Level.Info, ForeColor = ConsoleColor.Green });
            appender.AddMapping(new ManagedColoredConsoleAppender.LevelColors {Level = Level.Debug, ForeColor = ConsoleColor.Gray});
            appender.AddMapping(new ManagedColoredConsoleAppender.LevelColors {Level = Level.Warn, ForeColor = ConsoleColor.Yellow});
            appender.AddMapping(new ManagedColoredConsoleAppender.LevelColors {Level = Level.Error, ForeColor = ConsoleColor.Red});
            appender.AddMapping(new ManagedColoredConsoleAppender.LevelColors {Level = Level.Critical, ForeColor = ConsoleColor.Red});
            appender.ActivateOptions();
            return appender;
        }

        private static FileAppender _GetFileAppender(string sFileName , Level threshhold ,bool bFileAppend)
        {
            var appender = new FileAppender();
            appender.Name = sFileName;
            appender.AppendToFile = bFileAppend;
            appender.File = sFileName;
            appender.Layout = _layout;
            appender.Threshold = threshhold;
            appender.ActivateOptions();
            return appender;
        }
    }
}