using System;
using System.Collections.Generic;

namespace AndroidRedirectNotification
{
    public enum NotificationDisposition
    {
        New,
        Updated,
        Duplicate
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
            string? sbnKey,
            string packageName,
            string title,
            string message,
            int id,
            string tag,
            double intervalSeconds,
            bool matchContentOnly,
            bool extendWindowOnHit = true)
        {
            long now = Environment.TickCount64;
            long windowMs = Math.Max(0, (long)(intervalSeconds * 1000.0));

            string safePkg = packageName ?? string.Empty;
            string safeTitle = title ?? string.Empty;
            string safeMessage = message ?? string.Empty;
            string safeTag = tag ?? string.Empty;
            string slotKey = GetSlotKey(sbnKey, safePkg, safeTag, id);

            var dupKey = matchContentOnly
                ? new ContentKey(safePkg, safeTitle, safeMessage, 0, string.Empty)
                : new ContentKey(safePkg, safeTitle, safeMessage, id, safeTag);

            lock (_lock)
            {
                long purgeInterval = Math.Max(5_000, windowMs * 2);
                if (_recentDuplicateTicks.Count > 30 && (now - _lastPurgeTick > purgeInterval))
                {
                    PurgeStale(now, purgeInterval);
                    _lastPurgeTick = now;
                }

                // 1. Suppress rapid identical re-posts within the debounce window
                if (windowMs > 0 &&
                    _recentDuplicateTicks.TryGetValue(dupKey, out long lastSeen) &&
                    now - lastSeen <= windowMs)
                {
                    if (extendWindowOnHit)
                    {
                        _recentDuplicateTicks[dupKey] = now;
                    }
                    return NotificationDisposition.Duplicate;
                }

                _recentDuplicateTicks[dupKey] = now;

                // 2. Check if this Android notification slot is already active
                if (_activeSlots.TryGetValue(slotKey, out var existingSlot))
                {
                    bool contentChanged =
                        !string.Equals(existingSlot.Title, safeTitle, StringComparison.Ordinal) ||
                        !string.Equals(existingSlot.Message, safeMessage, StringComparison.Ordinal);

                    _activeSlots[slotKey] = new ActiveSlotState(safeTitle, safeMessage, now);

                    // Same active slot: if text changed, mark as Updated; if identical outside window, still an update
                    return contentChanged
                        ? NotificationDisposition.Updated
                        : NotificationDisposition.Updated;
                }

                _activeSlots[slotKey] = new ActiveSlotState(safeTitle, safeMessage, now);
                return NotificationDisposition.New;
            }
        }

        private static string GetSlotKey(string? sbnKey, string packageName, string tag, int id)
        {
            if (!string.IsNullOrEmpty(sbnKey))
                return sbnKey;

            return $"{packageName ?? ""}\u001f{tag ?? ""}\u001f{id}";
        }

        public void MarkRemoved(string? sbnKey, string packageName, string tag, int id)
        {
            string slotKey = GetSlotKey(sbnKey, packageName, tag, id);

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
