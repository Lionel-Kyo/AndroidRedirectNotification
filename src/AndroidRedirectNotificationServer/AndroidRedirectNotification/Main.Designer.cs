namespace AndroidRedirectNotification
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            recvMsgText = new TextBox();
            recvMsgMenu = new ContextMenuStrip(components);
            recvMsgMenu_Copy = new ToolStripMenuItem();
            recvMsgMenu_SelectAll = new ToolStripMenuItem();
            recvMsgMenu_ClearAll = new ToolStripMenuItem();
            portLabel = new Label();
            portNum = new NumericUpDown();
            label1 = new Label();
            applyBtn = new Button();
            tableLayoutPanel = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            tableLayoutPanel1 = new TableLayoutPanel();
            recvMsgMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)portNum).BeginInit();
            tableLayoutPanel.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // recvMsgText
            // 
            recvMsgText.BackColor = Color.White;
            recvMsgText.ContextMenuStrip = recvMsgMenu;
            recvMsgText.Dock = DockStyle.Fill;
            recvMsgText.Location = new Point(3, 32);
            recvMsgText.MaxLength = int.MaxValue;
            recvMsgText.Multiline = true;
            recvMsgText.Name = "recvMsgText";
            recvMsgText.ReadOnly = true;
            recvMsgText.Size = new Size(422, 208);
            recvMsgText.TabIndex = 4;
            // 
            // recvMsgMenu
            // 
            recvMsgMenu.Items.AddRange(new ToolStripItem[] { recvMsgMenu_Copy, recvMsgMenu_SelectAll, recvMsgMenu_ClearAll });
            recvMsgMenu.Name = "recvMsgMenu";
            recvMsgMenu.Size = new Size(123, 70);
            // 
            // recvMsgMenu_Copy
            // 
            recvMsgMenu_Copy.Name = "recvMsgMenu_Copy";
            recvMsgMenu_Copy.Size = new Size(122, 22);
            recvMsgMenu_Copy.Text = "Copy";
            recvMsgMenu_Copy.Click += recvMsgMenu_Copy_Click;
            // 
            // recvMsgMenu_SelectAll
            // 
            recvMsgMenu_SelectAll.Name = "recvMsgMenu_SelectAll";
            recvMsgMenu_SelectAll.Size = new Size(122, 22);
            recvMsgMenu_SelectAll.Text = "Select All";
            recvMsgMenu_SelectAll.Click += recvMsgMenu_SelectAll_Click;
            // 
            // recvMsgMenu_ClearAll
            // 
            recvMsgMenu_ClearAll.Name = "recvMsgMenu_ClearAll";
            recvMsgMenu_ClearAll.Size = new Size(122, 22);
            recvMsgMenu_ClearAll.Text = "Clear All";
            recvMsgMenu_ClearAll.Click += recvMsgMenu_ClearAll_Click;
            // 
            // portLabel
            // 
            portLabel.AutoSize = true;
            portLabel.Location = new Point(3, 0);
            portLabel.Name = "portLabel";
            portLabel.Size = new Size(79, 19);
            portLabel.TabIndex = 0;
            portLabel.Text = "Server Port:";
            // 
            // portNum
            // 
            portNum.Location = new Point(99, 3);
            portNum.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            portNum.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            portNum.Name = "portNum";
            portNum.Size = new Size(61, 25);
            portNum.TabIndex = 1;
            portNum.Value = new decimal(new int[] { 40001, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(123, 19);
            label1.TabIndex = 3;
            label1.Text = "Received Message:";
            // 
            // applyBtn
            // 
            applyBtn.Location = new Point(166, 3);
            applyBtn.Name = "applyBtn";
            applyBtn.Size = new Size(97, 30);
            applyBtn.TabIndex = 2;
            applyBtn.Text = "Apply";
            applyBtn.UseVisualStyleBackColor = true;
            applyBtn.Click += applyBtn_Click;
            // 
            // tableLayoutPanel
            // 
            tableLayoutPanel.ColumnCount = 1;
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel.Controls.Add(tableLayoutPanel2, 0, 0);
            tableLayoutPanel.Controls.Add(tableLayoutPanel1, 0, 1);
            tableLayoutPanel.Dock = DockStyle.Fill;
            tableLayoutPanel.Location = new Point(0, 0);
            tableLayoutPanel.Name = "tableLayoutPanel";
            tableLayoutPanel.RowCount = 2;
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 15.8783779F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 84.12162F));
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel.Size = new Size(434, 296);
            tableLayoutPanel.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 59.02778F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40.97222F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 264F));
            tableLayoutPanel2.Controls.Add(applyBtn, 2, 0);
            tableLayoutPanel2.Controls.Add(portNum, 1, 0);
            tableLayoutPanel2.Controls.Add(portLabel, 0, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 3);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel2.Size = new Size(428, 41);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(recvMsgText, 0, 1);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(3, 50);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 11.9341564F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 88.06584F));
            tableLayoutPanel1.Size = new Size(428, 243);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(434, 296);
            Controls.Add(tableLayoutPanel);
            Font = new Font("Segoe UI", 10F);
            Name = "Main";
            Text = "Android Redirect Notification Server";
            recvMsgMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)portNum).EndInit();
            tableLayoutPanel.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TextBox recvMsgText;
        private Label portLabel;
        private NumericUpDown portNum;
        private Label label1;
        private Button applyBtn;
        private ContextMenuStrip recvMsgMenu;
        private ToolStripMenuItem recvMsgMenu_Copy;
        private ToolStripMenuItem recvMsgMenu_SelectAll;
        private ToolStripMenuItem recvMsgMenu_ClearAll;
        private TableLayoutPanel tableLayoutPanel;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel1;
    }
}
