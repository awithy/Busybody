using System;
using System.IO;
using Busybody.Config;
using Topshelf;

namespace Busybody
{
    class Program
    {
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
                Console.WriteLine("Fatal exception");                
                Console.WriteLine(ex.ToString());
                Environment.FailFast("Fatal exception");
            }
            return -1;
        }

        static void _Main()
        {
            var host = _ConfigureServiceHost();

            Directory.CreateDirectory(CommonPaths.BusybodyTemp());

            _SetupLogging();

            _log = new Logger(typeof (Program));
            try
            {
                _HandleUnhandledExceptions();

                _log.Info("Starting Busybody v0.1");

                _SetupAppContext();

                _log.Trace("Running host");
                host.Run();
            }
            catch (Exception ex)
            {
                _log.Error("Unexpected " + ex.GetType().Name + " occurred.  Aborting.  " + Environment.NewLine + ex);
                Environment.FailFast("Failing fast due to unexpected exception of type: " + ex.GetType().Name + ".  Detail: " + ex);
            }
        }

        static void _SetupAppContext()
        {
            AppContext.Instance = new AppContext();
            var configFilePath = CommonPaths.CurrentConfigFilePath();
            AppContext.Instance.Config = BusybodyConfig.ReadFromFile(configFilePath);
        }

        static Host _ConfigureServiceHost()
        {
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
            return serviceHost;
        }

        static void _SetupLogging()
        {
            var logsDirectory = CommonPaths.LogsPath();
            Directory.CreateDirectory(logsDirectory);
            var logFilePath = CommonPaths.LogFilePath("Debug");
            LogSetup.Setup(logFilePath, _verboseLogging);
        }

        static void _HandleUnhandledExceptions()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                _log.Error("Unhandled exception occurred.  Type: " + args.ExceptionObject.GetType().Name + ".  Detail: " + args.ExceptionObject.ToString());
                Environment.FailFast("Failing fast due to exception of type: " + args.ExceptionObject.GetType().Name);
            };
        }
    }
}