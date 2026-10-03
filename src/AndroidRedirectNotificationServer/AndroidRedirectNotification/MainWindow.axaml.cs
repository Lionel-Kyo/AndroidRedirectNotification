using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AndroidRedirectNotification
{
    internal partial class MainWindow : Window
    {
        private Settings settings = null!;
        private MyTcpListener? myTcpListener;
        private DuplicatedNotificationTracker duplicatedNotificationTracker = null!;
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

            duplicatedNotificationTracker = new DuplicatedNotificationTracker(
                TimeSpan.FromMilliseconds(settings.SkipDuplicateMsgMs),
                120000);

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
                string appName = string.IsNullOrEmpty(data.AppName) ? data.PackageName : data.AppName;
                bool isDuplicated = duplicatedNotificationTracker.IsDuplicate(data);
                bool addNewMessage = !settings.SkipDuplicateMsg || !isDuplicated;

                if (addNewMessage)
                {
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        try
                        {
                            Notifications.Add(new NotificationRowItem
                            {
                                DateTimeId = $"{data.GetDateTime(): yyyy-MM-dd HH:mm:ss} ({data.Id})",
                                Tag = data.Tag,
                                PackageName = data.PackageName,
                                AppName = data.AppName,
                                Title = data.Title,
                                Message = data.Message,
                                Category = data.Category.ToString(),
                                Importance = data.Importantce.ToString(),
                                ActionTitles = string.Join(", ", data.ActionTitles),
                                Flags = string.Join(", ", data.Flags),
                                Data = data
                            });
                        }
                        catch (Exception ex) { ExceptionRecord.AddExceptionRecord(ex); }
                    });

                    if (settings.ShowWindowsNotification &&
                        data.Category != NotificationCategory.CategoryTransport &&
                        !data.Flags.Contains("OngoingEvent"))
                    {
                        ShowWindowsNotification($"({appName}) {data.Title}", data.Message);
                    }
                }
            }
            catch (Exception ex) { ExceptionRecord.AddExceptionRecord(ex); }
        }

        public void ShowWindowsNotification(string title, string message)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                try
                {
                    new ToastContentBuilder()
                        .AddText(title)
                        .AddText(message)
                        .Show();
                }
                catch (Exception ex) { ExceptionRecord.AddExceptionRecord(ex); }
            });
        }

        private void RecvMsgMenu_SelectAll_Click(object? sender, RoutedEventArgs e)
        {
            dgv.SelectAll();
        }

        private void RecvMsgMenu_ClearAll_Click(object? sender, RoutedEventArgs e)
        {
            Notifications.Clear();
        }

        private async void Menu_Settings_General_Click(object? sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow(settings);
            var newSettings = await settingsWindow.ShowDialog<Settings?>(this);

            if (newSettings == null)
                return;

            Settings oldSettings = settings;
            settings = newSettings;

            duplicatedNotificationTracker.Window = TimeSpan.FromMilliseconds(newSettings.SkipDuplicateMsgMs);
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
                var viewMsgWindow = new ViewMsgWindow(data.Message, new List<string> { data.PictureIcon, data.Picture });
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