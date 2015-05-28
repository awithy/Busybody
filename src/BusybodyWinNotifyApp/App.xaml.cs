using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Newtonsoft.Json;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace BusybodyDekstopNotificationApp
{
    public partial class App : Application
    {
        public App()
        {
            try
            {
                var configFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "BusybodyNotification.cfg");
                var configFileText = File.ReadAllText(configFilePath);
                var config = JsonConvert.DeserializeObject<BusybodyNotificationConfig>(configFileText);
                AppContext.Instance = new AppContext(config);

                Application.Current.DispatcherUnhandledException += _UnhandledException;
                AppDomain.CurrentDomain.UnhandledException += _AppDomainUnhandled;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message + Environment.NewLine + Environment.NewLine + "Detail:" + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }

        void _AppDomainUnhandled(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("App Domain Unhandled Exception: " + e.ExceptionObject);
        }

        void _UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Dispatcher Unhandled Exception: " + e.Exception);
        }
    }
}