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

        public void Error(string message, string detail)
        {
            _log.Error(message);
            _HandleError(message, "Error", null);
            AppContext.Instance.EventBus.Publish("All", new SystemErrorEvent(message, detail));
        }

        public void Error(Exception ex, string messageFormat, params object[] formatObjects)
        {
            Error(ex, string.Format(messageFormat, formatObjects));
        }

        public void Error(Exception ex, string message)
        {
            _log.Error(message, ex);
            _HandleError(message, "Error", ex);
            if (AppContext.Instance == null)
            {
                CriticalFailFast(new Exception("Failed to initialize correctly.  AppContext null", ex), "Filaed to initialize correctly.  Inner message:" + message);
                return;
            }
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
            var message = string.Format(messageFormat, formatObjects);
            Error(ex, message);
            Environment.FailFast(message);
        }

        public void CriticalFailFast(Exception ex, string message)
        {
            _log.Critical(message, ex);
            _HandleError(message, "Critical", ex);
            Environment.FailFast(message);
        }

        static void _HandleError(string message, string level, Exception ex = null)
        {
            var errorReportContents = _BuildErrorMessage(message, level, ex);
            _WriteErrorReportFile(level, errorReportContents);
        }

        static string _BuildErrorMessage(string message, string level, Exception ex = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("{0} occurred at:{1}", level, DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss")));
            sb.AppendLine("Message:" + message);
            if(ex != null)
                sb.AppendLine("Detail:" + ex);
            var errorReportContents = sb.ToString();
            return errorReportContents;
        }

        static void _WriteErrorReportFile(string level, string errorReportContents)
        {
            if (CommonPaths.BusybodyData() != null)
            {
                var errorReportDirectory = Path.Combine(CommonPaths.BusybodyData(), "Errors");
                Directory.CreateDirectory(errorReportDirectory);
                var fileName = string.Format("{0}-{1}-{2}.log", DateTime.Now.ToString("yyyyMMdd-HHmmss"), level, Guid.NewGuid().ToString("N").Substring(0, 5));
                var errorReportPath = Path.Combine(errorReportDirectory, fileName);
                File.WriteAllText(errorReportPath, errorReportContents);
            }
            else
            {
                Console.WriteLine("Trying to write error report and config not available");
                Console.WriteLine(errorReportContents);
            }
        }
    }
}