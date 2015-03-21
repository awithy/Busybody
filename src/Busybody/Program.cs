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
            //TODO: Handle unhandled exceptions

            var host = _ConfigureServiceHost();

            var busybodyTempPath = CommonPaths.BusybodyTemp();
            Directory.CreateDirectory(busybodyTempPath);

            _SetupLogging();

            _log = new Logger(typeof (Program));
            try
            {
                _log.Info("Starting Busybody v0.1");

                AppContext.Instance = new AppContext();
                var configFilePath = CommonPaths.CurrentConfigFilePath();
                AppContext.Instance.Config = BusybodyConfig.ReadFromFile(configFilePath);

                _log.Debug("Running host");
                host.Run();

                return 0;
            }
            catch (Exception ex)
            {
                _log.Error("Unexpected " + ex.GetType().Name + " occurred.  Aborting.  " + Environment.NewLine + ex);
                return -1;
            }
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
    }
}