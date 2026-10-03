using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidRedirectNotification
{
    internal class NotificationRowItem
    {
        public string DateTimeId { get; set; } = string.Empty;
        public string Tag { get; set; } = string.Empty;
        public string PackageName { get; set; } = string.Empty;
        public string AppName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Importance { get; set; } = string.Empty;
        public string ActionTitles { get; set; } = string.Empty;
        public string Flags { get; set; } = string.Empty;
        public MyNotificationData Data { get; set; } = null!;
    }
}
