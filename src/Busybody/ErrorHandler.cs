using System;
using System.IO;
using System.Text;
using Busybody.Events;

namespace Busybody
{
    public interface IErrorHandler
    {
        void Error(Exception ex, string messageFormat, params object[] formatObjects);
        void Error(Exception ex, string message);
        void Critical(Exception ex, string messageFormat, params object[] formatObjects);
        void Critical(Exception ex, string message);
    }

    public class ErrorHandler : IErrorHandler
    {
        static Logger _log = new Logger(typeof(ErrorHandler));

        public void Error(Exception ex, string messageFormat, params object[] formatObjects)
        {
            Error(ex, string.Format(messageFormat, formatObjects));
        }

        public void Error(Exception ex, string message)
        {
            _log.Error(message, ex);
            _HandleError(message, "Error", ex);
            AppContext.Instance.EventBus.Publish("All", new SystemErrorEvent(message, ex.ToString()));
        }

        public void Critical(Exception ex, string messageFormat, params object[] formatObjects)
        {
            Error(ex, string.Format(messageFormat, formatObjects));
        }

        public void Critical(Exception ex, string message)
        {
            _log.Critical(message, ex);
            _HandleError(message, "Critical", ex);
        }

        public void CriticalFailFast(Exception ex, string messageFormat, params object[] formatObjects)
        {
            Error(ex, string.Format(messageFormat, formatObjects));
        }

        public void CriticalFailFast(Exception ex, string message)
        {
            _log.Critical(message, ex);
            _HandleError(message, "Critical", ex);
            Environment.FailFast(message);
        }

        static void _HandleError(string message, string level, Exception ex)
        {
            var errorReportContents = _BuildErrorMessage(message, level, ex);
            _WriteErrorReportFile(level, errorReportContents);
        }

        static string _BuildErrorMessage(string message, string level, Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("{0} occurred at:{1}", level, DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss")));
            sb.AppendLine("Message:" + message);
            sb.AppendLine("Detail:" + ex);
            var errorReportContents = sb.ToString();
            return errorReportContents;
        }

        static void _WriteErrorReportFile(string level, string errorReportContents)
        {
            var errorReportDirectory = Path.Combine(CommonPaths.BusybodyData(), "Errors");
            Directory.CreateDirectory(errorReportDirectory);
            var fileName = string.Format("{0}-{1}-{2}.log", DateTime.Now.ToString("yyyyMMdd-HHmmss"), level, Guid.NewGuid().ToString("N").Substring(0, 5));
            var errorReportPath = Path.Combine(errorReportDirectory, fileName);
            File.WriteAllText(errorReportPath, errorReportContents);
        }
    }
}