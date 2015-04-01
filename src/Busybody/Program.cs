using System;
using System.IO;
using Busybody.Config;
using Topshelf;

namespace Busybody
{
    class Program
    {
        //TODO: Busybody process memory monitoring
        //TODO: Report on average ping latency
        //TODO: E-mail report of activity

        static Logger _log = new Logger(typeof(Program));
        static bool _verboseLogging;
        static string _configFilePathParameter;

        static int Main()
        {
            try
            {
                var serviceHost = _CreateHost();
                _ReadConfigAndSetupAppContext();
                Directory.CreateDirectory(CommonPaths.BusybodyData());
                _SetupLogging();
                _log.Info("Starting Busybody v0.1");
                _HandleUnhandledExceptions();
                serviceHost.Run();
                return 0;
            }
            catch (Exception ex)
            {
                new ErrorHandler().CriticalFailFast(ex, "Failing fast due to unexpected exception of type: {0}", ex.GetType().Name);
                return -1;
            }
        }

        static void _ReadConfigAndSetupAppContext()
        {
            try
            {
                AppContext.Instance = new AppContext();
                var configFilePath = CommonPaths.CurrentConfigFilePath();

                if (_configFilePathParameter != null)
                {
                    if (!File.Exists(_configFilePathParameter))
                    {
                        throw new ConfigFileNotFoundException(_configFilePathParameter);
                    }
                    configFilePath = _configFilePathParameter;
                }

                AppContext.Instance.Config = BusybodyConfig.ReadFromFile(configFilePath);
            }
            catch (Exception ex)
            {
                throw new ConfigFileReadException(ex);
            }
        }

        static Topshelf.Host _CreateHost()
        {
            _log.Trace("Creating host");
            var serviceHost = HostFactory.New(x =>
            {
                x.AfterInstall(() => _log.Info("The Busybody service has been installed."));
                x.BeforeUninstall(() => _log.Info("The Busybody service is being uninstalled."));
                x.RunAsLocalSystem();
                x.SetDescription("Busybody");
                x.SetDisplayName("Busybody");
                x.SetServiceName("Busybody");
                x.AddCommandLineSwitch("v", verboseLogging => _verboseLogging = verboseLogging);
                x.AddCommandLineDefinition("c", configFilePathParameter => _configFilePathParameter = configFilePathParameter);

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
            LogSetup.Setup(CommonPaths.LogsPath(), _verboseLogging);
            if(_configFilePathParameter != null)
                _log.Debug("Overridden config file specified:" + _configFilePathParameter);
        }

        static void _HandleUnhandledExceptions()
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Exception ex = null;
                if (args != null && args.ExceptionObject != null)
                    ex = (Exception)args.ExceptionObject;
                new ErrorHandler().CriticalFailFast(ex, "Failing fast due to exception of type: {0}", ex.GetType().Name);
            };
        }
    }

    public class ConfigFileNotFoundException : Exception
    {
        public ConfigFileNotFoundException(string configFilePathParameter) : base(string.Format("Config file not found at " + configFilePathParameter))
        {
        }
    }

    internal class ConfigFileReadException : Exception
    {
        public ConfigFileReadException(Exception exception) : base("Failed to read config file.", exception)
        {
        }
    }
}