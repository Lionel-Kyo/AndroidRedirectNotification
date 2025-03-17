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
            ToolStripMenuItem menu_Settings;
            menu_Settings_General = new ToolStripMenuItem();
            dgvMenu = new ContextMenuStrip(components);
            recvMsgMenu_SelectAll = new ToolStripMenuItem();
            recvMsgMenu_ClearAll = new ToolStripMenuItem();
            label1 = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            dgv = new DataGridView();
            menu = new MenuStrip();
            dgvId = new DataGridViewTextBoxColumn();
            dgvTag = new DataGridViewTextBoxColumn();
            dgvPackageName = new DataGridViewTextBoxColumn();
            dgvAppName = new DataGridViewTextBoxColumn();
            dgvTitle = new DataGridViewTextBoxColumn();
            dgvCategory = new DataGridViewTextBoxColumn();
            dgvImportantce = new DataGridViewTextBoxColumn();
            dgvActionTitles = new DataGridViewTextBoxColumn();
            dgvFlags = new DataGridViewTextBoxColumn();
            dgvMessage = new DataGridViewTextBoxColumn();
            menu_Settings = new ToolStripMenuItem();
            dgvMenu.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgv).BeginInit();
            menu.SuspendLayout();
            SuspendLayout();
            // 
            // menu_Settings
            // 
            menu_Settings.DropDownItems.AddRange(new ToolStripItem[] { menu_Settings_General });
            menu_Settings.Name = "menu_Settings";
            menu_Settings.Size = new Size(78, 25);
            menu_Settings.Text = "Settings";
            // 
            // menu_Settings_General
            // 
            menu_Settings_General.Name = "menu_Settings_General";
            menu_Settings_General.Size = new Size(134, 26);
            menu_Settings_General.Text = "General";
            menu_Settings_General.Click += menu_Settings_General_Click;
            // 
            // dgvMenu
            // 
            dgvMenu.Items.AddRange(new ToolStripItem[] { recvMsgMenu_SelectAll, recvMsgMenu_ClearAll });
            dgvMenu.Name = "recvMsgMenu";
            dgvMenu.Size = new Size(123, 48);
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
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Location = new Point(4, 1);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(147, 21);
            label1.TabIndex = 0;
            label1.Text = "Received Messages:";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(dgv, 0, 1);
            tableLayoutPanel1.Location = new Point(0, 33);
            tableLayoutPanel1.Margin = new Padding(4);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 8.178438F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 91.82156F));
            tableLayoutPanel1.Size = new Size(984, 277);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // dgv
            // 
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.BackgroundColor = Color.White;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.Columns.AddRange(new DataGridViewColumn[] { dgvId, dgvTag, dgvPackageName, dgvAppName, dgvTitle, dgvCategory, dgvImportantce, dgvActionTitles, dgvFlags, dgvMessage });
            dgv.ContextMenuStrip = dgvMenu;
            dgv.Dock = DockStyle.Fill;
            dgv.Location = new Point(3, 25);
            dgv.Name = "dgv";
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.Size = new Size(978, 249);
            dgv.TabIndex = 0;
            // 
            // menu
            // 
            menu.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            menu.Items.AddRange(new ToolStripItem[] { menu_Settings });
            menu.Location = new Point(0, 0);
            menu.Name = "menu";
            menu.Padding = new Padding(8, 2, 0, 2);
            menu.Size = new Size(984, 29);
            menu.TabIndex = 1;
            menu.Text = "menuStrip1";
            // 
            // dgvId
            // 
            dgvId.HeaderText = "Id";
            dgvId.Name = "dgvId";
            dgvId.ReadOnly = true;
            dgvId.Width = 80;
            // 
            // dgvTag
            // 
            dgvTag.HeaderText = "Tag";
            dgvTag.Name = "dgvTag";
            dgvTag.ReadOnly = true;
            dgvTag.Width = 80;
            // 
            // dgvPackageName
            // 
            dgvPackageName.HeaderText = "Package Name";
            dgvPackageName.Name = "dgvPackageName";
            dgvPackageName.ReadOnly = true;
            // 
            // dgvAppName
            // 
            dgvAppName.HeaderText = "App Name";
            dgvAppName.Name = "dgvAppName";
            dgvAppName.ReadOnly = true;
            // 
            // dgvTitle
            // 
            dgvTitle.HeaderText = "Title";
            dgvTitle.Name = "dgvTitle";
            dgvTitle.ReadOnly = true;
            // 
            // dgvCategory
            // 
            dgvCategory.HeaderText = "Category";
            dgvCategory.Name = "dgvCategory";
            dgvCategory.ReadOnly = true;
            // 
            // dgvImportantce
            // 
            dgvImportantce.HeaderText = "Importantce";
            dgvImportantce.Name = "dgvImportantce";
            dgvImportantce.ReadOnly = true;
            // 
            // dgvActionTitles
            // 
            dgvActionTitles.HeaderText = "Action Titles";
            dgvActionTitles.Name = "dgvActionTitles";
            dgvActionTitles.ReadOnly = true;
            // 
            // dgvFlags
            // 
            dgvFlags.HeaderText = "Flags";
            dgvFlags.Name = "dgvFlags";
            dgvFlags.ReadOnly = true;
            dgvFlags.Width = 60;
            // 
            // dgvMessage
            // 
            dgvMessage.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvMessage.HeaderText = "Message";
            dgvMessage.Name = "dgvMessage";
            dgvMessage.ReadOnly = true;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 311);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(menu);
            Font = new Font("Segoe UI", 12F);
            MainMenuStrip = menu;
            Margin = new Padding(4);
            Name = "Main";
            Text = "Android Redirect Notification Server";
            dgvMenu.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgv).EndInit();
            menu.ResumeLayout(false);
            menu.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private ContextMenuStrip dgvMenu;
        private ToolStripMenuItem recvMsgMenu_SelectAll;
        private ToolStripMenuItem recvMsgMenu_ClearAll;
        private TableLayoutPanel tableLayoutPanel1;
        private MenuStrip menu;
        private ToolStripMenuItem menu_Settings;
        private ToolStripMenuItem menu_Settings_General;
        private DataGridView dgv;
        private DataGridViewTextBoxColumn dgvId;
        private DataGridViewTextBoxColumn dgvTag;
        private DataGridViewTextBoxColumn dgvPackageName;
        private DataGridViewTextBoxColumn dgvAppName;
        private DataGridViewTextBoxColumn dgvTitle;
        private DataGridViewTextBoxColumn dgvCategory;
        private DataGridViewTextBoxColumn dgvImportantce;
        private DataGridViewTextBoxColumn dgvActionTitles;
        private DataGridViewTextBoxColumn dgvFlags;
        private DataGridViewTextBoxColumn dgvMessage;
    }
}
