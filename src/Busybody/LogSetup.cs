using System;
using log4net;
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

        public void Debug(string message)
        {
            _log.Debug(message);
        }

        public void Error(string message)
        {
            ConsoleLog.Log("ERROR:" + message);
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

    public static class ConsoleLog
    {
        public static void Log(string message)
        {
            var logMessage = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + message;
            Console.WriteLine(logMessage);
        }
    }

    public class LogSetup
    {
        static readonly PatternLayout _layout;

        static LogSetup()
        {
            _layout = new PatternLayout("[%d{HH:mm:ss}][%level][%logger][%thread] %message%newline");
        }

        public static void Setup(string logFile)
        {
            log4net.Config.BasicConfigurator.Configure();
            var repository = (Hierarchy) LogManager.GetRepository();
            repository.Root.RemoveAllAppenders();
            repository.Root.AddAppender(_GetFileAppender(logFile, Level.All, true));
            //repository.Root.AddAppender(new ConsoleAppender {Layout = _layout});
            repository.Root.Level = Level.All;
        }

        private static log4net.Appender.FileAppender _GetFileAppender(string sFileName , Level threshhold ,bool bFileAppend)
        {
            var lAppender = new log4net.Appender.FileAppender();
            lAppender.Name = sFileName;
            lAppender.AppendToFile = bFileAppend;
            lAppender.File = sFileName;
            lAppender.Layout = _layout;
            lAppender.Threshold = threshhold;
            lAppender.ActivateOptions();
            return lAppender;
        }
    }
}