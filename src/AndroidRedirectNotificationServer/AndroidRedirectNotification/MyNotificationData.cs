using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidRedirectNotification
{
    internal class MyNotificationData
    {
        public int Id { get; set; }
        public string Tag { get; set; }
        public string PackageName { get; set; }
        public string AppName { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Category { get; set; }
        public int Importantce { get; set; }
        public List<string> ActionTitles { get; set; }
        /// <summary>
        /// ShowLights, OngoingEvent, Insistent, OnlyAlertOnce, AutoCancel, NoClear, ForegroundService, HighPriority, LocalOnly, GroupSummary, Bubble
        /// </summary>
        public List<string> Flags { get; set; }

        public MyNotificationData()
        {
            Id = 0;
            Tag = "";
            PackageName = "";
            AppName = "";
            Title = "";
            Message = "";
            Category = "";
            ActionTitles = new List<string>();
            Flags = new List<string>();
            Importantce = 0;
        }
    }
}
