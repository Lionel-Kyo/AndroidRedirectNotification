using Microsoft.Toolkit.Uwp.Notifications;
using System.Diagnostics;
using Windows.UI.Notifications;

namespace AndroidRedirectNotification_PC
{
    public partial class Main : Form
    {
        private MyTcpListener myTcpListener;
        private long lastRecvTime;
        public Main()
        {
            InitializeComponent();
            this.recvMsgText.Text = "";
            this.myTcpListener = new MyTcpListener((ushort)this.portNum.Value);
            try
            {
                this.myTcpListener.Start();
                this.myTcpListener.OnMessageReceived += MyTcpListener_OnMessageReceived;
            }
            catch 
            {
                MessageBox.Show($"Cannot start server with port: {(ushort)this.portNum.Value}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MyTcpListener_OnMessageReceived(string packetName, string appName, string title, string message)
        {
            if (string.IsNullOrEmpty(appName))
                appName = packetName;

            long recvTime = Program.ApplicationTime.ElapsedMilliseconds;
            this.Invoke(() => this.recvMsgText.AppendText($"({appName}) {title}: {message}{Environment.NewLine}"));
            if (this.lastRecvTime <= 0 || (recvTime - this.lastRecvTime > 1500))
            {
                ShowWindowsNotification($"({appName}) {title}", message);
            }
            this.lastRecvTime = recvTime;
        }

        public void ShowWindowsNotification(string title, string message)
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .Show();

            //string toastXml = $@"
            //<toast>
            //    <visual>
            //        <binding template='ToastGeneric'>
            //            <text>{title}</text>
            //            <text>{message}</text>
            //        </binding>
            //    </visual>
            //</toast>";
            //Windows.Data.Xml.Dom.XmlDocument xmlDoc = new Windows.Data.Xml.Dom.XmlDocument();
            //xmlDoc.LoadXml(toastXml);

            //ToastNotification toast = new ToastNotification(xmlDoc);
            //ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private void applyBtn_Click(object sender, EventArgs e)
        {
            this.myTcpListener?.Stop();
            this.myTcpListener = new MyTcpListener((ushort)this.portNum.Value);
            try
            {
                this.myTcpListener.Start();
                this.myTcpListener.OnMessageReceived += MyTcpListener_OnMessageReceived;
            }
            catch
            {
                MessageBox.Show($"Cannot start server with port: {(ushort)this.portNum.Value}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void recvMsgMenu_Copy_Click(object sender, EventArgs e)
        {
            string? selectedText = this.recvMsgText.Text;
            if (!string.IsNullOrEmpty(selectedText)) 
                Clipboard.SetText(selectedText);
        }

        private void recvMsgMenu_SelectAll_Click(object sender, EventArgs e)
        {
            this.recvMsgText.SelectAll();
        }

        private void recvMsgMenu_ClearAll_Click(object sender, EventArgs e)
        {
            this.recvMsgText.Clear();
        }
    }
}
