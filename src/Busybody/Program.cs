using System;
using System.IO;
using Busybody.Config;
using Topshelf;

namespace Busybody
{
    class Program
    {
        //TODO: Error handling/reporting/alerting
        //TODO: Test last chance exceptions and unhandled exceptions
        //TODO: Busybody process memory monitoring
        //TODO: Report on average ping latency
        //TODO: Time limit each test to some maximum allowable level then abort

        static Logger _log;
        static bool _verboseLogging;

        static int Main()
        {
            try
            {
                _Main();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fatal last chance exception");                
                Console.WriteLine(ex.ToString());
                Environment.FailFast("Fatal last chance exception " + ex);
            }
            return -1;
        }

        static void _Main()
        {
            try
            {
                _ReadConfigAndSetupAppContext();
                Directory.CreateDirectory(CommonPaths.BusybodyData());
                _SetupLogging();
                _log.Info("Starting Busybody v0.1");
                _HandleUnhandledExceptions();
                _RunHost();
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("Failing fast due to unexpected exception of type: {0}.  Detail: {1}", ex.GetType().Name, ex);
                if (_log != null)
                    _log.Critical(errorMessage, ex);
                else
                    Console.WriteLine(errorMessage);
                Environment.FailFast(errorMessage);
            }
        }

        static void _ReadConfigAndSetupAppContext()
        {
            try
            {
                AppContext.Instance = new AppContext();
                var configFilePath = CommonPaths.CurrentConfigFilePath();
                AppContext.Instance.Config = BusybodyConfig.ReadFromFile(configFilePath);
            }
            catch (Exception ex)
            {
                throw new ConfigFileReadException(ex);
            }
        }

        static void _RunHost()
        {
            _log.Trace("Running host");

            var serviceHost = HostFactory.New(x =>
            {
                x.AfterInstall(() => _log.Info("The Busybody service has been installed."));
                x.BeforeUninstall(() => _log.Info("The Busybody service is being uninstalled."));
                x.RunAsLocalSystem();
                x.SetDescription("Busybody");
                x.SetDisplayName("Busybody");
                x.SetServiceName("Busybody");
                x.AddCommandLineSwitch("v", verboseLogging => _verboseLogging = verboseLogging);

                x.Service<BusybodyDaemon>(s =>
                {
                    x.SetServiceName("Busybody");
                    s.ConstructUsing(name => new BusybodyDaemon());
                    s.WhenStarted(busybodyDaemon => busybodyDaemon.Start());
                    s.WhenStopped(busybodyDaemon => busybodyDaemon.Stop());
                });

            });
            serviceHost.Run();
        }

        static void _SetupLogging()
        {
            var logsDirectory = CommonPaths.LogsPath();
            Directory.CreateDirectory(logsDirectory);
            LogSetup.Setup(CommonPaths.LogsPath(), _verboseLogging);
            _log = new Logger(typeof (Program));
        }

        static void _HandleUnhandledExceptions()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var detail = "";
                if (args != null && args.ExceptionObject != null)
                    detail = args.ExceptionObject.ToString();
                if(_log != null)
                    _log.CriticalFormat(null, "Unhandled critical {0} occurred.  Detail:{1}", args.ExceptionObject.GetType().Name, detail);
                Environment.FailFast("Failing fast due to exception of type: " + args.ExceptionObject.GetType().Name  + "  Detail:" + detail);
            };
        }
    }

    internal class ConfigFileReadException : Exception
    {
        public ConfigFileReadException(Exception exception) : base("Failed to read config file.", exception)
        {
        }
    }
}