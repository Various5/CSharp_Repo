using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;



namespace AbschaumBot
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Opacity = 0;
            this.ShowInTaskbar = false;
            // Create and show the overlay
            Overlay overlay = new Overlay();
            overlay.Show();
        }
    }

    public class Overlay : Form
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        private Button minimizeButton;
        private Button closeButton;
        private bool isMinimized;
        private CommandList commandList;
        private ComboBox processComboBox;

        public Overlay()
        {
            // Set the form properties to make it semi-transparent and click-through
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Fuchsia;
            this.TransparencyKey = Color.Fuchsia;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
            this.Opacity = 1.5;

            // Add a label to the form with the text "Overlay"
            Label label = new Label();
            label.Text = "Overlay";
            label.Font = new Font("Arial", 16, FontStyle.Bold);
            label.ForeColor = Color.White;
            label.AutoSize = true;
            label.Location = new Point((this.Width - label.Width) / 2, 50);
            this.Controls.Add(label);

            // Add a minimize button to the form
            this.minimizeButton = new Button();
            this.minimizeButton.Text = "-";
            this.minimizeButton.Font = new Font("Arial", 12, FontStyle.Bold);
            this.minimizeButton.ForeColor = Color.Blue;
            this.minimizeButton.FlatStyle = FlatStyle.Flat;
            this.minimizeButton.FlatAppearance.BorderSize = 0;
            this.minimizeButton.AutoSize = true;
            this.minimizeButton.Location = new Point(this.Width - this.minimizeButton.Width - 10, 10);
            this.Controls.Add(this.minimizeButton);
            this.minimizeButton.Click += new EventHandler(MinimizeButton_Click);

            // Add a close button to the form
            this.closeButton = new Button();
            this.closeButton.Text = "X";
            this.closeButton.Font = new Font("Arial", 12, FontStyle.Bold);
            this.closeButton.ForeColor = Color.Blue;
            this.closeButton.FlatStyle = FlatStyle.Flat;
            this.closeButton.FlatAppearance.BorderSize = 0;
            this.closeButton.AutoSize = true;
            this.closeButton.Location = new Point(this.Width - this.closeButton.Width - this.minimizeButton.Width - 20, 10);
            this.Controls.Add(this.closeButton);
            this.closeButton.Click += new EventHandler(CloseButton_Click);

            // Add a dropdown of running processes to choose from
            this.processComboBox = new ComboBox();
            this.processComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.processComboBox.Font = new Font("Arial", 12, FontStyle.Bold);
            this.processComboBox.ForeColor = Color.Blue;
            this.processComboBox.FlatStyle = FlatStyle.Flat;
            this.processComboBox.BackColor = Color.White;
            this.processComboBox.Location = new Point(10, 70);
            this.Controls.Add(this.processComboBox);

            // Fill the dropdown with the names of running processes
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    this.processComboBox.Items.Add(process.ProcessName);
                }
            }

            // Add a button to the form to attach the selected process and send keys
            Button attachButton = new Button();
            attachButton.Text = "Attach and Send Keys";
            attachButton.Font = new Font("Arial", 12, FontStyle.Bold);
            attachButton.ForeColor = Color.Blue;
            attachButton.FlatStyle = FlatStyle.Flat;
            attachButton.FlatAppearance.BorderSize = 0;
            attachButton.AutoSize = true;
            attachButton.Location = new Point(10, 110);
            this.Controls.Add(attachButton);
            attachButton.Click += new EventHandler(AttachButton_Click);

            // Set the initial state of the form to not minimized
            this.isMinimized = false;

            // Handle the Resize event to restore the form when it is maximized
            this.Resize += new EventHandler(Overlay_Resize);

            // Initialize the CommandList instance
            this.commandList = new CommandList();

            // Add a button to the form to call the WriteTest method
            Button writeTestButton = new Button();
            writeTestButton.Text = "Write Test";
            writeTestButton.Font = new Font("Arial", 12, FontStyle.Bold);
            writeTestButton.ForeColor = Color.Blue;
            writeTestButton.FlatStyle = FlatStyle.Flat;
            writeTestButton.FlatAppearance.BorderSize = 0;
            writeTestButton.AutoSize = true;
            writeTestButton.Location = new Point(10, 150);
            this.Controls.Add(writeTestButton);
            writeTestButton.Click += new EventHandler(WriteTestButton_Click);
        }

        private void AttachButton_Click(object sender, EventArgs e)
        {
            // Get the selected process from the dropdown
            string processName = this.processComboBox.SelectedItem as string;

            // Attach to the process by name
            Process[] processes = Process.GetProcessesByName(processName);
            if (processes.Length > 0)
            {
                Process process = processes[0];
                IntPtr processHandle = process.MainWindowHandle;

                // Bring the process to the front
                SetForegroundWindow(processHandle);

                // Send the desired keys to the process
                SendKeys.SendWait("{T}");
                SendKeys.SendWait("Bot Writing Test lul");
                SendKeys.SendWait("{ENTER}");
            }
        }

        private void WriteTestButton_Click(object sender, EventArgs e)
        {
            // Call the WriteTest method of the CommandList instance
            this.commandList.Writetest();
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            if (this.isMinimized)
            {
                // If the form is already minimized, restore it
                this.WindowState = FormWindowState.Normal;
                this.minimizeButton.Text = "-";
                this.isMinimized = false;
            }
            else
            {
                // If the form is not minimized, minimize it
                this.WindowState = FormWindowState.Minimized;
                this.minimizeButton.Text = "+";
                this.isMinimized = true;
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            // Close the form when the close button is clicked
            this.Close();
            Application.Exit();
        }

        private void Overlay_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                // If the form has been restored to its normal state, set isMinimized to false
                this.isMinimized = false;
            }
        }
    }
}
