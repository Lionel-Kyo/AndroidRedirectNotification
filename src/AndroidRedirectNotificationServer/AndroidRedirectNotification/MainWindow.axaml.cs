using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security;
using System.Threading.Tasks;

namespace AndroidRedirectNotification
{
    internal partial class MainWindow : Window
    {
        private Settings settings = null!;
        private MyTcpListener? myTcpListener;
        public ObservableCollection<NotificationRowItem> Notifications { get; } = new();

        public MainWindow()
        {
            InitializeComponent();

            dgv.ItemsSource = Notifications;

            try
            {
                var _settings = Settings.ReadSettings();
                settings = _settings ?? new Settings();
            }
            catch (Exception ex)
            {
                ExceptionRecord.AddExceptionRecord(ex);
                settings = new Settings();
            }

            _ = RestartTcpListenerAsync();
        }

        private async Task<bool> RestartTcpListenerAsync()
        {
            if (myTcpListener != null)
                await myTcpListener.StopAsync();

            ushort port = settings.Port;
            try
            {
                myTcpListener = new MyTcpListener(port);
                myTcpListener.OnMessageReceived += MyTcpListener_OnMessageReceived;
                myTcpListener.Start();
            }
            catch (Exception ex)
            {
                ExceptionRecord.AddExceptionRecord(ex);
                return false;
            }
            return true;
        }

        private void MyTcpListener_OnMessageReceived(MyNotificationData data)
        {
            try
            {
                NotificationHistoryStore.Record(data);

                string displayName = string.IsNullOrEmpty(data.AppName) ? data.PackageName : data.AppName;

                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    try
                    {
                        var row = new NotificationRowItem
                        {
                            TimeId = $"{data.GetDateTime():HH:mm:ss (yyyy-MM-dd)} ({data.Id})",
                            Name = displayName,
                            Title = data.Title,
                            Message = data.Message,
                            Data = data
                        };

                        //if (data.IsUpdate || data.IsRemoved)
                        string targetKey = data.GetSlotKey();
                        var existingRows = Notifications
                            .Where(x => string.Equals(x.Data.GetSlotKey(), targetKey, StringComparison.Ordinal))
                            .ToList();

                        foreach (var existing in existingRows)
                        {
                            Notifications.Remove(existing);
                        }

                        Notifications.Insert(0, row);
                    }
                    catch (Exception ex) { ExceptionRecord.AddExceptionRecord(ex); }
                });

                if (settings.ShouldShowWindowsToast(data))
                {
                    ShowWindowsNotification($"({displayName}) {data.Title}", data.Message);
                }
            }
            catch (Exception ex) { ExceptionRecord.AddExceptionRecord(ex); }
        }

        public void ShowWindowsNotification(string title, string message)
        {
            try
            {
                string safeTitle = SecurityElement.Escape(title) ?? string.Empty;
                string safeMessage = SecurityElement.Escape(message) ?? string.Empty;

                string xml = $@"<toast>
    <visual>
        <binding template=""ToastGeneric"">
            <text>{safeTitle}</text>
            <text>{safeMessage}</text>
        </binding>
    </visual>
</toast>";

                var xmlDoc = new Windows.Data.Xml.Dom.XmlDocument();
                xmlDoc.LoadXml(xml);
                var toast = new Windows.UI.Notifications.ToastNotification(xmlDoc);

                toast.Failed += (s, e) =>
                {
                    ExceptionRecord.AddExceptionRecord(new Exception($"Toast failed to display: 0x{e.ErrorCode:X8}"));
                };

                Windows.UI.Notifications.ToastNotificationManager.CreateToastNotifier(ToastHelper.AppId).Show(toast);
            }
            catch (Exception ex)
            {
                ExceptionRecord.AddExceptionRecord(ex);
            }
        }

        private void RecvMsgMenu_SelectAll_Click(object? sender, RoutedEventArgs e)
        {
            dgv.SelectAll();
        }

        private void RecvMsgMenu_ClearAll_Click(object? sender, RoutedEventArgs e)
        {
            Notifications.Clear();
            NotificationHistoryStore.Clear();
        }

        private async void Menu_Settings_General_Click(object? sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(settings);
            var newSettings = await settingsWindow.ShowDialog<Settings?>(this);

            if (newSettings == null)
                return;

            Settings oldSettings = settings;
            settings = newSettings;

            if (oldSettings.Port != newSettings.Port)
            {
                _ = RestartTcpListenerAsync();
            }
        }

        private async void Menu_ShowMessage_Click(object? sender, RoutedEventArgs e)
        {
            if (dgv.SelectedItem is NotificationRowItem selectedRow)
            {
                var data = selectedRow.Data;
                var viewMsgWindow = new ViewMsgWindow(data);
                await viewMsgWindow.ShowDialog(this);
            }
        }

        private void ExceptionHistory_Click(object? sender, RoutedEventArgs e)
        {
            var window = new ExceptionRecordViewerWindow();
            window.Show();
        }
    }
}
