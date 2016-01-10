using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;

namespace TBID
{
    public partial class MainForm : Form
    {
        private bool IsDownloading = false;

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

        private void ButtonAbout_Click(object sender, EventArgs e)
        {

        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            if (!IsDownloading)
            {
                // TODO: Validate all fields before starting.

                // Validating directory.
                if (!Directory.Exists(TextBoxDownloadDirectory.Text))
                {
                    MessageBox.Show("The specified directory is invalid. Please input a valid directory.", "Invalid directory.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Validating URL.
                // TODO: Consider placing this in a separate thread?
                if (!IsValidURL(TextBoxUsernameBlogLink.Text))
                {
                    // Might be a username, check validity.
                    string UserURL = "http://" + TextBoxUsernameBlogLink.Text + ".tumblr.com/";
                    if (!IsValidURL(UserURL))
                    {
                        MessageBox.Show("The specified URL / username is invalid. Please recheck your input.", "Invalid URL / username.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (!IsExistingURL(UserURL))
                    {
                        MessageBox.Show("An error occured while accessing the blog. This may be because the blog has been deleted or there was an error in your input.", "URL-404 / username not found.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        // Valid username.
                    }
                }
                else
                {
                    // Is a URL.
                    if (!IsExistingURL(TextBoxUsernameBlogLink.Text))
                    {
                        MessageBox.Show("An error occured while accessing the blog. This may be because the blog has been deleted or there was an error in your input.", "URL-404 / username not found.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        // Valid URL.
                    }
                }

                IsDownloading = true;
                ButtonStart.Text = "Stop";
                ModifyControls(false, InvokeRequired);
                // TODO: Begin download.
            }
            else
            {
                // TODO: Stop download / clean.
                ModifyControls(true, InvokeRequired);
                ButtonStart.Text = "Start";
                IsDownloading = false;
            }
        }

        private void ModifyControls(bool Enabled, bool InvokeRequired)
        {
            if (InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate { ModifyControls(Enabled, false); });
            }
            else
            {
                TextBoxUsernameBlogLink.Enabled = Enabled;
                TextBoxTags.Enabled = Enabled;
                TextBoxDownloadDirectory.Enabled = Enabled;
                ButtonBrowse.Enabled = Enabled;
                NumericUpDownDownloadLimit.Enabled = Enabled;
                CheckBoxUseListCache.Enabled = Enabled;
                CheckBoxNotify.Enabled = Enabled;
                TextBoxAPIKey.Enabled = Enabled;
            }
        }

        private void ButtonBrowse_Click(object sender, EventArgs e)
        {
            DialogResult Result = FolderBrowserDialogMain.ShowDialog();
            if (Result == DialogResult.Cancel || Result == DialogResult.Abort || Result == DialogResult.Ignore || Result == DialogResult.No || Result == DialogResult.None)
            {
                return;
            }
            TextBoxDownloadDirectory.Clear();
            TextBoxDownloadDirectory.Text = FolderBrowserDialogMain.SelectedPath;
        }

        #region URL / Webpage Validation

        private bool IsValidURL(string URL)
        {
            Uri uri = null;
            if (!Uri.TryCreate(URL, UriKind.Absolute, out uri) || null == uri)
            {
                return false;
            }
            return true;
        }

        private bool IsExistingURL(string URL)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
                request.Method = "HEAD";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                response.Close();
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
