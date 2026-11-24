using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidRedirectNotification
{
    internal class NotificationRowItem
    {
        public string TimeId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Title { get; set; } = "";
        public string Message { get; set; } = "";
        public MyNotificationData Data { get; set; } = new();
    }
}
