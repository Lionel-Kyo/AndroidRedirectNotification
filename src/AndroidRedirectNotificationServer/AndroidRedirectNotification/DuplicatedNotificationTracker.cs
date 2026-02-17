using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidRedirectNotification
{
    internal class DuplicatedNotificationTracker
    {
        private object locker = new object();
        private readonly Dictionary<MyNotificationData, long> lastSeenTicks;

        public System.Timers.Timer cleanTimer;

        public TimeSpan Window { get; set; }

        public DuplicatedNotificationTracker(TimeSpan window, double cleanInterval)
        {
            this.Window = window;
            this.lastSeenTicks = new Dictionary<MyNotificationData, long>(new MyNotificationDataDuplicatedComparer());
            this.cleanTimer = new System.Timers.Timer(cleanInterval);
            this.cleanTimer.Elapsed += CleanTimer_Elapsed;
        }

        private void CleanTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            this.Cleanup();
        }

        public bool IsDuplicate(MyNotificationData data)
        {
            long now = DateTime.UtcNow.Ticks;
            long windowTicks = Window.Ticks;

            lock (locker)
            {
                if (lastSeenTicks.TryGetValue(data, out long lastSeen))
                {
                    if (now - lastSeen <= windowTicks)
                    {
                        return true;
                    }
                    else
                    {
                        lastSeenTicks.Remove(data);
                        lastSeenTicks[data] = now;
                    }
                }
                else
                {
                    lastSeenTicks[data] = now;
                }
                return false;
            }
        }

        private void Cleanup()
        {
            long now = DateTime.UtcNow.Ticks;
            long windowTicks = Window.Ticks;

            lock (locker)
            {
                foreach (var lastSeenTick in lastSeenTicks)
                {
                    if (now - lastSeenTick.Value > windowTicks)
                        lastSeenTicks.Remove(lastSeenTick.Key, out _);
                }
            }
        }
    }
}
