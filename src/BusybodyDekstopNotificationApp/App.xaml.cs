using System;
using System.IO;
using System.Reflection;
using System.Windows;
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message + Environment.NewLine + Environment.NewLine + "Detail:" + ex.StackTrace, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }
        }
    }
}