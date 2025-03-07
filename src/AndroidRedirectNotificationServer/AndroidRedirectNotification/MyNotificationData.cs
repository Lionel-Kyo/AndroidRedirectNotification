using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidRedirectNotification
{
    internal class MyNotificationData
    {
        public string PackageName { get; set; }
        public string AppName { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Category { get; set; }
        public List<string> ActionTitles { get; set; }
        public int Importantce { get; set; }
        public NotificationFlags Flags { get; set; }

        public MyNotificationData()
        {
            PackageName = "";
            AppName = "";
            Title = "";
            Message = "";
            Category = "";
            ActionTitles = new List<string>();
            Importantce = 0;
            Flags = default(NotificationFlags);
        }
    }
}
