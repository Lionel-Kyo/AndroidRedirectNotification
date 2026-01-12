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

        public bool Equals(MyNotificationData? obj)
        {
            if (obj == null) 
                return false;

            return this.Id == obj.Id && this.Tag == obj.Tag &&
                this.PackageName == obj.PackageName && this.AppName == obj.AppName &&
                this.Title == obj.Title && this.Message == obj.Message &&
                this.Category == obj.Category && this.Importantce == obj.Importantce &&
                this.ActionTitles.SequenceEqual(obj.ActionTitles) && this.Flags.SequenceEqual(obj.Flags);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) 
                return false; 

            if (obj is MyNotificationData)
                return Equals((MyNotificationData)obj);

            return false;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Id);
            hash.Add(Tag);
            hash.Add(PackageName);
            hash.Add(AppName);
            hash.Add(Title);
            hash.Add(Message);
            hash.Add(Category);
            hash.Add(Importantce);
            hash.Add(ActionTitles);
            hash.Add(Flags);
            return hash.ToHashCode();
        }
    }
}
