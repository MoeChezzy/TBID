﻿using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace TBID
{
    internal static class Program
    {
        public const string Name = "TBID";
        public const string Author = "Chezzy";

        private static readonly string GUID = ((GuidAttribute) Assembly.GetExecutingAssembly().GetCustomAttributes(typeof (GuidAttribute), false).GetValue(0)).Value;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize variables for mutex.
            string MutexID = string.Format("Global\\{{{0}}}", GUID);
            bool CreatedNewMutex;
            MutexAccessRule AllowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
            MutexSecurity SecuritySettings = new MutexSecurity();
            SecuritySettings.AddAccessRule(AllowEveryoneRule);

            using (Mutex mutex = new Mutex(false, MutexID, out CreatedNewMutex, SecuritySettings)) {
                bool HandleAcquired = false;
                try {
                    try {
                        HandleAcquired = mutex.WaitOne(2000, false);
                        if (!HandleAcquired) {
                            MessageBox.Show("There is an instance of " + Name + " already currently running.", Name + " already open.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Close();
                        }
                    }
                    catch (AbandonedMutexException) {
                        // The mutex was abandoned in another process, so in this case it will still get acquired.
                        HandleAcquired = true;
                    }

                    // Begin program.
                    Application.Run(new MainForm());
                    // End program.
                }
                finally {
                    // Release the mutex if it was acquired.
                    if (HandleAcquired)
                        mutex.ReleaseMutex();
                }
            }
        }

        public static void Close()
        {
            if (Application.MessageLoop) Application.Exit();
            else Environment.Exit(0);
        }
    }
}