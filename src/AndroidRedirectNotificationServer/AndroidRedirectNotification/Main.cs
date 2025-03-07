using Microsoft.Toolkit.Uwp.Notifications;
using System.Diagnostics;
using System.Text.Json;
using Windows.UI.Notifications;

namespace AndroidRedirectNotification
{
    internal partial class Main : Form
    {
        private static readonly NotificationFlags[] NotificationFlagsValues = Enum.GetValues(typeof(NotificationFlags)).Cast<NotificationFlags>().ToArray();
        private Settings settings;
        private MyTcpListener myTcpListener;
        private long lastRecvTime;
        public Main()
        {
            InitializeComponent();
            try
            {
                this.settings = Settings.ReadSettings()!;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Read Settings Failed.\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
            finally
            {
                // For disabling constructor null members
                if (this.settings == null)
                    this.settings = new Settings();
                this.myTcpListener = null!;
            }
            this.RestartTcpListener();
        }

        private bool RestartTcpListener()
        {
            if (this.myTcpListener != null)
                this.myTcpListener.Dispose();

            ushort port = this.settings.Port;
            this.myTcpListener = new MyTcpListener(port);
            try
            {
                this.myTcpListener.Start();
                this.myTcpListener.OnMessageReceived += MyTcpListener_OnMessageReceived;
            }
            catch
            {
                MessageBox.Show($"Cannot start server with port: {port}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void MyTcpListener_OnMessageReceived(MyNotificationData data)
        {
            string appName = data.AppName;
            if (string.IsNullOrEmpty(appName))
                appName = data.PackageName;

            long recvTime = Program.ApplicationTime.ElapsedMilliseconds;

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            this.Invoke(() =>
            {
                int i = this.dgv.Rows.Add();
                DataGridViewRow row = this.dgv.Rows[i];
                row.Cells["dgvPackageName"].Value = data.PackageName;
                row.Cells["dgvAppName"].Value = data.AppName;
                row.Cells["dgvTitle"].Value = data.Title;
                row.Cells["dgvMessage"].Value = data.Message;
                row.Cells["dgvCategory"].Value = data.Category;
                row.Cells["dgvActionTitles"].Value = string.Join(", ", data.ActionTitles);
                row.Cells["dgvImportantce"].Value = data.Importantce;
                row.Cells["dgvFlags"].Value = string.Join(", ", NotificationFlagsValues.Where(f => data.Flags.HasFlag(f)).Select(f => f.ToString()));
            });
            if (data.Category != NotificationCategory.CategoryTransport && !data.Flags.HasFlag(NotificationFlags.OngoingEvent))
            {
                //if (this.lastRecvTime <= 0 || (recvTime - this.lastRecvTime > 1500))
                ShowWindowsNotification($"({appName}) {data.Title}", data.Message);

            }
            this.lastRecvTime = recvTime;
        }

        public void ShowWindowsNotification(string title, string message)
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .Show();
        }

        private void recvMsgMenu_SelectAll_Click(object sender, EventArgs e)
        {
            this.dgv.SelectAll();
        }

        private void recvMsgMenu_ClearAll_Click(object sender, EventArgs e)
        {
            this.dgv.Rows.Clear();
        }

        private void menu_Settings_General_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm(this.settings);
            settingsForm.StartPosition = FormStartPosition.CenterParent;
            settingsForm.ShowDialog();
            Settings oldSettings = this.settings;
            Settings? newSettings = settingsForm.Value;
            if (newSettings == null)
                return;
            this.settings = newSettings;

            if (oldSettings.Port != newSettings.Port)
            {
                this.RestartTcpListener();
            }
        }
    }
}
