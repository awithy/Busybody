using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using BusybodyShared;

namespace BusybodyDekstopNotificationApp
{
    public partial class MainWindow : Window
    {
        NotifyIcon _notifyIcon;
        Timer _timer;
        Dictionary<string, ToolStripMenuItem> _systemsMenuItemDictionary;
        Image _upArrowImage;
        Image _downArrowImage;
        Icon _upIcon;
        Icon _downIcon;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            _upIcon = new Icon(Path.Combine(Environment.CurrentDirectory, "Icons", "app.ico"));
            _downIcon = new Icon(Path.Combine(Environment.CurrentDirectory, "Icons", "down.ico"));

            _upArrowImage = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "Icons", "uparrow.ico"));
            _downArrowImage = Image.FromFile(Path.Combine(Environment.CurrentDirectory, "Icons", "downarrow.ico"));

            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = _upIcon;
            _notifyIcon.Visible = true;

            var contextMenuStrip = new ContextMenuStrip();
            var exitMenuItem = new ToolStripMenuItem("Exit");
            exitMenuItem.Click += _OnExitMenuItemClick;
            var separator = new ToolStripSeparator();

            _systemsMenuItemDictionary = new Dictionary<string, ToolStripMenuItem>();
            var systems = AppContext.Instance.Config.Systems;
            foreach (var system in systems)
            {
                var toolStripMenuItem = new ToolStripMenuItem(system.Name, _upArrowImage, _OnSystemClick)
                {
                    Tag = system
                };
                contextMenuStrip.Items.Add(toolStripMenuItem);
                _systemsMenuItemDictionary.Add(system.SystemId, toolStripMenuItem);
            }

            contextMenuStrip.Items.Add(separator);
            contextMenuStrip.Items.Add(exitMenuItem);

            _notifyIcon.ContextMenuStrip = contextMenuStrip;

            _timer = new Timer();
            _timer.Interval = AppContext.Instance.Config.PollingInterval*1000;
            _timer.Tick += _OnTick;
            _timer.Start();
            _OnTick(null, null);

            base.OnInitialized(e);
        }

        void _OnSystemClick(object sender, EventArgs e)
        {
            var senderMenuItem = (ToolStripMenuItem) sender;
            var system = (SystemConfig) senderMenuItem.Tag;
            Process.Start(system.Url);
        }

        void _OnTick(object sender, EventArgs e)
        {
            var notificationMonitor = AppContext.Instance.SystemStatusProvider;
            var systemStatuses = notificationMonitor.GetSystemStatuses().ToArray();
            foreach (var systemStatus in systemStatuses)
            {
                var correspondingMenuItem = _systemsMenuItemDictionary[systemStatus.SystemId];
                correspondingMenuItem.Image = systemStatus.State == AzureStatusState.UP
                    ? _upArrowImage
                    : _downArrowImage;
            }
            _notifyIcon.Icon = systemStatuses.All(x => x.State == AzureStatusState.UP)
                ? _upIcon
                : _downIcon;
        }

        protected void _OnExitMenuItemClick(object sender, EventArgs eventArgs)
        {
            _timer.Stop();
            _notifyIcon.Visible = false;
            Environment.Exit(0);
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _notifyIcon.Visible = false;
            base.OnClosing(e);
        }
    }
}
