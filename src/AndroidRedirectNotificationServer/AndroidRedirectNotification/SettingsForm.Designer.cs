namespace AndroidRedirectNotification
{
    partial class SettingsForm
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
            settingsPanel = new Panel();
            skipDuplicateMsgLabel = new Label();
            skipDuplicateMsgNum = new NumericUpDown();
            skipDuplicateMsgCheck = new CheckBox();
            applyBtn = new Button();
            portNum = new NumericUpDown();
            portLabel = new Label();
            showWindowsNotificationCheck = new CheckBox();
            settingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)skipDuplicateMsgNum).BeginInit();
            ((System.ComponentModel.ISupportInitialize)portNum).BeginInit();
            SuspendLayout();
            // 
            // settingsPanel
            // 
            settingsPanel.AutoScroll = true;
            settingsPanel.Controls.Add(showWindowsNotificationCheck);
            settingsPanel.Controls.Add(skipDuplicateMsgLabel);
            settingsPanel.Controls.Add(skipDuplicateMsgNum);
            settingsPanel.Controls.Add(skipDuplicateMsgCheck);
            settingsPanel.Controls.Add(applyBtn);
            settingsPanel.Controls.Add(portNum);
            settingsPanel.Controls.Add(portLabel);
            settingsPanel.Dock = DockStyle.Fill;
            settingsPanel.Location = new Point(0, 0);
            settingsPanel.Name = "settingsPanel";
            settingsPanel.Size = new Size(484, 361);
            settingsPanel.TabIndex = 0;
            // 
            // skipDuplicateMsgLabel
            // 
            skipDuplicateMsgLabel.AutoSize = true;
            skipDuplicateMsgLabel.Location = new Point(284, 57);
            skipDuplicateMsgLabel.Name = "skipDuplicateMsgLabel";
            skipDuplicateMsgLabel.Size = new Size(31, 21);
            skipDuplicateMsgLabel.TabIndex = 6;
            skipDuplicateMsgLabel.Text = "ms";
            // 
            // skipDuplicateMsgNum
            // 
            skipDuplicateMsgNum.Location = new Point(208, 53);
            skipDuplicateMsgNum.Maximum = new decimal(new int[] { 99999, 0, 0, 0 });
            skipDuplicateMsgNum.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            skipDuplicateMsgNum.Name = "skipDuplicateMsgNum";
            skipDuplicateMsgNum.Size = new Size(70, 29);
            skipDuplicateMsgNum.TabIndex = 5;
            skipDuplicateMsgNum.TextAlign = HorizontalAlignment.Center;
            skipDuplicateMsgNum.Value = new decimal(new int[] { 1000, 0, 0, 0 });
            // 
            // skipDuplicateMsgCheck
            // 
            skipDuplicateMsgCheck.AutoSize = true;
            skipDuplicateMsgCheck.Location = new Point(12, 54);
            skipDuplicateMsgCheck.Name = "skipDuplicateMsgCheck";
            skipDuplicateMsgCheck.Size = new Size(193, 25);
            skipDuplicateMsgCheck.TabIndex = 4;
            skipDuplicateMsgCheck.Text = "Skip Duplicate Message";
            skipDuplicateMsgCheck.UseVisualStyleBackColor = true;
            // 
            // applyBtn
            // 
            applyBtn.Location = new Point(194, 320);
            applyBtn.Name = "applyBtn";
            applyBtn.Size = new Size(97, 30);
            applyBtn.TabIndex = 3;
            applyBtn.Text = "Apply";
            applyBtn.UseVisualStyleBackColor = true;
            applyBtn.Click += applyBtn_Click;
            // 
            // portNum
            // 
            portNum.Location = new Point(108, 6);
            portNum.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            portNum.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            portNum.Name = "portNum";
            portNum.Size = new Size(70, 29);
            portNum.TabIndex = 1;
            portNum.TextAlign = HorizontalAlignment.Center;
            portNum.Value = new decimal(new int[] { 443, 0, 0, 0 });
            // 
            // portLabel
            // 
            portLabel.AutoSize = true;
            portLabel.Location = new Point(12, 9);
            portLabel.Name = "portLabel";
            portLabel.Size = new Size(90, 21);
            portLabel.TabIndex = 0;
            portLabel.Text = "Server Port:";
            // 
            // showWindowsNotificationCheck
            // 
            showWindowsNotificationCheck.AutoSize = true;
            showWindowsNotificationCheck.Location = new Point(12, 101);
            showWindowsNotificationCheck.Name = "showWindowsNotificationCheck";
            showWindowsNotificationCheck.Size = new Size(222, 25);
            showWindowsNotificationCheck.TabIndex = 7;
            showWindowsNotificationCheck.Text = "Show Windows Notification";
            showWindowsNotificationCheck.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(484, 361);
            Controls.Add(settingsPanel);
            Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4);
            MaximizeBox = false;
            Name = "SettingsForm";
            ShowIcon = false;
            Text = "Settings";
            KeyDown += SettingsForm_KeyDown;
            settingsPanel.ResumeLayout(false);
            settingsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)skipDuplicateMsgNum).EndInit();
            ((System.ComponentModel.ISupportInitialize)portNum).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel settingsPanel;
        private NumericUpDown portNum;
        private Label portLabel;
        private Button applyBtn;
        private CheckBox skipDuplicateMsgCheck;
        private NumericUpDown skipDuplicateMsgNum;
        private Label skipDuplicateMsgLabel;
        private CheckBox showWindowsNotificationCheck;
    }
}