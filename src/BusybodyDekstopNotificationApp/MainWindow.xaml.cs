using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;

namespace BusybodyDekstopNotificationApp
{
    public partial class MainWindow : Window
    {
        NotifyIcon _notifyIcon;
        string _upIconPath;
        string _downIconPath;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            _notifyIcon = new NotifyIcon();
            _upIconPath = Path.Combine(Environment.CurrentDirectory, "Icons", "app.ico");
            _downIconPath = Path.Combine(Environment.CurrentDirectory, "Icons", "down.ico");
            _upArrowIconPath = Path.Combine(Environment.CurrentDirectory, "Icons", "uparrow.ico");
            _downArrowIconPath = Path.Combine(Environment.CurrentDirectory, "Icons", "downarrow.ico");
            _notifyIcon.Icon = new Icon(_upIconPath);
            _notifyIcon.Visible = true;

            var contextMenuStrip = new ContextMenuStrip();
            var exitMenuItem = new ToolStripMenuItem("Exit");
            exitMenuItem.Click += _OnExitMenuItemClick;
            var separator = new ToolStripSeparator();
            var busybodySystem1 = new ToolStripMenuItem("99 Miller", Image.FromFile(_upArrowIconPath), _OnChangeIconClick);
            var busybodySystem2 = new ToolStripMenuItem("Home", Image.FromFile(_downArrowIconPath), _OnChangeIconClick);
            contextMenuStrip.Items.Add(busybodySystem1);
            contextMenuStrip.Items.Add(busybodySystem2);
            contextMenuStrip.Items.Add(separator);
            contextMenuStrip.Items.Add(exitMenuItem);

            _notifyIcon.ContextMenuStrip = contextMenuStrip;

            base.OnInitialized(e);
        }

        protected void _OnExitMenuItemClick(object sender, EventArgs eventArgs)
        {
            _notifyIcon.Visible = false;
            Environment.Exit(0);
        }

        protected bool _up = true;
        string _upArrowIconPath;
        string _downArrowIconPath;

        protected void _OnChangeIconClick(object sender, EventArgs eventArgs)
        {
            _up = !_up;
            if (_up)
            {
                _notifyIcon.Icon = new Icon(_upIconPath);
            }
            else
            {
                _notifyIcon.Icon = new Icon(_downIconPath);
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _notifyIcon.Visible = false;
            base.OnClosing(e);
        }
    }
}
