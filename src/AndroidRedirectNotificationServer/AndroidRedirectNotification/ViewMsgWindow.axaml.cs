using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AndroidRedirectNotification
{
    internal partial class ViewMsgWindow : Window
    {
        private bool _isLoadingHistory;
        private List<MyNotificationData> _historyEntries = new();

        public ViewMsgWindow()
        {
            InitializeComponent();
        }

        public ViewMsgWindow(MyNotificationData data) : this()
        {
            ApplySnapshotToView(data);
            LoadHistoryEntries(data);
        }

        private void ApplySnapshotToView(MyNotificationData item)
        {
            string displayName = string.IsNullOrEmpty(item.AppName) ? item.PackageName : item.AppName;
            txtTitle1.Text = $"{displayName}  •  {item.GetDateTime():yyyy-MM-dd HH:mm:ss}  (#{item.Id})";
            txtTitle1.IsVisible = !string.IsNullOrWhiteSpace(item.Title);
            txtTitle2.Text = item.Title ?? string.Empty;
            txtTitle2.IsVisible = !string.IsNullOrWhiteSpace(item.Title);
            txtMessage.Text = item.Message ?? string.Empty;

            var (statusText, statusColorHex, statusBgHex) = NotificationRowItem.GetStatusDetails(item);
            txtStatusBadge.Text = statusText;
            txtStatusBadge.Foreground = SolidColorBrush.Parse(statusColorHex);
            badgeStatus.Background = SolidColorBrush.Parse(statusBgHex);
            badgeStatus.BorderBrush = SolidColorBrush.Parse(statusColorHex);

            var images = new List<string>();
            if (!string.IsNullOrWhiteSpace(item.Picture)) images.Add(item.Picture);
            if (!string.IsNullOrWhiteSpace(item.PictureIcon)) images.Add(item.PictureIcon);
            LoadImages(images);

            string flagsText = item.Flags != null && item.Flags.Count > 0 ? string.Join(", ", item.Flags) : "None";
            string actionsText = item.ActionTitles != null && item.ActionTitles.Count > 0 ? string.Join(", ", item.ActionTitles) : "None";

            txtMetadata.Text =
                $"Status:      {statusText}\n" +
                $"SbnKey:      {item.SbnKey}\n" +
                $"Title:       {item.Title}\n" +
                $"AppName:     {item.AppName}\n" +
                $"Package:     {item.PackageName}\n" +
                $"Id:          {item.Id}\n" +
                $"Tag:         {(string.IsNullOrEmpty(item.Tag) ? "(None)" : item.Tag)}\n" +
                $"Category:    {(string.IsNullOrEmpty(item.Category) ? "(None)" : item.Category)}\n" +
                $"Importance:  {item.Importantce}\n" +
                $"Flags:       {flagsText}\n" +
                $"Actions:     {actionsText}";
        }

        private void LoadHistoryEntries(MyNotificationData currentData)
        {
            _isLoadingHistory = true;
            try
            {
                _historyEntries = NotificationHistoryStore.GetHistory(currentData).ToList();

                var lines = _historyEntries.Select(item =>
                    $"[{item.GetDateTime():HH:mm:ss}] [{NotificationRowItem.GetStatusDetails(item).text}] {item.Title}: {item.Message}")
                    .ToList();

                lstHistory.ItemsSource = lines;

                int matchIndex = _historyEntries.IndexOf(currentData);
                if (matchIndex >= 0)
                {
                    lstHistory.SelectedIndex = matchIndex;
                }
            }
            finally
            {
                _isLoadingHistory = false;
            }
        }

        private void LoadImages(List<string>? base64Images)
        {
            if (base64Images == null)
                return;

            var bitmaps = new List<Bitmap>();
            foreach (var base64 in base64Images)
            {
                if (string.IsNullOrWhiteSpace(base64))
                    continue;

                try
                {
                    byte[] bytes = Convert.FromBase64String(base64);
                    using var ms = new MemoryStream(bytes);
                    bitmaps.Add(new Bitmap(ms));
                }
                catch (Exception ex)
                {
                    ExceptionRecord.AddExceptionRecord(ex);
                }
            }

            imgContainer.ItemsSource = bitmaps;
        }

        private void ViewMsgWindow_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void LstHistory_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_isLoadingHistory)
                return;

            if (lstHistory.SelectedIndex >= 0 && lstHistory.SelectedIndex < _historyEntries.Count)
            {
                ApplySnapshotToView(_historyEntries[lstHistory.SelectedIndex]);
            }
        }

        private async void CopyMessage_Click(object? sender, RoutedEventArgs e)
        {
            string? text = txtMessage.Text;
            if (!string.IsNullOrEmpty(text))
            {
                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel?.Clipboard != null)
                {
                    await topLevel.Clipboard.SetTextAsync(text);
                }
            }
        }

        private async void CopyMetadata_Click(object? sender, RoutedEventArgs e)
        {
            string? text = txtMetadata.Text;
            if (!string.IsNullOrEmpty(text))
            {
                var topLevel = TopLevel.GetTopLevel(this);
                if (topLevel?.Clipboard != null)
                {
                    await topLevel.Clipboard.SetTextAsync(text);
                }
            }
        }
    }
}
