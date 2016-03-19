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
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace TBID
{
    public partial class MainForm : Form
    {
        private bool IsDownloading = false;

        private Thread ThreadScrape = null;
        private Thread ThreadDownload = null;

        private Queue<string> LinkQueue = new Queue<string>();

        private List<WebClient> WebClients = new List<WebClient>();

        private Stopwatch Sw = new Stopwatch();

        private ulong DownloadDownloaded = 0;
        private ulong DownloadTotal = 0;

        private const int ResponseLimit = 20;

        public MainForm()
        {
            InitializeComponent();
            InitializeFormClosingEvents();
            InitializeNotifyIcon();

            Icon = Properties.Resources.Icon;
            TextBoxAPIKey.PasswordChar = '\u25CF';
        }

        private void InitializeFormClosingEvents()
        {
            FormClosing += MainForm_FormClosing;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsDownloading)
            {
                DialogResult Confirmation = MessageBox.Show("There is currently a download in progress! Closing " + Program.Name + " will abort the download. Do you still want to exit?", "Download in progress.", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (Confirmation == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    CleanupThreads();
                    Program.Close();
                }
            }
            else
            {
                CleanupThreads();
                Program.Close();
            }
        }

        private void CleanupThreads()
        {
            while (ThreadScrape.IsAlive)
            {
                ThreadDownload.Abort();
            }
            while (ThreadDownload.IsAlive)
            {
                ThreadDownload.Abort();
            }
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
            MessageBox.Show(string.Format("{0} is a portable and lightweight tumblr blog image downloader. It is free and open source, released under the GNU General Public License (version 3).{1}{1}Copyright \u00a9 {2} {3}", Program.Name, Environment.NewLine, DateTime.Now.Year, Program.Author), "About");
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            if (!IsDownloading)
            {
                // Validating directory.
                if (!Directory.Exists(TextBoxDownloadDirectory.Text))
                {
                    MessageBox.Show("The specified directory is invalid. Please input a valid directory.", "Invalid directory.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                // Removing potential trailing backslash.
                if (TextBoxDownloadDirectory.Text[TextBoxDownloadDirectory.Text.Length - 1] == '\\')
                {
                    TextBoxDownloadDirectory.Text = TextBoxDownloadDirectory.Text.Remove(TextBoxDownloadDirectory.Text.Length - 1);
                }

                // Validating URL.
                // We might want to consider putting this check into a separate thread.
                if (!IsValidUrl(TextBoxUsernameBlogLink.Text))
                {
                    // Might be a username, check validity.
                    string UserUrl = "http://" + TextBoxUsernameBlogLink.Text + ".tumblr.com/";
                    if (!IsValidUrl(UserUrl))
                    {
                        MessageBox.Show("The specified URL / username is invalid. Please recheck your input.", "Invalid URL / username.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (!IsExistingUrl(UserUrl))
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
                    if (!IsExistingUrl(TextBoxUsernameBlogLink.Text))
                    {
                        MessageBox.Show("An error occured while accessing the blog. This may be because the blog has been deleted or there was an error in your input.", "URL-404 / username not found.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        // Valid URL.
                    }
                }

                // Validating API key.
                // We'll be checking for the valid length (50 characters).

                if (string.IsNullOrWhiteSpace(TextBoxAPIKey.Text))
                {
                    MessageBox.Show(string.Format("Please enter a valid API key; you cannot use {0} without one.", Program.Name), "Missing API key.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (TextBoxAPIKey.Text.Length != 50)
                {
                    MessageBox.Show("The API key you entered is invalid.", "Invalid API key.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (Properties.Settings.Default.FirstTimeInconsistencies)
                {
                    if (!string.IsNullOrWhiteSpace(TextBoxTags.Text))
                    {
                        // Notify the user about the progress inconsistencies with multiple tags.
                        if (TextBoxTags.Text.Split(',').Length > 1)
                        {
                            MessageBox.Show(string.Format("Because of the limitations of tumblr's API, it is not possible to retrieve the total amount of posts that contain multiple tags in one request. The progress bar may fluctuate and go up or down as {0} recalculates the actual amount of posts containing all of your specified tags.", Program.Name), "Progress bar inconsistencies.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Properties.Settings.Default.FirstTimeInconsistencies = false;
                            Properties.Settings.Default.Save();
                        }
                    }
                }

                IsDownloading = true;
                ButtonStart.Text = "Stop";
                ModifyControls(false);
                LinkQueue.Clear();

                Sw.Reset();
                Sw.Start();

                SetProgress(0);

                ThreadDownload = new Thread(Download);
                ThreadDownload.Start();
            }
            else
            {
                CleanupThreads();
                Sw.Stop();
                UpdateStatus("Download cancelled.");
                SetProgress(0);
                ModifyControls(true);
                ButtonStart.Text = "Start";
                IsDownloading = false;
            }
        }

        private void Download()
        {
            UpdateStatus("Began download thread.");

            DownloadDownloaded = 0;
            DownloadTotal = 0;

            ThreadScrape = new Thread(Scrape);
            ThreadScrape.Start();

            while (ThreadScrape.IsAlive)
            {
                while (LinkQueue.Count > 0)
                {
                    string Url = LinkQueue.Dequeue();
                    Uri Uri = new Uri(Url);
                    WebClients.Add(new WebClient());
                    WebClients.Last().DownloadFileCompleted += WebClient_DownloadFileCompleted;
                    WebClients.Last().DownloadFileAsync(Uri, TextBoxDownloadDirectory.Text + "\\" + GetFilename(Url));
                }
            }

            // Check if any WebClient objects are downloading.
            bool Done = false;
            while (!Done)
            {
                Done = true;
                foreach (WebClient Wc in WebClients.ToList())
                {
                    if (Wc.IsBusy)
                    {
                        Done = false;
                        break;
                    }
                    else
                    {
                        WebClients.Remove(Wc);
                    }
                }
            }

            // Done.
            IsDownloading = false;
            Invoke((MethodInvoker)delegate { ButtonStart.Text = "Start"; });
            Invoke((MethodInvoker)delegate { SetProgress(100); });
            UpdateStatus("Finished downloading. Time elapsed: " + Sw.Elapsed + ".");
            ModifyControls(true);
            LinkQueue.Clear();
            Sw.Stop();
            ThreadDownload.Abort();
            ThreadScrape.Abort();
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            DownloadDownloaded++;
            UpdateStatus("Download progress: " + DownloadDownloaded + "/" + DownloadTotal + ".");
            SetProgress(DownloadDownloaded, DownloadTotal);
        }

        private void Scrape()
        {
            // Regex for acquiring total posts:
            // total_posts":(\d+)
            // Regex for acquiring picture:
            // original_size":{"url":"([^"]+)"

            ulong Found = 0;
            bool DownloadLimitReached = false;

            if (TextBoxTags.Text.Split(',').Length > 1)
            {
                // Multiple tags.
                throw new NotImplementedException("This hasn't been added yet. Sorry.");
            }
            else
            {
                // One tag, or no tags.
                using (WebClient Wc = new WebClient())
                {
                    string Url = "http://api.tumblr.com/v2/blog/" + TextBoxUsernameBlogLink.Text + "/posts/photo?api_key=" + TextBoxAPIKey.Text;
                    if (!string.IsNullOrWhiteSpace(TextBoxTags.Text))
                    {
                        Url += "&tag=" + GetTags();
                    }
                    string Response = Wc.DownloadString(Url);
                    Match TotalPosts = Regex.Match(Response, @"total_posts\"":(\d+)");
                    ulong Total = ulong.Parse(TotalPosts.Groups[1].Value);
                    ulong Pages = Total / ResponseLimit;
                    if ((Total * 1.0 / ResponseLimit) % 1 != 0)
                    {
                        Pages += 1;
                    }
                    for (ulong P = 0; P < Pages; P++)
                    {
                        string PageUrl = "http://api.tumblr.com/v2/blog/" + TextBoxUsernameBlogLink.Text + "/posts/photo?api_key=" + TextBoxAPIKey.Text + "&offset=" + (P * ResponseLimit);
                        UpdateStatus("Retrieved page " + (P + 1) + ", current total: " + DownloadTotal + ".");
                        if (!string.IsNullOrWhiteSpace(TextBoxTags.Text))
                        {
                            PageUrl += "&tag=" + GetTags();
                        }
                        string PageResponse = Wc.DownloadString(PageUrl);
                        MatchCollection Matches = Regex.Matches(PageResponse, @"original_size\"":{\""url\"":\""([^\""]+)\""");
                        DownloadTotal += (ulong)Matches.Count;
                        foreach (Match M in Matches)
                        {
                            LinkQueue.Enqueue(M.Groups[1].Value.Replace("\\/", "/"));
                            Found++;
                            UpdateStatus("Found picture: " + Found + " on page " + (P + 1) + ", out of " + DownloadTotal + ".");
                            if (NumericUpDownDownloadLimit.Value != 0 && Found >= NumericUpDownDownloadLimit.Value)
                            {
                                DownloadLimitReached = true;
                                break;
                            }
                        }
                        if (DownloadLimitReached)
                        {
                            break;
                        }
                    }
                }
            }

            UpdateStatus("Scraping finished, waiting for downloads.");
            // Done.
        }

        private string GetTags()
        {
            return TextBoxTags.Text.Replace(" ", "+");
        }

        private void SetProgress(ulong downloaded, ulong total)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { SetProgress(downloaded, total); });
            }
            else
            {
                int Value = (int)Math.Round(downloaded * 100.0 / total, MidpointRounding.AwayFromZero);
                if (Value > 100)
                {
                    Value = 100;
                }
                if (Value < 0)
                {
                    Value = 0;
                }
                ProgressBarMain.Value = Value;
            }
        }

        private void SetProgress(int value)
        {
            // This should be only used on the main UI thread.
            ProgressBarMain.Value = value;
        }

        private void ModifyControls(bool enabled)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { ModifyControls(enabled); });
            }
            else
            {
                TextBoxUsernameBlogLink.Enabled = enabled;
                TextBoxTags.Enabled = enabled;
                TextBoxDownloadDirectory.Enabled = enabled;
                ButtonBrowse.Enabled = enabled;
                NumericUpDownDownloadLimit.Enabled = enabled;
                CheckBoxUseListCache.Enabled = enabled;
                CheckBoxNotify.Enabled = enabled;
                TextBoxAPIKey.Enabled = enabled;
            }
        }

        private void UpdateStatus(string status)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { LabelStatus.Text = "Status: " + status;  });
            }
            else
            {
                LabelStatus.Text = "Status: " + status;
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

        private string GetFilename(string url)
        {
            return url.Split('/')[url.Split('/').Length - 1];
        }

        #region URL / Webpage Validation

        private bool IsValidUrl(string url)
        {
            Uri Uri = null;
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri) || null == Uri)
            {
                return false;
            }
            return true;
        }

        private bool IsExistingUrl(string url)
        {
            try
            {
                HttpWebRequest Request = WebRequest.Create(url) as HttpWebRequest;
                Request.Method = "HEAD";
                HttpWebResponse Response = Request.GetResponse() as HttpWebResponse;
                Response.Close();
                return (Response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
