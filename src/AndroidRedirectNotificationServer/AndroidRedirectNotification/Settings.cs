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
        public bool ShowWindowsNotification { get; set; }

        public Settings()
        {
            this.Port = 443;
            this.ShowWindowsNotification = true;
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
