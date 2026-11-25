using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
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
            showWindowsToastCheck.IsChecked = settings.ShowWindowsToast;
            showToastOnUpdateCheck.IsChecked = settings.ShowToastOnUpdate;
            SelectMatchingItems(blockedCategoriesList, settings.BlockedCategories);
            SelectMatchingItems(blockedFlagsList, settings.BlockedFlags);
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            Dispatcher.UIThread.Post(() =>
            {
                mainScrollViewer.Offset = new Vector(0, 0);
            }, DispatcherPriority.Loaded);
        }

        private static void SelectMatchingItems(ListBox listBox, List<string> selectedValues)
        {
            listBox.SelectedItems?.Clear();
            foreach (ListBoxItem? item in listBox.Items)
            {
                if (item == null) continue;

                if (item.Content is string text &&
                    selectedValues.Contains(text, StringComparer.OrdinalIgnoreCase))
                {
                    listBox.SelectedItems?.Add(item);
                }
            }
        }

        private static List<string> GetSelectedContents(ListBox listBox)
        {
            return listBox.SelectedItems?
                .OfType<ListBoxItem>()
                .Select(item => item.Content?.ToString())
                .Where(text => !string.IsNullOrEmpty(text))
                .Cast<string>()
                .ToList() ?? new List<string>();
        }

        private void ApplyBtn_Click(object? sender, RoutedEventArgs e)
        {
            var newSettings = new Settings
            {
                Port = (ushort)(portNum.Value ?? 5000),
                ShowWindowsToast = showWindowsToastCheck.IsChecked == true,
                ShowToastOnUpdate = showToastOnUpdateCheck.IsChecked == true,
                BlockedCategories = GetSelectedContents(blockedCategoriesList),
                BlockedFlags = GetSelectedContents(blockedFlagsList)
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

        private void CancelBtn_Click(object? sender, RoutedEventArgs e)
        {
            Close(null);
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
