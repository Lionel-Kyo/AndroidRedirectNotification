using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using AndroidRedirectNotification.Models;

namespace AndroidRedirectNotification
{
    public partial class NotificationDetailPage : ContentPage
    {
        private readonly NotificationItemModel _itemModel;
        private readonly ObservableCollection<HistorySnapshotViewModel> _historyList = new();
        private MyNotificationData? _currentSnapshot;
        private bool _isUpdatingSelection;

        public NotificationDetailPage(NotificationItemModel item)
        {
            InitializeComponent();
            _itemModel = item;

            historyCollectionView.ItemsSource = _historyList;

            LoadSnapshots();
        }

        private void LoadSnapshots()
        {
            _historyList.Clear();

            var snapshots = _itemModel.History != null && _itemModel.History.Count > 0
                ? _itemModel.History
                : new List<MyNotificationData> { _itemModel.LatestData };

            historyCountLabel.Text = snapshots.Count == 1 ? "1 event" : $"{snapshots.Count} events";

            foreach (var snap in snapshots)
            {
                string status = NotificationItemModel.GetEventStatus(snap);
                string color = snap.IsRemoved ? "#FFB4AB" : (snap.IsUpdate ? "#80D8FF" : "#81C784");
                string bg = snap.IsRemoved ? "#3D1A1C" : (snap.IsUpdate ? "#132D40" : "#1B3B22");

                _historyList.Add(new HistorySnapshotViewModel
                {
                    Data = snap,
                    TimeText = snap.GetDateTime().ToString("HH:mm:ss"),
                    StatusText = status,
                    StatusColor = color,
                    StatusBackground = bg,
                    Summary = $"{snap.Title}: {snap.Message}"
                });
            }

            // Initially select the latest snapshot
            var latest = snapshots.LastOrDefault() ?? _itemModel.LatestData;
            ApplySnapshotToView(latest);

            _isUpdatingSelection = true;
            try
            {
                historyCollectionView.SelectedItem = _historyList.LastOrDefault();
            }
            finally
            {
                _isUpdatingSelection = false;
            }
        }

        private void ApplySnapshotToView(MyNotificationData snapshot)
        {
            _currentSnapshot = snapshot;

            string appDisplay = !string.IsNullOrWhiteSpace(snapshot.AppName)
                ? snapshot.AppName
                : (!string.IsNullOrWhiteSpace(snapshot.PackageName) ? snapshot.PackageName : "Unknown App");

            appAndIdLabel.Text = $"{appDisplay}  •  #{snapshot.Id}";

            string status = NotificationItemModel.GetEventStatus(snapshot);
            statusLabel.Text = status;

            string statusColorHex = snapshot.IsRemoved ? "#FFB4AB" : (snapshot.IsUpdate ? "#80D8FF" : "#81C784");
            string statusBgHex = snapshot.IsRemoved ? "#3D1A1C" : (snapshot.IsUpdate ? "#132D40" : "#1B3B22");

            Color statusColor = Color.FromArgb(statusColorHex);
            Color statusBg = Color.FromArgb(statusBgHex);

            statusLabel.TextColor = statusColor;
            statusBadge.BackgroundColor = statusBg;
            statusBadge.Stroke = new SolidColorBrush(statusColor);

            titleLabel.Text = string.IsNullOrWhiteSpace(snapshot.Title) ? "(No Title)" : snapshot.Title;
            timestampLabel.Text = $"{snapshot.GetDateTime():yyyy-MM-dd HH:mm:ss} (UTC {DateTimeOffset.FromUnixTimeSeconds(snapshot.TimeStamp):yyyy-MM-dd HH:mm:ss})";

            messageLabel.Text = string.IsNullOrWhiteSpace(snapshot.Message) ? "(No message body)" : snapshot.Message;

            // Load images
            bool hasPicture = !string.IsNullOrWhiteSpace(snapshot.Picture);
            bool hasIcon = !string.IsNullOrWhiteSpace(snapshot.PictureIcon);

            pictureBorder.IsVisible = false;
            iconBorder.IsVisible = false;

            if (hasPicture)
            {
                var pictureSource = Base64ToImageSource(snapshot.Picture);
                if (pictureSource != null)
                {
                    pictureImage.Source = pictureSource;
                    pictureBorder.IsVisible = true;
                }
            }

            if (hasIcon)
            {
                var iconSource = Base64ToImageSource(snapshot.PictureIcon);
                if (iconSource != null)
                {
                    iconImage.Source = iconSource;
                    iconBorder.IsVisible = true;
                }
            }

            imagesCard.IsVisible = pictureBorder.IsVisible || iconBorder.IsVisible;

            // Technical details
            string flagsText = snapshot.Flags != null && snapshot.Flags.Count > 0 ? string.Join(", ", snapshot.Flags) : "None";
            string actionsText = snapshot.ActionTitles != null && snapshot.ActionTitles.Count > 0 ? string.Join(", ", snapshot.ActionTitles) : "None";

            var sb = new StringBuilder();
            sb.AppendLine($"Status:      {status}");
            sb.AppendLine($"SbnKey:      {snapshot.SbnKey}");
            sb.AppendLine($"Package:     {snapshot.PackageName}");
            sb.AppendLine($"AppName:     {snapshot.AppName}");
            sb.AppendLine($"Id:          {snapshot.Id}");
            sb.AppendLine($"Tag:         {(string.IsNullOrWhiteSpace(snapshot.Tag) ? "(None)" : snapshot.Tag)}");
            sb.AppendLine($"Category:    {(string.IsNullOrWhiteSpace(snapshot.Category) ? "(None)" : snapshot.Category)}");
            sb.AppendLine($"Importance:  {snapshot.Importantce}");
            sb.AppendLine($"Flags:       {flagsText}");
            sb.AppendLine($"Actions:     {actionsText}");

            metadataLabel.Text = sb.ToString();
        }

        private static ImageSource? Base64ToImageSource(string base64)
        {
            if (string.IsNullOrWhiteSpace(base64))
                return null;

            try
            {
                byte[] bytes = Convert.FromBase64String(base64);
                return ImageSource.FromStream(() => new MemoryStream(bytes));
            }
            catch
            {
                return null;
            }
        }

        private void OnHistorySelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (_isUpdatingSelection)
                return;

            if (e.CurrentSelection.FirstOrDefault() is HistorySnapshotViewModel selected)
            {
                ApplySnapshotToView(selected.Data);
            }
        }

        private async void OnBackButtonClicked(object? sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void OnCopyMessageClicked(object? sender, EventArgs e)
        {
            string message = _currentSnapshot?.Message ?? "";
            if (!string.IsNullOrEmpty(message))
            {
                await Clipboard.Default.SetTextAsync(message);
                await DisplayAlert("Copied", "Message copied to clipboard.", "OK");
            }
        }

        private async void OnCopyMetadataClicked(object? sender, EventArgs e)
        {
            string metadata = metadataLabel.Text ?? "";
            if (!string.IsNullOrEmpty(metadata))
            {
                await Clipboard.Default.SetTextAsync(metadata);
                await DisplayAlert("Copied", "Technical details copied to clipboard.", "OK");
            }
        }

        private async void OnCopyAllClicked(object? sender, EventArgs e)
        {
            if (_currentSnapshot == null)
                return;

            var sb = new StringBuilder();
            sb.AppendLine($"[{_currentSnapshot.AppName}] {_currentSnapshot.Title}");
            sb.AppendLine($"Timestamp: {_currentSnapshot.GetDateTime():yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Message: {_currentSnapshot.Message}");
            sb.AppendLine();
            sb.AppendLine("--- Technical Metadata ---");
            sb.AppendLine(metadataLabel.Text);

            await Clipboard.Default.SetTextAsync(sb.ToString());
            await DisplayAlert("Copied", "Complete notification record copied to clipboard.", "OK");
        }
    }

    public class HistorySnapshotViewModel
    {
        public MyNotificationData Data { get; set; } = null!;
        public string TimeText { get; set; } = string.Empty;
        public string StatusText { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
        public string StatusBackground { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
    }
}
