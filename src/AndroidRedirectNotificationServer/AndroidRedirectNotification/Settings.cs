using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace AndroidRedirectNotification
{
    internal class Settings
    {
        public static readonly string SettingsPath = "./Settings/Settings.json";
        public ushort Port { get; set; }
        public bool ShowWindowsToast { get; set; }
        public bool ShowToastOnUpdate { get; set; }
        public List<string> BlockedCategories { get; set; }
        public List<string> BlockedFlags { get; set; }

        public Settings()
        {
            this.Port = 5000;
            this.ShowWindowsToast = true;
            this.ShowToastOnUpdate = false;
            this.BlockedCategories = ["transport"];
            this.BlockedFlags = ["OngoingEvent"];
        }

        public bool ShouldShowWindowsToast(MyNotificationData notification)
        {
            if (!ShowWindowsToast || notification.IsRemoved)
                return false;

            if (!string.IsNullOrEmpty(notification.Category) &&
                BlockedCategories.Contains(notification.Category, StringComparer.OrdinalIgnoreCase))
                return false;

            if (notification.Flags.Any(flag =>
                BlockedFlags.Contains(flag, StringComparer.OrdinalIgnoreCase)))
                return false;

            if (notification.IsUpdate && !ShowToastOnUpdate)
                return false;

            return true;
        }

        public static Settings? ReadSettings()
        {
            if (!File.Exists(SettingsPath))
                return null;

            string content = File.ReadAllText(SettingsPath, Encoding.UTF8);
            return JsonSerializer.Deserialize(content, AppJsonSerializerContext.Default.Settings);
        }

        public static void SaveSettings(Settings settings)
        {
            string? directory = Path.GetDirectoryName(SettingsPath);
            if (directory == null)
                return;

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            string content = JsonSerializer.Serialize(settings, AppJsonSerializerContext.Default.Settings);
            File.WriteAllText(SettingsPath, content, Encoding.UTF8);
        }
    }
}
