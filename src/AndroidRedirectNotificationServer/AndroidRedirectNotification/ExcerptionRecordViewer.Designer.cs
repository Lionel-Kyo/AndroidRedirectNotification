namespace AndroidRedirectNotification
{
    partial class ExcerptionRecordViewer
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
            dgv = new DataGridView();
            dgvDateTime = new DataGridViewTextBoxColumn();
            dgvName = new DataGridViewTextBoxColumn();
            dgvMessage = new DataGridViewTextBoxColumn();
            dgvRecord = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dgv).BeginInit();
            SuspendLayout();
            // 
            // dgv
            // 
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.BackgroundColor = Color.White;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.Columns.AddRange(new DataGridViewColumn[] { dgvDateTime, dgvName, dgvMessage, dgvRecord });
            dgv.Dock = DockStyle.Fill;
            dgv.Location = new Point(0, 0);
            dgv.Margin = new Padding(4);
            dgv.Name = "dgv";
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.Size = new Size(584, 261);
            dgv.TabIndex = 1;
            // 
            // dgvDateTime
            // 
            dgvDateTime.HeaderText = "DateTime";
            dgvDateTime.Name = "dgvDateTime";
            dgvDateTime.ReadOnly = true;
            dgvDateTime.Width = 150;
            // 
            // dgvName
            // 
            dgvName.HeaderText = "Name";
            dgvName.Name = "dgvName";
            dgvName.ReadOnly = true;
            // 
            // dgvMessage
            // 
            dgvMessage.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvMessage.HeaderText = "Message";
            dgvMessage.Name = "dgvMessage";
            dgvMessage.ReadOnly = true;
            // 
            // dgvRecord
            // 
            dgvRecord.HeaderText = "Record";
            dgvRecord.Name = "dgvRecord";
            dgvRecord.ReadOnly = true;
            dgvRecord.Visible = false;
            // 
            // ExcerptionRecordViewer
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(584, 261);
            Controls.Add(dgv);
            Font = new Font("Segoe UI", 12F);
            Margin = new Padding(4);
            Name = "ExcerptionRecordViewer";
            ShowIcon = false;
            Text = "Excerption Record Viewer";
            FormClosing += ExcerptionRecordViewer_FormClosing;
            KeyDown += ExcerptionRecordViewer_KeyDown;
            ((System.ComponentModel.ISupportInitialize)dgv).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgv;
        private DataGridViewTextBoxColumn dgvDateTime;
        private DataGridViewTextBoxColumn dgvName;
        private DataGridViewTextBoxColumn dgvMessage;
        private DataGridViewTextBoxColumn dgvRecord;
    }
}