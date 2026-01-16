namespace AndroidRedirectNotification
{
    partial class ViewMsgForm
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
            webBrowser = new WebBrowser();
            SuspendLayout();
            // 
            // webBrowser
            // 
            webBrowser.Dock = DockStyle.Fill;
            webBrowser.Location = new Point(0, 0);
            webBrowser.Name = "webBrowser";
            webBrowser.Size = new Size(484, 311);
            webBrowser.TabIndex = 0;
            // 
            // ViewMsgForm
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(484, 311);
            Controls.Add(webBrowser);
            Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Margin = new Padding(4);
            Name = "ViewMsgForm";
            ShowIcon = false;
            Text = "View Message";
            KeyDown += ViewMsgForm_KeyDown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private WebBrowser webBrowser;
    }
}