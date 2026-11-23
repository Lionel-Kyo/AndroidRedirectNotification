using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;

namespace AndroidRedirectNotification
{
    internal partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        public SettingsWindow(Settings settings) : this()
        {
            portNum.Value = settings.Port;
            showWindowsNotificationCheck.IsChecked = settings.ShowWindowsNotification;
        }

        private void ApplyBtn_Click(object? sender, RoutedEventArgs e)
        {
            var newSettings = new Settings
            {
                Port = (ushort)(portNum.Value ?? 8080),
                ShowWindowsNotification = showWindowsNotificationCheck.IsChecked ?? false
            };

            try
            {
                Settings.SaveSettings(newSettings);
            }
            catch (Exception ex)
            {
                ExceptionRecord.AddExceptionRecord(ex);
                return;
            }

            Close(newSettings);
        }

        private void SettingsWindow_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close(null);
            }
        }
    }
}
