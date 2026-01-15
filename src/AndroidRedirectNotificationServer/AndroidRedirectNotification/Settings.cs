using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AndroidRedirectNotification
{
    internal class Settings
    {
        public static readonly string SettingsPath = "./Settings/Settings.json";
        public ushort Port { get; set; }
        public bool SkipDuplicateMsg { get; set; }
        public int SkipDuplicateMsgMs { get; set; }
        public bool ShowWindowsNotification { get; set; }

        public Settings()
        {
            this.Port = 443;
            this.SkipDuplicateMsg = true;
            this.SkipDuplicateMsgMs = 1000;
            this.ShowWindowsNotification = true;
        }

        public static Settings? ReadSettings()
        {
            if (!File.Exists(SettingsPath))
                return null;

            string content = File.ReadAllText(SettingsPath, Encoding.UTF8);
            return JsonSerializer.Deserialize<Settings>(content);
        }

        public static void SaveSettings(Settings settings)
        {
            string? directory = Path.GetDirectoryName(SettingsPath);
            if (directory == null)
                return;

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            string content = JsonSerializer.Serialize(settings, jsonSerializerOptions);
            File.WriteAllText(SettingsPath, content, Encoding.UTF8);
        }
    }
}
