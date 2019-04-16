using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Runtime.InteropServices;

namespace MouseTracker {
    /// <summary>
    /// Interakční logika pro App.xaml
    /// </summary>
    public partial class App : Application {
        private System.Windows.Forms.NotifyIcon _notifyIcon;
        public System.Windows.Input.MouseButtonState RightButton { get; }
        private bool _isExit;
        public const UInt32 SPI_SETMOUSESPEED = 0x0071;
        [DllImport("user32.dll")]
        static extern Boolean SystemParametersInfo(
            UInt32 uiAction,
            UInt32 uiParam,
            UInt32 pvParam,
            UInt32 fWinIni);

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow = new MainWindow();
            MainWindow.Closing += MainWindow_Closing;

            _notifyIcon = new System.Windows.Forms.NotifyIcon();
            //_notifyIcon.Click += (s, args) => ShowMainWindow();
            _notifyIcon.MouseDown += new System.Windows.Forms.MouseEventHandler(MouseClick);
            _notifyIcon.Icon = MouseTracker.Properties.Resources.Icon1;
            _notifyIcon.Visible = true;
            CreateContextMenu();

            SystemParametersInfo(SPI_SETMOUSESPEED, 0, 10, 0);
            
        }

        public void MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Right)
            {
                ShowMainWindow();
            }
        }

        public static Point GetMousePosition() {
            System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
            return new Point(point.X, point.Y);
        }

        private void CreateContextMenu()
        {
            _notifyIcon.ContextMenuStrip =
              new System.Windows.Forms.ContextMenuStrip();
            _notifyIcon.ContextMenuStrip.Items.Add("Exit").Click += (s, e) => ExitApplication();
        }

        private void ExitApplication()
        {
            _isExit = true;
            MainWindow.Close();
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }

        private void ShowMainWindow() {
                MainWindow.Top = GetMousePosition().Y - 450;
                MainWindow.Left = GetMousePosition().X - 200;
                MainWindow.Show();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!_isExit)
            {
                e.Cancel = true;
                MainWindow.Hide(); // A hidden window can be shown again, a closed one not
            }
        }
    }
}

