using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Abschaumbot_v._0._0._1
{
    public partial class AbschaumBot_Main : Form
    {
        private IntPtr attachedProcess;

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        public AbschaumBot_Main()
        {
            InitializeComponent();

            // Attach the BtnAttach_Click event to the BtnAttach button
            BtnAttach.Click += BtnAttach_Click;

            // Populate the ComboBox with a list of all running processes
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                comboProcesses.Items.Add(process.ProcessName);
            }
        }


        public void BtnAttach_Click(object sender, EventArgs e)
        {
            // Get the process name from the selected item in the ComboBox
            string processName = comboProcesses.SelectedItem.ToString();

            // Find the process with the given name
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length > 0)
            {
                Process process = processes[0];

                // Assign the MainWindowHandle value to the attachedProcess field of the class
                attachedProcess = process.MainWindowHandle;

                // Bring the process to the front
                SetForegroundWindow(process.MainWindowHandle);
                SendKeys.SendWait("{T}");
                SendKeys.SendWait("The Bot is up and ready");
                SendKeys.SendWait("{ENTER}");
                SendKeys.SendWait("{ENTER}");

                // Update the status label
                lblStatus.Text = "Online";
            }
            else
            {
                // Update the status label
                lblStatus.Text = "Offline";
            }
        }

        private void AbschaumBot_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Close the attached process when the form is closed
            if (attachedProcess != IntPtr.Zero && !Process.GetProcessById(attachedProcess.ToInt32()).HasExited)
            {
                Process.GetProcessById(attachedProcess.ToInt32()).Kill();
            }
        }

        private void BtnSpawnTest_Click(object sender, EventArgs e)
        {

                SetForegroundWindow(attachedProcess);
                SendKeys.SendWait("{T}");
                SendKeys.SendWait("#SpawnAnimal BP_Crow");
                SendKeys.SendWait("{ENTER}");
                SendKeys.SendWait("{ENTER}");
            
        }
    }
}
