using System;
using System.Collections.Generic;

namespace AndroidRedirectNotification
{
    public sealed class DuplicateNotificationTracker
    {
        private readonly struct NotificationKey : IEquatable<NotificationKey>
        {
            public readonly string PackageName;
            public readonly string Title;
            public readonly string Message;
            public readonly int Id;
            public readonly string Tag;
            public readonly bool MatchContentOnly;

            public NotificationKey(string packageName, string title, string message, int id, string tag, bool matchContentOnly)
            {
                PackageName = packageName ?? string.Empty;
                Title = title ?? string.Empty;
                Message = message ?? string.Empty;
                Id = id;
                Tag = tag ?? string.Empty;
                MatchContentOnly = matchContentOnly;
            }

            public bool Equals(NotificationKey other)
            {
                if (MatchContentOnly || other.MatchContentOnly)
                {
                    return string.Equals(PackageName, other.PackageName, StringComparison.Ordinal)
                        && string.Equals(Title, other.Title, StringComparison.Ordinal)
                        && string.Equals(Message, other.Message, StringComparison.Ordinal);
                }

                return Id == other.Id
                    && string.Equals(Tag, other.Tag, StringComparison.Ordinal)
                    && string.Equals(PackageName, other.PackageName, StringComparison.Ordinal)
                    && string.Equals(Title, other.Title, StringComparison.Ordinal)
                    && string.Equals(Message, other.Message, StringComparison.Ordinal);
            }

            public override bool Equals(object? obj) => obj is NotificationKey other && Equals(other);

            public override int GetHashCode()
            {
                var hash = new HashCode();
                hash.Add(PackageName, StringComparer.Ordinal);
                hash.Add(Title, StringComparer.Ordinal);
                hash.Add(Message, StringComparer.Ordinal);

                if (!MatchContentOnly)
                {
                    hash.Add(Id);
                    hash.Add(Tag, StringComparer.Ordinal);
                }

                return hash.ToHashCode();
            }
        }

        private readonly object _lock = new();
        private readonly Dictionary<NotificationKey, long> _lastSeenTicks = new();
        private readonly List<NotificationKey> _purgeBuffer = new();
        private long _lastPurgeTick = Environment.TickCount64;

        /// <summary>
        /// Checks whether the notification is a duplicate within the given window.
        /// </summary>
        public bool IsDuplicate(string packageName, string title, string message, int id, string tag, double intervalSeconds, bool matchContentOnly)
        {
            long now = Environment.TickCount64;
            long windowMs = (long)Math.Max(100, intervalSeconds * 1000.0);
            var key = new NotificationKey(packageName, title, message, id, tag, matchContentOnly);

            lock (_lock)
            {
                // Periodically purge stale entries to prevent memory growth
                if (_lastSeenTicks.Count > 30 && (now - _lastPurgeTick > windowMs * 2))
                {
                    PurgeStale(now, windowMs * 2);
                    _lastPurgeTick = now;
                }

                if (_lastSeenTicks.TryGetValue(key, out long lastSeen))
                {
                    if (now - lastSeen <= windowMs)
                    {
                        // Reset timestamp on rapid bursts (debounce suppression)
                        _lastSeenTicks[key] = now;
                        return true;
                    }
                }

                _lastSeenTicks[key] = now;
                return false;
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _lastSeenTicks.Clear();
            }
        }

        private void PurgeStale(long now, long expirationMs)
        {
            _purgeBuffer.Clear();

            foreach (var kvp in _lastSeenTicks)
            {
                if (now - kvp.Value > expirationMs)
                {
                    _purgeBuffer.Add(kvp.Key);
                }
            }

            for (int i = 0; i < _purgeBuffer.Count; i++)
            {
                _lastSeenTicks.Remove(_purgeBuffer[i]);
            }

            _purgeBuffer.Clear();
        }
    }
}
