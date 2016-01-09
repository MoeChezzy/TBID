using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TBID
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            InitializeNotifyIcon();

            this.Icon = Properties.Resources.Icon;
            this.TextBoxAPIKey.PasswordChar = '\u25CF';
        }

        #region Notify Icon

        private void InitializeNotifyIcon()
        {
            NotifyIconMain.Icon = Properties.Resources.Icon;
            NotifyIconMain.Text = Program.Name;
            NotifyIconMain.BalloonTipTitle = Program.Name;
        }

        /// <summary>
        /// Creates a notification in the system tray from the notify icon.
        /// </summary>
        /// <param name="duration">The length, in milliseconds, that the notification should last.</param>
        /// <param name="message">The message displayed on the notification.</param>
        /// <param name="icon">The icon displayed on the notification bubble.</param>
        private void Notify(int duration, string message, ToolTipIcon icon)
        {
            NotifyIconMain.ShowBalloonTip(duration, Program.Name, message, icon);
        }

        #endregion
    }
}
