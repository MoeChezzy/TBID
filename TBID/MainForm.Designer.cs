﻿namespace TBID
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.CheckBoxUseListCache = new System.Windows.Forms.CheckBox();
            this.GroupBoxDownloadSettings = new System.Windows.Forms.GroupBox();
            this.LabelDownloadDirectory = new System.Windows.Forms.Label();
            this.ButtonBrowse = new System.Windows.Forms.Button();
            this.TextBoxDownloadDirectory = new System.Windows.Forms.TextBox();
            this.LabelTags = new System.Windows.Forms.Label();
            this.TextBoxTags = new System.Windows.Forms.TextBox();
            this.LabelUsernameBlogLink = new System.Windows.Forms.Label();
            this.TextBoxUsernameBlogLink = new System.Windows.Forms.TextBox();
            this.CheckBoxNotify = new System.Windows.Forms.CheckBox();
            this.ButtonStart = new System.Windows.Forms.Button();
            this.ProgressBarMain = new System.Windows.Forms.ProgressBar();
            this.FolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.ToolTipMain = new System.Windows.Forms.ToolTip(this.components);
            this.NotifyIconMain = new System.Windows.Forms.NotifyIcon(this.components);
            this.ButtonAbout = new System.Windows.Forms.Button();
            this.TextBoxAPIKey = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.GroupBoxDownloadSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // CheckBoxUseListCache
            // 
            this.CheckBoxUseListCache.AutoSize = true;
            this.CheckBoxUseListCache.Checked = true;
            this.CheckBoxUseListCache.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBoxUseListCache.Location = new System.Drawing.Point(12, 128);
            this.CheckBoxUseListCache.Name = "CheckBoxUseListCache";
            this.CheckBoxUseListCache.Size = new System.Drawing.Size(113, 17);
            this.CheckBoxUseListCache.TabIndex = 8;
            this.CheckBoxUseListCache.Text = "Use List Cache (?)";
            this.CheckBoxUseListCache.UseVisualStyleBackColor = true;
            // 
            // GroupBoxDownloadSettings
            // 
            this.GroupBoxDownloadSettings.Controls.Add(this.LabelDownloadDirectory);
            this.GroupBoxDownloadSettings.Controls.Add(this.ButtonBrowse);
            this.GroupBoxDownloadSettings.Controls.Add(this.TextBoxDownloadDirectory);
            this.GroupBoxDownloadSettings.Controls.Add(this.LabelTags);
            this.GroupBoxDownloadSettings.Controls.Add(this.TextBoxTags);
            this.GroupBoxDownloadSettings.Controls.Add(this.LabelUsernameBlogLink);
            this.GroupBoxDownloadSettings.Controls.Add(this.TextBoxUsernameBlogLink);
            this.GroupBoxDownloadSettings.Location = new System.Drawing.Point(12, 12);
            this.GroupBoxDownloadSettings.Name = "GroupBoxDownloadSettings";
            this.GroupBoxDownloadSettings.Size = new System.Drawing.Size(343, 110);
            this.GroupBoxDownloadSettings.TabIndex = 7;
            this.GroupBoxDownloadSettings.TabStop = false;
            this.GroupBoxDownloadSettings.Text = "Download Settings";
            // 
            // LabelDownloadDirectory
            // 
            this.LabelDownloadDirectory.AutoSize = true;
            this.LabelDownloadDirectory.Location = new System.Drawing.Point(20, 76);
            this.LabelDownloadDirectory.Name = "LabelDownloadDirectory";
            this.LabelDownloadDirectory.Size = new System.Drawing.Size(103, 13);
            this.LabelDownloadDirectory.TabIndex = 6;
            this.LabelDownloadDirectory.Text = "Download Directory:";
            // 
            // ButtonBrowse
            // 
            this.ButtonBrowse.Location = new System.Drawing.Point(310, 71);
            this.ButtonBrowse.Name = "ButtonBrowse";
            this.ButtonBrowse.Size = new System.Drawing.Size(27, 23);
            this.ButtonBrowse.TabIndex = 5;
            this.ButtonBrowse.Text = "...";
            this.ButtonBrowse.UseVisualStyleBackColor = true;
            // 
            // TextBoxDownloadDirectory
            // 
            this.TextBoxDownloadDirectory.Location = new System.Drawing.Point(129, 73);
            this.TextBoxDownloadDirectory.Name = "TextBoxDownloadDirectory";
            this.TextBoxDownloadDirectory.Size = new System.Drawing.Size(175, 20);
            this.TextBoxDownloadDirectory.TabIndex = 4;
            // 
            // LabelTags
            // 
            this.LabelTags.AutoSize = true;
            this.LabelTags.Location = new System.Drawing.Point(57, 50);
            this.LabelTags.Name = "LabelTags";
            this.LabelTags.Size = new System.Drawing.Size(66, 13);
            this.LabelTags.TabIndex = 3;
            this.LabelTags.Text = "Tags to use:";
            // 
            // TextBoxTags
            // 
            this.TextBoxTags.ImeMode = System.Windows.Forms.ImeMode.On;
            this.TextBoxTags.Location = new System.Drawing.Point(129, 47);
            this.TextBoxTags.Name = "TextBoxTags";
            this.TextBoxTags.Size = new System.Drawing.Size(208, 20);
            this.TextBoxTags.TabIndex = 2;
            // 
            // LabelUsernameBlogLink
            // 
            this.LabelUsernameBlogLink.AutoSize = true;
            this.LabelUsernameBlogLink.Location = new System.Drawing.Point(6, 24);
            this.LabelUsernameBlogLink.Name = "LabelUsernameBlogLink";
            this.LabelUsernameBlogLink.Size = new System.Drawing.Size(117, 13);
            this.LabelUsernameBlogLink.TabIndex = 8;
            this.LabelUsernameBlogLink.Text = "Username or Blog Link:";
            // 
            // TextBoxUsernameBlogLink
            // 
            this.TextBoxUsernameBlogLink.Location = new System.Drawing.Point(129, 21);
            this.TextBoxUsernameBlogLink.Name = "TextBoxUsernameBlogLink";
            this.TextBoxUsernameBlogLink.Size = new System.Drawing.Size(208, 20);
            this.TextBoxUsernameBlogLink.TabIndex = 0;
            // 
            // CheckBoxNotify
            // 
            this.CheckBoxNotify.AutoSize = true;
            this.CheckBoxNotify.Checked = true;
            this.CheckBoxNotify.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBoxNotify.Location = new System.Drawing.Point(132, 128);
            this.CheckBoxNotify.Name = "CheckBoxNotify";
            this.CheckBoxNotify.Size = new System.Drawing.Size(169, 17);
            this.CheckBoxNotify.TabIndex = 9;
            this.CheckBoxNotify.Text = "Notify when download finishes";
            this.CheckBoxNotify.UseVisualStyleBackColor = true;
            // 
            // ButtonStart
            // 
            this.ButtonStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonStart.Location = new System.Drawing.Point(307, 124);
            this.ButtonStart.Name = "ButtonStart";
            this.ButtonStart.Size = new System.Drawing.Size(48, 23);
            this.ButtonStart.TabIndex = 10;
            this.ButtonStart.Text = "Start";
            this.ButtonStart.UseVisualStyleBackColor = true;
            // 
            // ProgressBarMain
            // 
            this.ProgressBarMain.Location = new System.Drawing.Point(12, 151);
            this.ProgressBarMain.Name = "ProgressBarMain";
            this.ProgressBarMain.Size = new System.Drawing.Size(289, 23);
            this.ProgressBarMain.TabIndex = 12;
            // 
            // NotifyIconMain
            // 
            this.NotifyIconMain.Visible = true;
            // 
            // ButtonAbout
            // 
            this.ButtonAbout.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ButtonAbout.Location = new System.Drawing.Point(307, 151);
            this.ButtonAbout.Name = "ButtonAbout";
            this.ButtonAbout.Size = new System.Drawing.Size(48, 23);
            this.ButtonAbout.TabIndex = 11;
            this.ButtonAbout.Text = "About";
            this.ButtonAbout.UseVisualStyleBackColor = true;
            // 
            // TextBoxAPIKey
            // 
            this.TextBoxAPIKey.Location = new System.Drawing.Point(12, 180);
            this.TextBoxAPIKey.Name = "TextBoxAPIKey";
            this.TextBoxAPIKey.Size = new System.Drawing.Size(267, 20);
            this.TextBoxAPIKey.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(285, 184);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Your API Key";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 212);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TextBoxAPIKey);
            this.Controls.Add(this.ButtonAbout);
            this.Controls.Add(this.ProgressBarMain);
            this.Controls.Add(this.ButtonStart);
            this.Controls.Add(this.CheckBoxNotify);
            this.Controls.Add(this.GroupBoxDownloadSettings);
            this.Controls.Add(this.CheckBoxUseListCache);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TBID";
            this.GroupBoxDownloadSettings.ResumeLayout(false);
            this.GroupBoxDownloadSettings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox CheckBoxUseListCache;
        private System.Windows.Forms.GroupBox GroupBoxDownloadSettings;
        private System.Windows.Forms.TextBox TextBoxUsernameBlogLink;
        private System.Windows.Forms.Label LabelUsernameBlogLink;
        private System.Windows.Forms.TextBox TextBoxTags;
        private System.Windows.Forms.Label LabelTags;
        private System.Windows.Forms.TextBox TextBoxDownloadDirectory;
        private System.Windows.Forms.Button ButtonBrowse;
        private System.Windows.Forms.Label LabelDownloadDirectory;
        private System.Windows.Forms.CheckBox CheckBoxNotify;
        private System.Windows.Forms.Button ButtonStart;
        private System.Windows.Forms.ProgressBar ProgressBarMain;
        private System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog;
        private System.Windows.Forms.ToolTip ToolTipMain;
        private System.Windows.Forms.NotifyIcon NotifyIconMain;
        private System.Windows.Forms.Button ButtonAbout;
        private System.Windows.Forms.TextBox TextBoxAPIKey;
        private System.Windows.Forms.Label label1;
    }
}