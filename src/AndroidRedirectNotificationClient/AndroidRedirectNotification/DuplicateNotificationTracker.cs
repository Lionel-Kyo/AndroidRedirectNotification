using AndroidRedirectNotification.Models;
using System;
using System.Collections.Generic;

namespace AndroidRedirectNotification
{
    public enum NotificationDisposition
    {
        New,
        Updated,
        NewDuplicate,
        UpdatedDuplicate,
    }

    public sealed class DuplicateNotificationTracker
    {
        private readonly record struct ContentKey(
            string PackageName,
            string Title,
            string Message,
            int Id,
            string Tag);

        private readonly record struct ActiveSlotState(
            string Title,
            string Message,
            long LastUpdatedTick);

        private readonly object _lock = new();
        private readonly Dictionary<ContentKey, long> _recentDuplicateTicks = new();
        private readonly Dictionary<string, ActiveSlotState> _activeSlots = new(StringComparer.Ordinal);
        private readonly List<ContentKey> _purgeBuffer = new();
        private long _lastPurgeTick = Environment.TickCount64;

        public NotificationDisposition Evaluate(
            MyNotificationData data,
            double intervalSeconds,
            bool matchContentOnly,
            bool extendWindowOnHit = true)
        {
            long now = Environment.TickCount64;
            long windowMs = Math.Max(0, (long)(intervalSeconds * 1000.0));

            string slotKey = data.GetSlotKey();
            bool isUpdate = _activeSlots.TryGetValue(slotKey, out _);

            var dupKey = matchContentOnly
                ? new ContentKey(data.PackageName, data.Title, data.Message, 0, string.Empty)
                : new ContentKey(data.PackageName, data.Title, data.Message, data.Id, data.Tag);

            lock (_lock)
            {
                long purgeInterval = Math.Max(5_000, windowMs * 2);
                if (_recentDuplicateTicks.Count > 30 && (now - _lastPurgeTick > purgeInterval))
                {
                    PurgeStale(now, purgeInterval);
                    _lastPurgeTick = now;
                }

                // Suppress rapid identical re-posts within the debounce window
                if (windowMs > 0 &&
                    _recentDuplicateTicks.TryGetValue(dupKey, out long lastSeen) &&
                    now - lastSeen <= windowMs)
                {
                    if (extendWindowOnHit)
                    {
                        _recentDuplicateTicks[dupKey] = now;
                    }
                    return isUpdate ? NotificationDisposition.NewDuplicate : NotificationDisposition.UpdatedDuplicate;
                }

                _recentDuplicateTicks[dupKey] = now;

                _activeSlots[slotKey] = new ActiveSlotState(data.Title, data.Message, now);
                return isUpdate ? NotificationDisposition.Updated : NotificationDisposition.New;
            }
        }

        public void MarkRemoved(MyNotificationData data)
        {
            string slotKey = data.GetSlotKey();

            lock (_lock)
            {
                _activeSlots.Remove(slotKey);
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _recentDuplicateTicks.Clear();
                _activeSlots.Clear();
            }
        }

        private void PurgeStale(long now, long expirationMs)
        {
            _purgeBuffer.Clear();

            foreach (var kvp in _recentDuplicateTicks)
            {
                if (now - kvp.Value > expirationMs)
                {
                    _purgeBuffer.Add(kvp.Key);
                }
            }

            for (int i = 0; i < _purgeBuffer.Count; i++)
            {
                _recentDuplicateTicks.Remove(_purgeBuffer[i]);
            }

            _purgeBuffer.Clear();
        }
    }
}
