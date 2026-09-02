using Microsoft.Toolkit.Uwp.Notifications;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using System.Windows.Forms;
using Windows.UI.Notifications;

namespace AndroidRedirectNotification
{
    internal partial class Main : Form
    {
        private object dgvLock;
        private Settings settings;
        private MyTcpListener myTcpListener;
        private DuplicatedNotificationTracker duplicatedNotificationTracker;

        public Main()
        {
            InitializeComponent();
            this.dgvLock = new object();
            this.dgv.CellMouseDown += dgv_CellMouseDown;
            this.dgv.CellMouseDown += dgv_CellMouseDown2;
            try
            {
                this.settings = Settings.ReadSettings()!;
                if (this.settings.SkipDuplicateMsgMs > 99999)
                    this.settings.SkipDuplicateMsgMs = 99999;
                else if (this.settings.SkipDuplicateMsgMs < 100)
                    this.settings.SkipDuplicateMsgMs = 2000;
            }
            catch (Exception ex)
            {
                ExceptionRecord.AddExceptionRecord(ex);
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
                this.duplicatedNotificationTracker = new DuplicatedNotificationTracker(new TimeSpan(0, 0, 0, 0, settings.SkipDuplicateMsgMs), 120000);
            }
            _ = this.RestartTcpListenerAsync();
        }

        private async Task<bool> RestartTcpListenerAsync()
        {
            if (this.myTcpListener != null)
                await this.myTcpListener.StopAsync();

            ushort port = this.settings.Port;
            try
            {
                this.myTcpListener = new MyTcpListener(port);
                this.myTcpListener.OnMessageReceived += MyTcpListener_OnMessageReceived;
                this.myTcpListener.Start();
            }
            catch (Exception ex)
            {
                ExceptionRecord.AddExceptionRecord(ex);
                MessageBox.Show($"Cannot start server with port: {port}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void MyTcpListener_OnMessageReceived(MyNotificationData data)
        {
            try
            {
                string appName = data.AppName;
                if (string.IsNullOrEmpty(appName))
                    appName = data.PackageName;

                long recvTime = Program.ApplicationTime.ElapsedMilliseconds;
                bool isDuplicated = duplicatedNotificationTracker.IsDuplicate(data);
                bool addNewMessage = !this.settings.SkipDuplicateMsg || (this.settings.SkipDuplicateMsg && !isDuplicated);

                if (addNewMessage)
                {
                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    };

                    this.Invoke(() =>
                    {
                        try
                        {
                            lock (this.dgvLock)
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
                                row.Cells["dgvData"].Value = data;
                            }
                        }
                        catch (Exception ex) { ExceptionRecord.AddExceptionRecord(ex); }
                    });
                    if (settings.ShowWindowsNotification &&
                        data.Category != NotificationCategory.CategoryTransport && !data.Flags.Contains("OngoingEvent"))
                    {
                        //if (this.lastRecvTime <= 0 || (recvTime - this.lastRecvTime > 1500))
                        ShowWindowsNotification($"({appName}) {data.Title}", data.Message);
                    }
                }
            }
            catch (Exception ex) { ExceptionRecord.AddExceptionRecord(ex); }
        }

        //public void ShowWindowsNotification(string title, string message)
        //{
        //    new ToastContentBuilder()
        //        .AddText(title)
        //        .AddText(message)
        //        .Show();
        //}

        private void ShowWindowsNotificationThreadUnsafe(string title, string message)
        {
            try
            {
                new ToastContentBuilder()
                    .AddText(title)
                    .AddText(message)
                    .Show();
            }
            catch (Exception ex) { ExceptionRecord.AddExceptionRecord(ex); }
        }

        public void ShowWindowsNotification(string title, string message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(() =>
                {
                    ShowWindowsNotificationThreadUnsafe(title, message);
                });
            }
            else
            {
                ShowWindowsNotificationThreadUnsafe(title, message);
            }
        }

        private void recvMsgMenu_SelectAll_Click(object? sender, EventArgs e)
        {
            lock (this.dgvLock)
            {
                this.dgv.SelectAll();
            }
        }

        private void recvMsgMenu_ClearAll_Click(object? sender, EventArgs e)
        {
            lock (this.dgvLock)
            {
                this.dgv.Rows.Clear();
            }
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

            this.duplicatedNotificationTracker.Window = new TimeSpan(0, 0, 0, 0, newSettings.SkipDuplicateMsgMs);
            if (oldSettings.Port != newSettings.Port)
            {
                _ = this.RestartTcpListenerAsync();
            }
        }

        private void dgv_CellMouseDown(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                lock (this.dgvLock)
                {
                    this.dgv.ClearSelection();
                    this.dgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
                    this.dgv.CurrentCell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
                }
            }
        }

        private void dgv_CellMouseDown2(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right || e.RowIndex < 0)
                return;

            ContextMenuStrip menu = new ContextMenuStrip();
            {
                lock (this.dgvLock)
                {
                    var row = dgv.Rows[e.RowIndex];

                    if (e.ColumnIndex == 9)
                    {
                        menu.Items.Add("Show Message", null, (_s, _e) =>
                        {
                            var data = (MyNotificationData)dgv.Rows[e.RowIndex].Cells["dgvData"].Value;
                            ViewMsgForm viewTextForm = new ViewMsgForm(data.Message, new List<string> { data.PictureIcon, data.Picture });
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

        private void exceptionHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new ExcerptionRecordViewer();
            form.Show();
        }
    }
}
