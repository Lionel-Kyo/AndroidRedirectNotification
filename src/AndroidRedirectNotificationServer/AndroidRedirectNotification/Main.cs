using Microsoft.Toolkit.Uwp.Notifications;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Forms;
using Windows.UI.Notifications;

namespace AndroidRedirectNotification
{
    internal partial class Main : Form
    {
        private Settings settings;
        private MyTcpListener myTcpListener;
        private long lastRecvTime;
        private MyNotificationData? lastNotificationData;

        public Main()
        {
            InitializeComponent();
            this.dgv.CellMouseDown += dgv_CellMouseDown;
            this.dgv.CellMouseDown += dgv_CellMouseDown2;
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
            bool sameAsLastData = data.Equals(this.lastNotificationData);
            bool addNewMessage = !this.settings.SkipDuplicateMsg ||
                (this.settings.SkipDuplicateMsg && !sameAsLastData ||
                (this.settings.SkipDuplicateMsg && sameAsLastData && (recvTime - this.lastRecvTime >= settings.SkipDuplicateMsgMs)));

            this.lastNotificationData = data;
            this.lastRecvTime = recvTime;

            if (addNewMessage)
            {
                lastNotificationData = data;

                var jsonSerializerOptions = new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                this.Invoke(() =>
                {
                    int i = this.dgv.Rows.Add();
                    DataGridViewRow row = this.dgv.Rows[i];
                    row.Cells["dgvDateTimeId"].Value = $"{data.GetDateTime(): yyyy-MM-dd HH:mm:ss} ({data.Id})";
                    row.Cells["dgvTag"].Value = data.Tag;
                    row.Cells["dgvPackageName"].Value = data.PackageName;
                    row.Cells["dgvAppName"].Value = data.AppName;
                    row.Cells["dgvTitle"].Value = data.Title;
                    row.Cells["dgvMessage"].Value = data.Message;
                    row.Cells["dgvCategory"].Value = data.Category;
                    row.Cells["dgvImportantce"].Value = data.Importantce;
                    row.Cells["dgvActionTitles"].Value = string.Join(", ", data.ActionTitles);
                    row.Cells["dgvFlags"].Value = string.Join(", ", data.Flags);
                });
                if (settings.ShowWindowsNotification &&
                    data.Category != NotificationCategory.CategoryTransport && !data.Flags.Contains("OngoingEvent"))
                {
                    //if (this.lastRecvTime <= 0 || (recvTime - this.lastRecvTime > 1500))
                    ShowWindowsNotification($"({appName}) {data.Title}", data.Message);
                }
            }
        }

        public void ShowWindowsNotification(string title, string message)
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .Show();
        }

        private void recvMsgMenu_SelectAll_Click(object? sender, EventArgs e)
        {
            this.dgv.SelectAll();
        }

        private void recvMsgMenu_ClearAll_Click(object? sender, EventArgs e)
        {
            this.dgv.Rows.Clear();
        }

        private void menu_Settings_General_Click(object? sender, EventArgs e)
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

        private void dgv_CellMouseDown(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                this.dgv.ClearSelection();
                this.dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
                this.dgv.CurrentCell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
            }
        }

        private void dgv_CellMouseDown2(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || e.RowIndex < 0)
                return;

            ContextMenuStrip menu = new ContextMenuStrip();
            {
                var row = dgv.Rows[e.RowIndex];

                if (e.ColumnIndex == 9)
                {
                    menu.Items.Add("Show Message", null, (_s, _e) =>
                    {
                        ViewMsgForm viewTextForm = new ViewMsgForm((string)dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                        viewTextForm.StartPosition = FormStartPosition.CenterParent;
                        viewTextForm.ShowDialog();
                    });
                    menu.Items.Add(new ToolStripSeparator());
                }

                menu.Items.Add("Select All", null, recvMsgMenu_SelectAll_Click);
                menu.Items.Add("Clear All", null, recvMsgMenu_ClearAll_Click);

                menu.Show(Cursor.Position);
            }
        }
    }
}
