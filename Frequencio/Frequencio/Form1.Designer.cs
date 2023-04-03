namespace Frequencio
{
    partial class Frequencio_Form
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
            this.selectFolderButton = new System.Windows.Forms.Button();
            this.numberOfFilesLabel = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.audioFilesListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // selectFolderButton
            // 
            this.selectFolderButton.Location = new System.Drawing.Point(13, 13);
            this.selectFolderButton.Name = "selectFolderButton";
            this.selectFolderButton.Size = new System.Drawing.Size(170, 68);
            this.selectFolderButton.TabIndex = 0;
            this.selectFolderButton.Text = "selectFolderButton";
            this.selectFolderButton.UseVisualStyleBackColor = true;
            this.selectFolderButton.Click += new System.EventHandler(this.selectFolderButton_Click);
            // 
            // numberOfFilesLabel
            // 
            this.numberOfFilesLabel.AutoSize = true;
            this.numberOfFilesLabel.Location = new System.Drawing.Point(31, 407);
            this.numberOfFilesLabel.Name = "numberOfFilesLabel";
            this.numberOfFilesLabel.Size = new System.Drawing.Size(152, 20);
            this.numberOfFilesLabel.TabIndex = 1;
            this.numberOfFilesLabel.Text = "numberOfFilesLabel";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 398);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(776, 40);
            this.progressBar1.TabIndex = 2;
            // 
            // audioFilesListBox
            // 
            this.audioFilesListBox.FormattingEnabled = true;
            this.audioFilesListBox.ItemHeight = 20;
            this.audioFilesListBox.Location = new System.Drawing.Point(190, 13);
            this.audioFilesListBox.Name = "audioFilesListBox";
            this.audioFilesListBox.Size = new System.Drawing.Size(598, 364);
            this.audioFilesListBox.TabIndex = 3;
            // 
            // Frequencio_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.audioFilesListBox);
            this.Controls.Add(this.numberOfFilesLabel);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.selectFolderButton);
            this.Name = "Frequencio_Form";
            this.Text = "Frequencio";
            this.Load += new System.EventHandler(this.Frequencio_Form_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button selectFolderButton;
        private System.Windows.Forms.Label numberOfFilesLabel;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ListBox audioFilesListBox;

    }
}

