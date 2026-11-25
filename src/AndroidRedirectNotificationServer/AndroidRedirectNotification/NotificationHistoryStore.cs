using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidRedirectNotification
{
    internal static class NotificationHistoryStore
    {
        private static readonly object _lock = new();
        private static readonly Dictionary<string, List<MyNotificationData>> _historyBySlot = new(StringComparer.Ordinal);

        public static int Record(MyNotificationData data)
        {
            string key = data.GetSlotKey();
            lock (_lock)
            {
                if (!_historyBySlot.TryGetValue(key, out var list))
                {
                    list = new List<MyNotificationData>();
                    _historyBySlot[key] = list;
                }
                list.Add(data);
                return list.Count;
            }
        }

        public static IReadOnlyList<MyNotificationData> GetHistory(MyNotificationData data)
        {
            string key = data.GetSlotKey();
            lock (_lock)
            {
                if (_historyBySlot.TryGetValue(key, out var list))
                    return list.ToList();

                return new List<MyNotificationData> { data };
            }
        }

        public static void Clear()
        {
            lock (_lock)
            {
                _historyBySlot.Clear();
            }
        }
    }
}
