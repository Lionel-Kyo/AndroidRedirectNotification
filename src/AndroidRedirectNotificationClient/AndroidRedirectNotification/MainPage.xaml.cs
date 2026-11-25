using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text.Json;
using AndroidRedirectNotification.Models;

namespace AndroidRedirectNotification
{
    public partial class MainPage : ContentPage
    {
        public static MainPage? Instance { get; private set; }
        public static string ServerIp { get; private set; } = "192.168.1.100";
        public static ushort ServerPort { get; private set; } = 5000;
        public static double KeepHistoryValue { get; private set; } = 12;
        public static string KeepHistoryUnit { get; private set; } = "Hours";
        public static bool IsSendToServerEnabled { get; private set; } = true;
        public static bool SkipDuplicateMessages { get; private set; } = true;
        public static double DuplicateIntervalSeconds { get; private set; } = 2.0;
        public static bool MatchContentOnly { get; private set; } = false;
        public static DuplicateNotificationTracker DuplicateTracker { get; } = new();

        public Entry IpEditEntry => ipEditEntry;
        public Entry PortEntry => portEntry;

        public ObservableCollection<NotificationItemModel> Notifications { get; } = new();

        private string HistoryFilePath => Path.Combine(FileSystem.AppDataDirectory, "NotificationHistory.json");

        public MainPage()
        {
            InitializeComponent();
            Instance = this;

            historyCollectionView.ItemsSource = Notifications;

            ServerIp = Preferences.Get("server_ip", "192.168.1.100");
            ServerPort = (ushort)Preferences.Get("server_port", 5000);
            KeepHistoryValue = Preferences.Get("keep_history_value", 12.0);
            KeepHistoryUnit = Preferences.Get("keep_history_unit", "Hours");
            IsSendToServerEnabled = Preferences.Get("send_to_server_enabled", true);
            SkipDuplicateMessages = Preferences.Get("skip_duplicate_messages", true);
            DuplicateIntervalSeconds = Preferences.Get("duplicate_interval_seconds", 2.0);
            MatchContentOnly = Preferences.Get("match_content_only", false);

            this.ipEditEntry.Text = ServerIp;
            this.portEntry.Text = ServerPort.ToString();
            this.historyValueEntry.Text = KeepHistoryValue.ToString();
            this.historyUnitPicker.SelectedItem = KeepHistoryUnit;
            this.sendToServerSwitch.IsToggled = IsSendToServerEnabled;
            this.skipDuplicateSwitch.IsToggled = SkipDuplicateMessages;
            this.duplicateIntervalEntry.Text = DuplicateIntervalSeconds.ToString();
            this.matchContentOnlySwitch.IsToggled = MatchContentOnly;

            LoadAndCleanHistory();
        }

        private static DateTimeOffset? GetCutoffDate(double value, string unit)
        {
            if (value <= 0) 
                return null;

            return unit switch
            {
                "Seconds" => DateTimeOffset.UtcNow.AddSeconds(-value),
                "Minutes" => DateTimeOffset.UtcNow.AddMinutes(-value),
                "Hours" => DateTimeOffset.UtcNow.AddHours(-value),
                "Days" => DateTimeOffset.UtcNow.AddDays(-value),
                _ => DateTimeOffset.UtcNow.AddHours(-value)
            };
        }

        private void LoadAndCleanHistory()
        {
            if (File.Exists(HistoryFilePath))
            {
                try
                {
                    string json = File.ReadAllText(HistoryFilePath);
                    var loadedItems = JsonSerializer.Deserialize<List<NotificationItemModel>>(json);

                    if (loadedItems != null)
                    {
                        var cutoffDate = GetCutoffDate(KeepHistoryValue, KeepHistoryUnit);
                        bool needsResave = false;

                        foreach (var item in loadedItems)
                        {
                            if (cutoffDate == null || item.UtcTimestamp >= cutoffDate.Value)
                            {
                                Notifications.Add(item);
                            }
                            else
                            {
                                needsResave = true;
                            }
                        }

                        if (needsResave)
                        {
                            SaveHistory();
                        }
                    }
                }
                catch
                {
                    Notifications.Clear();
                }
            }
        }

        private void SaveHistory()
        {
            try
            {
                string json = JsonSerializer.Serialize(Notifications);
                File.WriteAllText(HistoryFilePath, json);
            }
            catch { }
        }

        public void AddNotification(MyNotificationData data)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                string targetKey = data.GetSlotKey();
                var existing = Notifications.FirstOrDefault(x => string.Equals(x.SlotKey, targetKey, StringComparison.Ordinal));

                List<MyNotificationData>? existingHistory = existing?.History;
                if (existing != null)
                {
                    Notifications.Remove(existing);
                }

                var rowItem = NotificationItemModel.FromData(data, existingHistory);
                Notifications.Insert(0, rowItem);

                CleanOldNotificationsAndSave();
            });
        }

        public static void LogNotification(MyNotificationData data)
        {
            Instance?.AddNotification(data);
        }

        private void CleanOldNotificationsAndSave()
        {
            var cutoffDate = GetCutoffDate(KeepHistoryValue, KeepHistoryUnit);

            if (cutoffDate != null)
            {
                var itemsToRemove = Notifications.Where(n => n.UtcTimestamp < cutoffDate.Value).ToList();

                foreach (var item in itemsToRemove)
                {
                    Notifications.Remove(item);
                }
            }

            SaveHistory();
        }

        private static bool IsValidIpAddress(string input)
        {
            return IPAddress.TryParse(input, out _);
        }

        private static bool IsValidHostName(string input)
        {
            try
            {
                var addresses = Dns.GetHostAddresses(input);
                return addresses.Length > 0;
            }
            catch { }
            return false;
        }

        private async void OnEditBtnClicked(object sender, EventArgs e)
        {
            string newIp = this.ipEditEntry.Text;
            string newPort = this.portEntry.Text;
            string newValueStr = this.historyValueEntry.Text;
            string newUnit = this.historyUnitPicker.SelectedItem?.ToString() ?? "Hours";
            bool isSendEnabled = this.sendToServerSwitch.IsToggled;
            bool isSkipDuplicate = this.skipDuplicateSwitch.IsToggled;
            string newDupIntervalStr = this.duplicateIntervalEntry.Text;
            bool isMatchContentOnly = this.matchContentOnlySwitch.IsToggled;

            if (!IsValidIpAddress(newIp) && !IsValidHostName(newIp))
            {
                await DisplayAlert("Server Configuration", "Invalid IP or host.", "OK");
                return;
            }

            if (!ushort.TryParse(newPort, out ushort u16Port) || u16Port == 0)
            {
                await DisplayAlert("Server Configuration", "Invalid port.", "OK");
                return;
            }

            if (!double.TryParse(newValueStr, out double value) || value < 0)
            {
                await DisplayAlert("Server Configuration", "Invalid Keep History duration value.", "OK");
                return;
            }

            if (!double.TryParse(newDupIntervalStr, out double dupInterval) || dupInterval < 0.1 || dupInterval > 3600)
            {
                await DisplayAlert("Server Configuration", "Invalid Duplicate Interval (must be between 0.1 and 3600 seconds).", "OK");
                return;
            }

            ServerIp = newIp;
            ServerPort = u16Port;
            KeepHistoryValue = value;
            KeepHistoryUnit = newUnit;
            IsSendToServerEnabled = isSendEnabled;
            SkipDuplicateMessages = isSkipDuplicate;
            DuplicateIntervalSeconds = dupInterval;
            MatchContentOnly = isMatchContentOnly;

            Preferences.Set("server_ip", newIp);
            Preferences.Set("server_port", u16Port);
            Preferences.Set("keep_history_value", value);
            Preferences.Set("keep_history_unit", newUnit);
            Preferences.Set("send_to_server_enabled", isSendEnabled);
            Preferences.Set("skip_duplicate_messages", isSkipDuplicate);
            Preferences.Set("duplicate_interval_seconds", dupInterval);
            Preferences.Set("match_content_only", isMatchContentOnly);

            CleanOldNotificationsAndSave();

            await DisplayAlert("Server Configuration", "Applied server information.", "OK");
        }

        private async void OnClearHistoryClicked(object sender, EventArgs e)
        {
            if (Notifications.Count == 0)
            {
                await DisplayAlert("Clear History", "No notification record.", "OK");
            }
            else
            {
                bool isConfirmed = await DisplayAlert(
                    "Clear History",
                    "Are you sure you want to delete all notification records?",
                    "Clear",
                    "Cancel"
                );

                if (isConfirmed)
                {
                    Notifications.Clear();
                    SaveHistory();
                }
            }
        }

        private async void OnNotificationTapped(object? sender, TappedEventArgs e)
        {
            if (e.Parameter is NotificationItemModel item)
            {
                await Navigation.PushAsync(new NotificationDetailPage(item));
            }
            else if (sender is BindableObject bindable && bindable.BindingContext is NotificationItemModel contextItem)
            {
                await Navigation.PushAsync(new NotificationDetailPage(contextItem));
            }
        }

        private async void OnNotificationSelected(object? sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is NotificationItemModel selectedItem)
            {
                if (sender is CollectionView cv)
                {
                    cv.SelectedItem = null;
                }

                await Navigation.PushAsync(new NotificationDetailPage(selectedItem));
            }
        }
    }
}
