using System;
using System.Diagnostics;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace BusybodyShared
{
    public class Logger
    {
        readonly ILog _log;
        readonly Type _sourceType;

        public Logger(Type type)
        {
            _sourceType = type;
            _log = LogManager.GetLogger(type);
        }

        public void TraceFormat(string message, params object[] formatObjects)
        {
            Trace(string.Format(message, formatObjects));
        }

        public void Trace(string message)
        {
            _log.Logger.Log(_sourceType, Level.Trace, message, null);
        }

        public void DebugFormat(string message, params object[] formatObjects)
        {
            Debug(string.Format(message, formatObjects));
        }

        public void Debug(string message)
        {
            _log.Debug(message);
        }

        public void InfoFormat(string message, params object[] formatObjects)
        {
            Info(string.Format(message, formatObjects));
        }

        public void Info(string message)
        {
            _log.Info(message);
        }

        public void WarnFormat(string message, params object[] formatObjects)
        {
            Warn(string.Format(message, formatObjects));
        }

        public void Warn(string message)
        {
            _log.Warn(message);
        }

        public void ErrorFormat(Exception exception, string message, params object[] formatObjects)
        {
            Error(string.Format(message, formatObjects), exception);
        }

        public void Error(string message, Exception exception = null)
        {
            _log.Error(message);
            if(exception != null)
                _log.Debug(exception.ToString());
        }

        public void CriticalFormat(Exception exception, string message, params object[] formatObjects)
        {
            Critical(string.Format(message, formatObjects), exception);
        }

        public void Critical(string message, Exception exception = null)
        {
            _log.Logger.Log(_sourceType, Level.Critical, message, null);
            if(exception != null)
                _log.Debug(exception.ToString());
        }
    }

    public class LogSetup
    {
        static readonly PatternLayout _layout;

        static LogSetup()
        {
            _layout = new PatternLayout("[%d{HH:mm:ss}][%level][%logger][%thread] %message%newline");
        }

        public static void Setup(string logsDirectory, bool verboseLogging)
        {
            log4net.Config.BasicConfigurator.Configure();
            var repository = (Hierarchy) LogManager.GetRepository();
            repository.Root.RemoveAllAppenders();
            repository.Root.AddAppender(_GetFileAppender(Path.Combine(logsDirectory, "Info.log"), Level.Info, true));
            repository.Root.AddAppender(_GetFileAppender(Path.Combine(logsDirectory, "Debug.log"), Level.Debug, true));
            repository.Root.AddAppender(_GetFileAppender(Path.Combine(logsDirectory, "Trace.log"), Level.Trace, true));
            if (Process.GetCurrentProcess().ProcessName.ToLower().Contains("busybody")) //Mute logs when running from test runner.
                repository.Root.AddAppender(_GetConsoleAppender(verboseLogging));
            repository.Root.Level = Level.All;
        }

        private static ManagedColoredConsoleAppender _GetConsoleAppender(bool verboseLogging)
        {
            var appender = new ManagedColoredConsoleAppender();
            appender.Name = "Console Appender";
            appender.Layout = _layout;

            if(verboseLogging) //Use default unless verbose console logging is enabled, and then highlight with green
            {
                appender.Threshold = Level.Debug;
                appender.AddMapping(new ManagedColoredConsoleAppender.LevelColors{ Level = Level.Info, ForeColor = ConsoleColor.Green });
            }
            else
            {
                appender.Threshold = Level.Info;
            }

            appender.AddMapping(new ManagedColoredConsoleAppender.LevelColors {Level = Level.Debug, ForeColor = ConsoleColor.Gray});
            appender.AddMapping(new ManagedColoredConsoleAppender.LevelColors {Level = Level.Warn, ForeColor = ConsoleColor.Yellow});
            appender.AddMapping(new ManagedColoredConsoleAppender.LevelColors {Level = Level.Error, ForeColor = ConsoleColor.Red});
            appender.AddMapping(new ManagedColoredConsoleAppender.LevelColors {Level = Level.Critical, ForeColor = ConsoleColor.Red});
            appender.ActivateOptions();
            return appender;
        }

        private static FileAppender _GetFileAppender(string fileName, Level threshhold, bool append)
        {
            var appender = new RollingFileAppender();
            appender.Name = threshhold.Name;
            appender.AppendToFile = append;
            appender.File = fileName;
            appender.Layout = _layout;
            appender.Threshold = threshhold;
            appender.RollingStyle = RollingFileAppender.RollingMode.Size;
            appender.MaximumFileSize = "20MB";
            appender.MaxSizeRollBackups = 3;
            appender.PreserveLogFileNameExtension = true;
            appender.ActivateOptions();
            return appender;
        }
    }
}