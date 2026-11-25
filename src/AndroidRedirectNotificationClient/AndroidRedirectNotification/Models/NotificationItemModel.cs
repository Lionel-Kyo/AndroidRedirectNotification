using System;
using System.Collections.Generic;
using System.Linq;

namespace AndroidRedirectNotification.Models
{
    public class NotificationItemModel
    {
        public string SlotKey { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string AppName { get; set; } = string.Empty;
        public string PackageName { get; set; } = string.Empty;
        public string Timestamp { get; set; } = string.Empty;
        public DateTimeOffset UtcTimestamp { get; set; }
        public string Status { get; set; } = "New";
        public string StatusColor { get; set; } = "#81C784";
        public string StatusBackground { get; set; } = "#1B3B22";
        public bool IsUpdate { get; set; }
        public bool IsRemoved { get; set; }
        public bool HasImages { get; set; }

        public MyNotificationData LatestData { get; set; } = new();
        public List<MyNotificationData> History { get; set; } = new();

        public int HistoryCount => History?.Count ?? 0;
        public bool HasHistory => (History?.Count ?? 0) > 1;
        public string RevisionBadgeText => $"{History?.Count ?? 0} events";

        public static string GetEventStatus(MyNotificationData item)
        {
            if (item.IsRemoved) return "Removed";
            return item.IsUpdate ? "Updated" : "New";
        }

        public static NotificationItemModel FromData(MyNotificationData data, List<MyNotificationData>? existingHistory = null)
        {
            var history = existingHistory != null ? new List<MyNotificationData>(existingHistory) : new List<MyNotificationData>();
            history.Add(data);

            string status = GetEventStatus(data);
            string statusColor = data.IsRemoved ? "#FFB4AB" : (data.IsUpdate ? "#80D8FF" : "#81C784");
            string statusBackground = data.IsRemoved ? "#3D1A1C" : (data.IsUpdate ? "#132D40" : "#1B3B22");

            bool hasImages = !string.IsNullOrWhiteSpace(data.Picture) || !string.IsNullOrWhiteSpace(data.PictureIcon);
            string appDisplay = !string.IsNullOrWhiteSpace(data.AppName) ? data.AppName : data.PackageName;

            return new NotificationItemModel
            {
                SlotKey = data.GetSlotKey(),
                Title = data.Title ?? string.Empty,
                Message = data.Message ?? string.Empty,
                AppName = appDisplay ?? string.Empty,
                PackageName = data.PackageName ?? string.Empty,
                Timestamp = data.GetDateTime().ToString("HH:mm:ss (yyyy-MM-dd)"),
                UtcTimestamp = DateTimeOffset.FromUnixTimeSeconds(data.TimeStamp > 0 ? data.TimeStamp : DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
                Status = status,
                StatusColor = statusColor,
                StatusBackground = statusBackground,
                IsUpdate = data.IsUpdate,
                IsRemoved = data.IsRemoved,
                HasImages = hasImages,
                LatestData = data,
                History = history
            };
        }
    }
}
