namespace DiscordBot
{
    partial class Form1
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
            this.btn_startbot = new System.Windows.Forms.Button();
            this.btn_stopbot = new System.Windows.Forms.Button();
            this.txtKey1 = new System.Windows.Forms.TextBox();
            this.txtKey2 = new System.Windows.Forms.TextBox();
            this.txtKey3 = new System.Windows.Forms.TextBox();
            this.lblKey1 = new System.Windows.Forms.Label();
            this.lblKey2 = new System.Windows.Forms.Label();
            this.lblKey3 = new System.Windows.Forms.Label();
            this.rtxt_debug = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btn_startbot
            // 
            this.btn_startbot.Location = new System.Drawing.Point(89, 87);
            this.btn_startbot.Name = "btn_startbot";
            this.btn_startbot.Size = new System.Drawing.Size(75, 23);
            this.btn_startbot.TabIndex = 0;
            this.btn_startbot.Text = "Start_Bot";
            this.btn_startbot.UseVisualStyleBackColor = true;
            this.btn_startbot.Click += new System.EventHandler(this.btn_startbot_Click_1);
            // 
            // btn_stopbot
            // 
            this.btn_stopbot.Location = new System.Drawing.Point(216, 87);
            this.btn_stopbot.Name = "btn_stopbot";
            this.btn_stopbot.Size = new System.Drawing.Size(75, 23);
            this.btn_stopbot.TabIndex = 1;
            this.btn_stopbot.Text = "Stop Bot";
            this.btn_stopbot.UseVisualStyleBackColor = true;
            this.btn_stopbot.Click += new System.EventHandler(this.btn_stopbot_Click_1);
            // 
            // txtKey1
            // 
            this.txtKey1.AccessibleName = "txtKey1";
            this.txtKey1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtKey1.Location = new System.Drawing.Point(89, 9);
            this.txtKey1.Name = "txtKey1";
            this.txtKey1.Size = new System.Drawing.Size(202, 20);
            this.txtKey1.TabIndex = 5;
            // 
            // txtKey2
            // 
            this.txtKey2.AccessibleDescription = "";
            this.txtKey2.AccessibleName = "txtKey2";
            this.txtKey2.Location = new System.Drawing.Point(89, 35);
            this.txtKey2.Name = "txtKey2";
            this.txtKey2.Size = new System.Drawing.Size(202, 20);
            this.txtKey2.TabIndex = 6;
            // 
            // txtKey3
            // 
            this.txtKey3.AccessibleName = "txtKey3";
            this.txtKey3.Location = new System.Drawing.Point(89, 61);
            this.txtKey3.Name = "txtKey3";
            this.txtKey3.Size = new System.Drawing.Size(202, 20);
            this.txtKey3.TabIndex = 7;
            // 
            // lblKey1
            // 
            this.lblKey1.AutoSize = true;
            this.lblKey1.Location = new System.Drawing.Point(12, 9);
            this.lblKey1.Name = "lblKey1";
            this.lblKey1.Size = new System.Drawing.Size(34, 13);
            this.lblKey1.TabIndex = 8;
            this.lblKey1.Text = "BotID";
            // 
            // lblKey2
            // 
            this.lblKey2.AutoSize = true;
            this.lblKey2.Location = new System.Drawing.Point(12, 35);
            this.lblKey2.Name = "lblKey2";
            this.lblKey2.Size = new System.Drawing.Size(42, 13);
            this.lblKey2.TabIndex = 9;
            this.lblKey2.Text = "GuildID";
            // 
            // lblKey3
            // 
            this.lblKey3.AutoSize = true;
            this.lblKey3.Location = new System.Drawing.Point(12, 61);
            this.lblKey3.Name = "lblKey3";
            this.lblKey3.Size = new System.Drawing.Size(49, 13);
            this.lblKey3.TabIndex = 10;
            this.lblKey3.Text = "ServerID";
            // 
            // rtxt_debug
            // 
            this.rtxt_debug.Location = new System.Drawing.Point(12, 116);
            this.rtxt_debug.Name = "rtxt_debug";
            this.rtxt_debug.Size = new System.Drawing.Size(279, 185);
            this.rtxt_debug.TabIndex = 11;
            this.rtxt_debug.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 313);
            this.Controls.Add(this.rtxt_debug);
            this.Controls.Add(this.lblKey3);
            this.Controls.Add(this.lblKey2);
            this.Controls.Add(this.lblKey1);
            this.Controls.Add(this.txtKey3);
            this.Controls.Add(this.txtKey2);
            this.Controls.Add(this.txtKey1);
            this.Controls.Add(this.btn_stopbot);
            this.Controls.Add(this.btn_startbot);
            this.Name = "Form1";
            this.Text = "DiscordBotInterface";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_startbot;
        private System.Windows.Forms.Button btn_stopbot;
        private System.Windows.Forms.TextBox txtKey1;
        private System.Windows.Forms.TextBox txtKey2;
        private System.Windows.Forms.TextBox txtKey3;
        private System.Windows.Forms.Label lblKey1;
        private System.Windows.Forms.Label lblKey2;
        private System.Windows.Forms.Label lblKey3;
        private System.Windows.Forms.RichTextBox rtxt_debug;
    }
}

