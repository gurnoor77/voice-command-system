// Form1.Designer.cs
namespace VoiceCommandApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.btnStartListening = new System.Windows.Forms.Button();
            this.btnViewHistory = new System.Windows.Forms.Button();
            this.btnDeleteHistory = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.txtRecognized = new System.Windows.Forms.TextBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.panelGrid = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panelGrid.SuspendLayout();
            this.SuspendLayout();

            // pictureBoxLogo
            this.pictureBoxLogo.Image = global::VoiceCommandApp.Properties.Resources.voice_icon;
            this.pictureBoxLogo.Size = new System.Drawing.Size(80, 80);
            this.pictureBoxLogo.Location = new System.Drawing.Point(30, 20);
            this.pictureBoxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;

            // ROW 1 BUTTONS — y=120, height=45
            // btnStartListening
            this.btnStartListening.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnStartListening.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnStartListening.ForeColor = System.Drawing.Color.White;
            this.btnStartListening.Location = new System.Drawing.Point(30, 120);
            this.btnStartListening.Size = new System.Drawing.Size(170, 45);
            this.btnStartListening.Text = "Start Listening";
            this.btnStartListening.UseVisualStyleBackColor = false;
            this.btnStartListening.Click += new System.EventHandler(this.btnStartListening_Click);

            // btnViewHistory
            this.btnViewHistory.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnViewHistory.BackColor = System.Drawing.Color.Gray;
            this.btnViewHistory.ForeColor = System.Drawing.Color.White;
            this.btnViewHistory.Location = new System.Drawing.Point(200, 120);
            this.btnViewHistory.Size = new System.Drawing.Size(160, 45);
            this.btnViewHistory.Text = "View History";
            this.btnViewHistory.UseVisualStyleBackColor = false;
            this.btnViewHistory.Click += new System.EventHandler(this.btnViewHistory_Click);

            // btnDeleteHistory
            this.btnDeleteHistory.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDeleteHistory.BackColor = System.Drawing.Color.Crimson;
            this.btnDeleteHistory.ForeColor = System.Drawing.Color.White;
            this.btnDeleteHistory.Location = new System.Drawing.Point(370, 120);
            this.btnDeleteHistory.Size = new System.Drawing.Size(160, 45);
            this.btnDeleteHistory.Text = "Delete History";
            this.btnDeleteHistory.UseVisualStyleBackColor = false;
            this.btnDeleteHistory.Click += new System.EventHandler(this.btnDeleteHistory_Click);

            // btnCopy
            this.btnCopy.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCopy.BackColor = System.Drawing.Color.SeaGreen;
            this.btnCopy.ForeColor = System.Drawing.Color.White;
            this.btnCopy.Location = new System.Drawing.Point(540, 120);
            this.btnCopy.Size = new System.Drawing.Size(160, 45);
            this.btnCopy.Text = "Copy";
            this.btnCopy.UseVisualStyleBackColor = false;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);

            // ROW 2 — txtRecognized y=185
            this.txtRecognized.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtRecognized.Location = new System.Drawing.Point(30, 185);
            this.txtRecognized.Size = new System.Drawing.Size(670, 30);
            this.txtRecognized.ReadOnly = true;

            // lblStatus
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblStatus.Location = new System.Drawing.Point(30, 225);
            this.lblStatus.Size = new System.Drawing.Size(670, 25);
            this.lblStatus.Text = "Status: Waiting";

            // panelGrid
            this.panelGrid.Location = new System.Drawing.Point(30, 260);
            this.panelGrid.Size = new System.Drawing.Size(720, 330);
            this.panelGrid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom;
            this.panelGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.panelGrid.BackColor = System.Drawing.Color.Transparent;
            this.panelGrid.Controls.Add(this.dataGridView1);

            // dataGridView1
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.ColumnHeadersVisible = true;

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 650);
            this.Controls.Add(this.pictureBoxLogo);
            this.Controls.Add(this.panelGrid);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtRecognized);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.btnDeleteHistory);
            this.Controls.Add(this.btnViewHistory);
            this.Controls.Add(this.btnStartListening);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Voice Command System";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panelGrid.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Button btnStartListening;
        private System.Windows.Forms.Button btnViewHistory;
        private System.Windows.Forms.Button btnDeleteHistory;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.TextBox txtRecognized;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.Panel panelGrid;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}