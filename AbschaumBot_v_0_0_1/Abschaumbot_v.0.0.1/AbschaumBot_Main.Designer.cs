namespace Abschaumbot_v._0._0._1
{
    partial class AbschaumBot_Main
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboProcesses = new System.Windows.Forms.ComboBox();
            this.BtnAttach = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.BtnSpawnTest = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comboProcesses
            // 
            this.comboProcesses.FormattingEnabled = true;
            this.comboProcesses.Location = new System.Drawing.Point(13, 13);
            this.comboProcesses.Name = "comboProcesses";
            this.comboProcesses.Size = new System.Drawing.Size(245, 21);
            this.comboProcesses.TabIndex = 0;
            // 
            // BtnAttach
            // 
            this.BtnAttach.Location = new System.Drawing.Point(264, 12);
            this.BtnAttach.Name = "BtnAttach";
            this.BtnAttach.Size = new System.Drawing.Size(111, 23);
            this.BtnAttach.TabIndex = 1;
            this.BtnAttach.Text = "Attach to Process";
            this.BtnAttach.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.BackColor = System.Drawing.SystemColors.Info;
            this.lblStatus.Location = new System.Drawing.Point(10, 37);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(37, 13);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Status";
            // 
            // BtnSpawnTest
            // 
            this.BtnSpawnTest.Location = new System.Drawing.Point(13, 53);
            this.BtnSpawnTest.Name = "BtnSpawnTest";
            this.BtnSpawnTest.Size = new System.Drawing.Size(75, 23);
            this.BtnSpawnTest.TabIndex = 5;
            this.BtnSpawnTest.Text = "SpawnTest";
            this.BtnSpawnTest.UseVisualStyleBackColor = true;
            this.BtnSpawnTest.Click += new System.EventHandler(this.BtnSpawnTest_Click);
            // 
            // AbschaumBot_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(414, 341);
            this.Controls.Add(this.BtnSpawnTest);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.BtnAttach);
            this.Controls.Add(this.comboProcesses);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "AbschaumBot_Main";
            this.Text = "Abschaumbot v0.0.1 by g3neric";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboProcesses;
        private System.Windows.Forms.Button BtnAttach;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button BtnSpawnTest;
    }
}

