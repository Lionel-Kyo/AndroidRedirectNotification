using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text.Json;

namespace AndroidRedirectNotification
{
    public partial class MainPage : ContentPage
    {
        public static MainPage? Instance { get; private set; }
        public static string ServerIp { get; private set; } = "192.168.1.1";
        public static ushort ServerPort { get; private set; } = 443;
        public static double KeepHistoryValue { get; private set; } = 7;
        public static string KeepHistoryUnit { get; private set; } = "Days";

        public Entry IpEditEntry => ipEditEntry;
        public Entry PortEditEntry => portEntry;

        public ObservableCollection<NotificationLog> Notifications { get; set; } = new();

        private string HistoryFilePath => Path.Combine(FileSystem.AppDataDirectory, "NotificationHistory.json");

        public MainPage()
        {
            InitializeComponent();
            Instance = this;

            historyCollectionView.ItemsSource = Notifications;

            ServerIp = Preferences.Get("server_ip", "192.168.1.1");
            ServerPort = (ushort)Preferences.Get("server_port", 443);
            KeepHistoryValue = Preferences.Get("keep_history_value", 7.0);
            KeepHistoryUnit = Preferences.Get("keep_history_unit", "Days");

            this.ipEditEntry.Text = ServerIp;
            this.portEntry.Text = ServerPort.ToString();
            this.historyValueEntry.Text = KeepHistoryValue.ToString();
            this.historyUnitPicker.SelectedItem = KeepHistoryUnit;

            LoadAndCleanHistory();
        }

        private static DateTimeOffset? GetCutoffDate(double value, string unit)
        {
            // 0: keep forever
            if (value <= 0) 
                return null;

            return unit switch
            {
                "Seconds" => DateTimeOffset.UtcNow.AddSeconds(-value),
                "Minutes" => DateTimeOffset.UtcNow.AddMinutes(-value),
                "Hours" => DateTimeOffset.UtcNow.AddHours(-value),
                "Days" => DateTimeOffset.UtcNow.AddDays(-value),
                _ => DateTimeOffset.UtcNow.AddDays(-value)
            };
        }

        private void LoadAndCleanHistory()
        {
            if (File.Exists(HistoryFilePath))
            {
                try
                {
                    string json = File.ReadAllText(HistoryFilePath);
                    var loadedLogs = JsonSerializer.Deserialize<List<NotificationLog>>(json);

                    if (loadedLogs != null)
                    {
                        var cutoffDate = GetCutoffDate(KeepHistoryValue, KeepHistoryUnit);
                        bool needsResave = false;

                        foreach (var log in loadedLogs)
                        {
                            if (cutoffDate == null || log.UtcTimestamp >= cutoffDate.Value)
                            {
                                Notifications.Add(log);
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

        public void AddNotification(string message, string title, DateTimeOffset utcDateTime)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Notifications.Insert(0, new NotificationLog
                {
                    Title = title,
                    Message = message,
                    Timestamp = utcDateTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                    UtcTimestamp = utcDateTime
                });

                CleanOldNotificationsAndSave();
            });
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

        public static void LogNotification(string message, string title, DateTimeOffset utcDateTime)
        {
            Instance?.AddNotification(message, title, utcDateTime);
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
            string newUnit = this.historyUnitPicker.SelectedItem?.ToString() ?? "Days";

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

            ServerIp = newIp;
            ServerPort = u16Port;
            KeepHistoryValue = value;
            KeepHistoryUnit = newUnit;

            Preferences.Set("server_ip", newIp);
            Preferences.Set("server_port", u16Port);
            Preferences.Set("keep_history_value", value);
            Preferences.Set("keep_history_unit", newUnit);

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
    }

    public class NotificationLog
    {
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public string Timestamp { get; set; } = "";
        public DateTimeOffset UtcTimestamp { get; set; }
    }
}