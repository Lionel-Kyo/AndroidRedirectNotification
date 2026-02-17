using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidRedirectNotification
{
    internal sealed class MyNotificationDataDuplicatedComparer : IEqualityComparer<MyNotificationData>
    {
        public bool Equals(MyNotificationData? x, MyNotificationData? y)
        {
            if (ReferenceEquals(x, y)) 
                return true;

            if (x is null || y is null) 
                return false;

            return x.Id == y.Id && x.Tag == y.Tag &&
                x.PackageName == y.PackageName && x.AppName == y.AppName &&
                x.Title == y.Title && x.Message == y.Message &&
                x.Category == y.Category && x.Importantce == y.Importantce &&
                x.ActionTitles.SequenceEqual(y.ActionTitles) && x.Flags.SequenceEqual(y.Flags);
        }

        public int GetHashCode(MyNotificationData data)
        {
            HashCode hash = new HashCode();
            hash.Add(data.Id);
            hash.Add(data.Tag);
            hash.Add(data.PackageName);
            hash.Add(data.AppName);
            hash.Add(data.Title);
            hash.Add(data.Message);
            hash.Add(data.Category);
            hash.Add(data.Importantce);
            hash.Add(data.ActionTitles);
            hash.Add(data.Flags);
            return hash.ToHashCode();
        }
    }
}
