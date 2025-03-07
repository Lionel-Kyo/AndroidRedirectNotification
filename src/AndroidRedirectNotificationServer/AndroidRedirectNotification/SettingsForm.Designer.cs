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
            applyBtn = new Button();
            portNum = new NumericUpDown();
            portLabel = new Label();
            settingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)portNum).BeginInit();
            SuspendLayout();
            // 
            // settingsPanel
            // 
            settingsPanel.AutoScroll = true;
            settingsPanel.Controls.Add(applyBtn);
            settingsPanel.Controls.Add(portNum);
            settingsPanel.Controls.Add(portLabel);
            settingsPanel.Dock = DockStyle.Fill;
            settingsPanel.Location = new Point(0, 0);
            settingsPanel.Name = "settingsPanel";
            settingsPanel.Size = new Size(484, 361);
            settingsPanel.TabIndex = 0;
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
            ((System.ComponentModel.ISupportInitialize)portNum).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel settingsPanel;
        private NumericUpDown portNum;
        private Label portLabel;
        private Button applyBtn;
    }
}