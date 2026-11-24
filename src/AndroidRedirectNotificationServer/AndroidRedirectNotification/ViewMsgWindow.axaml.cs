using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            txtTitle1.Text = $"{displayName}  •  {item.GetDateTime():HH:mm:ss (yyyy-MM-dd)}  (#{item.Id})";
            txtTitle1.IsVisible = !string.IsNullOrWhiteSpace(item.Title);
            txtTitle2.Text = item.Title ?? "";
            txtTitle2.IsVisible = !string.IsNullOrWhiteSpace(item.Title);
            txtMessage.Text = item.Message ?? string.Empty;

            var images = new List<string>();
            if (!string.IsNullOrWhiteSpace(item.Picture)) images.Add(item.Picture);
            if (!string.IsNullOrWhiteSpace(item.PictureIcon)) images.Add(item.PictureIcon);
            LoadImages(images);


            string flagsText = item.Flags.Count > 0 ? string.Join(", ", item.Flags) : "";
            string actionsText = item.ActionTitles.Count > 0 ? string.Join(", ", item.ActionTitles) : "";

            txtMetadata.Text =
                $"Status: {GetEventStatus(item)}\n" +
                $"SbnKey: {item.SbnKey}\n" +
                $"Title: {item.Title}\n" +
                $"AppName: {item.AppName}\n" +
                $"Package: {item.PackageName}\n" +
                $"Category: {(string.IsNullOrEmpty(item.Category) ? "" : item.Category)}\n" +
                $"Importance: {item.Importantce}\n" +
                $"Tag: {(string.IsNullOrEmpty(item.Tag) ? "" : item.Tag)}\n" +
                $"Flags: {flagsText}\n" +
                $"Actions: {actionsText}";
        }

        private void LoadHistoryEntries(MyNotificationData currentData)
        {
            _isLoadingHistory = true;
            try
            {
                _historyEntries = NotificationHistoryStore.GetHistory(currentData).ToList();

                var lines = _historyEntries.Select(item =>
                    $"[{item.GetDateTime():HH:mm:ss}] {GetEventStatus(item)} — {item.Title}: {item.Message}")
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

        private static string GetEventStatus(MyNotificationData item)
        {
            if (item.IsRemoved) return "Removed";
            return item.IsUpdate ? "Update" : "New";
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
    }
}