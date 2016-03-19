using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        private Stopwatch stopwatch = new Stopwatch();

        private ulong Download_Downloaded = 0;
        private ulong Download_Total = 0;

        private const int ResponseLimit = 20;

        public MainForm()
        {
            InitializeComponent();
            InitializeFormClosingEvents();
            InitializeNotifyIcon();

            this.Icon = Properties.Resources.Icon;
            this.TextBoxAPIKey.PasswordChar = '\u25CF';
        }

        private void InitializeFormClosingEvents()
        {
            this.FormClosing += MainForm_FormClosing;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!IsDownloading) {
                CleanupThreads();
                Program.Close();
            }
            else {
                DialogResult Confirmation = MessageBox.Show(string.Format("There is currently a download in progress! Closing {0} will abort the download. Do you still want to exit?", Program.Name), "Download in progress.", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (Confirmation == DialogResult.No) {
                    e.Cancel = true;
                }
                else {
                    CleanupThreads();
                    Program.Close();
                }
            }
        }

        private void CleanupThreads()
        {
            while (ThreadScrape.IsAlive)    ThreadDownload.Abort();
            while (ThreadDownload.IsAlive)  ThreadDownload.Abort();
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
            MessageBox.Show(string.Format("{0} " + "is a portable and lightweight tumblr blog image downloader." + " " + "It is free and open source, released under the GNU General Public License (version 3).{1}{1}Copyright \u00A9 {2} {3}", Program.Name, Environment.NewLine, DateTime.Now.Year, Program.Author), "About");
        }

        private void ButtonStart_Click(object sender, EventArgs e)
        {
            if (!IsDownloading) {
                // Validating directory.
                if (!Directory.Exists(TextBoxDownloadDirectory.Text)) {
                    MessageBox.Show("The specified directory is invalid. Please input a valid directory.", "Invalid directory.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Removing potential trailing backslash.
                if (TextBoxDownloadDirectory.Text[TextBoxDownloadDirectory.Text.Length - 1] == '\\')
                    TextBoxDownloadDirectory.Text = TextBoxDownloadDirectory.Text.Remove(TextBoxDownloadDirectory.Text.Length - 1);

                // Validating URL.
                // We might want to consider putting this check into a separate thread.
                if (IsValidURL(TextBoxUsernameBlogLink.Text)) {
                    // Is a URL.

                    if (!IsExistingURL(TextBoxUsernameBlogLink.Text)) {
                        MessageBox.Show("An error occured while accessing the blog. This may be because the blog has been deleted or there was an error in your input.", "URL-404 / username not found.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Valid URL.
                }
                else {
                    // Might be a username, check validity.
                    string UserURL = string.Format("http://{0}.tumblr.com/", TextBoxUsernameBlogLink.Text);
                    if (!IsValidURL(UserURL)) {
                        MessageBox.Show("The specified URL / username is invalid. Please recheck your input.", "Invalid URL / username.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (!IsExistingURL(UserURL)) {
                        MessageBox.Show("An error occured while accessing the blog. This may be because the blog has been deleted or there was an error in your input.", "URL-404 / username not found.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Valid username.
                }

                // Validating API key.
                // We'll be checking for the valid length (50 characters).

                if (string.IsNullOrWhiteSpace(TextBoxAPIKey.Text)) {
                    MessageBox.Show("Please enter a valid API key; you cannot use " + Program.Name + " without one.", "Missing API key.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (TextBoxAPIKey.Text.Length != 50) {
                    MessageBox.Show("The API key you entered is of invalid length.", "Invalid API key.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (Properties.Settings.Default.FirstTimeInconsistencies) {
                    if (!string.IsNullOrWhiteSpace(TextBoxTags.Text)) {
                        // Notify the user about the progress inconsistencies with multiple tags.
                        if (TextBoxTags.Text.Split(',').Length > 1) {
                            MessageBox.Show("Because of the limitations of Tumblr's API, it is not possible to retrieve the total amount of posts that contain multiple tags in one request. The progress bar may fluctuate and go up or down as " + Program.Name + " recalculates the actual amount of posts containing all of your specified tags.", "Progress bar inconsistencies.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Properties.Settings.Default.FirstTimeInconsistencies = false;
                            Properties.Settings.Default.Save();
                        }
                    }
                }

                IsDownloading = true;
                ButtonStart.Text = "Stop";
                ModifyControls(false);
                LinkQueue.Clear();

                stopwatch.Reset();
                stopwatch.Start();

                SetProgress(0);

                ThreadDownload = new Thread(Download);
                ThreadDownload.Start();
            }
            else
            {
                CleanupThreads();
                stopwatch.Stop();
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

            Download_Downloaded = 0;
            Download_Total = 0;

            ThreadScrape = new Thread(Scrape);
            ThreadScrape.Start();

            while (ThreadScrape.IsAlive)
            {
                while (LinkQueue.Count > 0)
                {
                    string url = LinkQueue.Dequeue();
                    Uri uri = new Uri(url);
                    WebClients.Add(new WebClient());
                    WebClients.Last().DownloadFileCompleted += WebClient_DownloadFileCompleted;
                    WebClients.Last().DownloadFileAsync(uri, TextBoxDownloadDirectory.Text + "\\" + GetFilename(url));
                }
            }

            // Check if any WebClient objects are downloading.
            bool Done = false;
            while (!Done)
            {
                Done = true;
                foreach (WebClient wc in WebClients.ToList())
                {
                    if (wc.IsBusy)
                    {
                        Done = false;
                        break;
                    }
                    WebClients.Remove(wc);
                }
            }

            // Done.
            IsDownloading = false;
            this.Invoke((MethodInvoker)delegate { ButtonStart.Text = "Start"; });
            this.Invoke((MethodInvoker)delegate { SetProgress(100); });
            UpdateStatus("Finished downloading. Time elapsed: " + stopwatch.Elapsed + ".");
            ModifyControls(true);
            LinkQueue.Clear();
            stopwatch.Stop();
            ThreadDownload.Abort();
            ThreadScrape.Abort();
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Download_Downloaded++;
            UpdateStatus(string.Format("Download progress: {0}/{1}.", Download_Downloaded, Download_Total));
            SetProgress(Download_Downloaded, Download_Total);
        }

        private void Scrape()
        {
            // Regex for acquiring total posts:
            // total_posts":(\d+)
            // Regex for acquiring picture:
            // original_size":{"url":"([^"]+)"

            ulong Found = 0;
            bool DownloadLimitReached = false;

            if (TextBoxTags.Text.Split(',').Length > 1) {
                // Multiple tags.
                throw new NotImplementedException("This hasn't been added yet. Sorry.");
            }
            else
            {
                // One tag, or no tags.
                using (WebClient wc = new WebClient()) {
                    string url = string.Format("http://api.tumblr.com/v2/blog/{0}/posts/photo?api_key={1}", TextBoxUsernameBlogLink.Text, TextBoxAPIKey.Text);
                    if (!string.IsNullOrWhiteSpace(TextBoxTags.Text)) url += "&tag=" + GetTags();
                    string response = wc.DownloadString(url);
                    Match totalPosts = Regex.Match(response, @"total_posts\"":(\d+)");
                    ulong total = ulong.Parse(totalPosts.Groups[1].Value);
                    ulong pages = total/ResponseLimit;
                    if ((total*1.0/ResponseLimit)%1 != 0)
                        pages ++;
                    for (ulong p = 0; p < pages; p++) {
                        string pageURL = string.Format("http://api.tumblr.com/v2/blog/{0}/posts/photo?api_key={1}&offset={2}", TextBoxUsernameBlogLink.Text, TextBoxAPIKey.Text, p*ResponseLimit);
                        UpdateStatus("Retrieved page " + (p + 1) + ", current total: " + Download_Total + ".");
                        if (!string.IsNullOrWhiteSpace(TextBoxTags.Text))
                            pageURL += "&tag=" + GetTags();
                        string pageResponse = wc.DownloadString(pageURL);
                        MatchCollection matches = Regex.Matches(pageResponse, @"original_size\"":{\""url\"":\""([^\""]+)\""");
                        Download_Total += (ulong) matches.Count;
                        foreach (Match m in matches) {
                            LinkQueue.Enqueue(m.Groups[1].Value.Replace("\\/", "/"));
                            Found++;
                            UpdateStatus(string.Format("Found picture: {0} on page {1}, out of {2}.", Found, p + 1, Download_Total));
                            if (NumericUpDownDownloadLimit.Value != 0 && Found >= NumericUpDownDownloadLimit.Value) {
                                DownloadLimitReached = true;
                                break;
                            }
                        }
                        if (DownloadLimitReached) break;
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

        private void SetProgress(ulong Downloaded, ulong Total)
        {
            if (InvokeRequired)
                this.Invoke((MethodInvoker) delegate { SetProgress(Downloaded, Total); });
            else {
                int value = (int) Math.Round(Downloaded*100.0/Total, MidpointRounding.AwayFromZero);
                if (value > 100) value = 100;
                if (value < 0) value = 0;
                ProgressBarMain.Value = value;
            }
        }

        private void SetProgress(int Value)
        {
            // This should be only used on the main UI thread.
            ProgressBarMain.Value = Value;
        }

        private void ModifyControls(bool Enabled)
        {
            if (InvokeRequired)
                this.Invoke((MethodInvoker) delegate { ModifyControls(Enabled); });
            else {
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

        private void UpdateStatus(string Status)
        {
            if (InvokeRequired)
                this.Invoke((MethodInvoker)delegate { LabelStatus.Text = "Status: " + Status; });
            else
                LabelStatus.Text = "Status: " + Status;
        }

        private void ButtonBrowse_Click(object sender, EventArgs e)
        {
            DialogResult Result = FolderBrowserDialogMain.ShowDialog();
            if (Result == DialogResult.Cancel || Result == DialogResult.Abort || Result == DialogResult.Ignore || Result == DialogResult.No || Result == DialogResult.None)
                return;
            TextBoxDownloadDirectory.Clear();
            TextBoxDownloadDirectory.Text = FolderBrowserDialogMain.SelectedPath;
        }

        private string GetFilename(string URL)
        {
            return URL.Split('/')[URL.Split('/').Length - 1];
        }

        #region URL / Webpage Validation

        private bool IsValidURL(string URL)
        {
            Uri uri;
            return Uri.TryCreate(URL, UriKind.Absolute, out uri) && uri != null;
        }

        private bool IsExistingURL(string URL)
        {
            try {
                HttpWebRequest request = WebRequest.Create(URL) as HttpWebRequest;
                request.Method = "HEAD";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                response.Close();
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch {
                return false;
            }
        }

        #endregion
    }
}
