using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

                _ = Dispatcher.UIThread.InvokeAsync(() =>
                {
                    txtPortStatus.Text = $"Port: {port} • Active";
                    txtFooterStatus.Text = $"● Listening on port {port} • X25519/Ed25519 + AES-256-GCM encrypted";
                });
            }
            catch (Exception ex)
            {
                ExceptionRecord.AddExceptionRecord(ex);
                _ = Dispatcher.UIThread.InvokeAsync(() =>
                {
                    txtPortStatus.Text = $"Port: {port} • Error";
                    txtFooterStatus.Text = $"Error binding port {port}: {ex.Message}";
                });
                return false;
            }
            return true;
        }

        private void MyTcpListener_OnMessageReceived(MyNotificationData data)
        {
            try
            {
                int numberOfEvents = NotificationHistoryStore.Record(data);

                string displayName = string.IsNullOrEmpty(data.AppName) ? data.PackageName : data.AppName;

                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    try
                    {
                        var (statusText, statusColor, statusBg) = NotificationRowItem.GetStatusDetails(data);
                        bool hasImages = !string.IsNullOrEmpty(data.Picture) || !string.IsNullOrEmpty(data.PictureIcon);

                        var row = new NotificationRowItem
                        {
                            Time = $"{data.GetDateTime():HH:mm:ss (yyyy-MM-dd)}",
                            Name = displayName,
                            Title = data.Title,
                            Message = data.Message,
                            Status = statusText,
                            StatusColor = statusColor,
                            StatusBackground = statusBg,
                            NumberOfEvents = numberOfEvents,
                            Data = data
                        };

                        string targetKey = data.GetSlotKey();
                        var existingRows = Notifications
                            .Where(x => string.Equals(x.Data.GetSlotKey(), targetKey, StringComparison.Ordinal))
                            .ToList();

                        foreach (var existing in existingRows)
                        {
                            Notifications.Remove(existing);
                        }

                        Notifications.Insert(0, row);
                        txtCountBadge.Text = $"{Notifications.Count} items";
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

        private void SelectAll_Click(object? sender, RoutedEventArgs e)
        {
            dgv.SelectAll();
        }

        private async void ClearAll_Click(object? sender, RoutedEventArgs e)
        {
            if (Notifications.Count <= 0)
                return;

            if (TopLevel.GetTopLevel(this) is not Window owner)
                return;

            var dialog = new Window
            {
                Title = "Confirm Clear",
                Width = 340,
                Height = 140,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false
            };

            var yesBtn = new Button { Content = "Yes", Width = 80, HorizontalContentAlignment = HorizontalAlignment.Center };
            var noBtn = new Button { Content = "No", Width = 80, HorizontalContentAlignment = HorizontalAlignment.Center };

            yesBtn.Click += (_, _) => dialog.Close(true);
            noBtn.Click += (_, _) => dialog.Close(false);

            dialog.Content = new StackPanel
            {
                Margin = new Thickness(20),
                Spacing = 20,
                Children = {
                    new TextBlock { Text = "Are you sure you want to clear all notifications?" },
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Spacing = 10,
                        Children = { yesBtn, noBtn }
                    }
                }
            };

            bool confirmed = await dialog.ShowDialog<bool>(owner);

            if (confirmed)
            {
                Notifications.Clear();
                NotificationHistoryStore.Clear();
                txtCountBadge.Text = "0 items";
            }
        }

        private async void Settings_General_Click(object? sender, RoutedEventArgs e)
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

        private async void ShowMessage_Click(object? sender, RoutedEventArgs e)
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
