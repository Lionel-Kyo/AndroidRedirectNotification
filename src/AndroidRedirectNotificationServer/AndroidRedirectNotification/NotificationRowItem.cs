using System;

namespace AndroidRedirectNotification
{
    internal class NotificationRowItem
    {
        public string Time { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Status { get; set; } = "New";
        public string StatusColor { get; set; } = "#81C784";
        public string StatusBackground { get; set; } = "#1B3B22";
        public int NumberOfEvents { get; set; }
        public MyNotificationData Data { get; set; } = new();

        public static (string text, string color, string bg) GetStatusDetails(MyNotificationData data)
        {
            if (data.IsRemoved)
                return ("Removed", "#FFB4AB", "#3D1A1C");

            if (data.IsUpdate)
                return ("Update", "#80D8FF", "#132D40");

            return ("New", "#81C784", "#1B3B22");
        }
    }
}
