using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
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
            this.SkipDuplicateMsgMs = 2000;
            this.ShowWindowsNotification = true;
        }

        public static Settings? ReadSettings()
        {
            if (!File.Exists(SettingsPath))
                return null;

            string content = File.ReadAllText(SettingsPath, Encoding.UTF8);
            var settings = JsonSerializer.Deserialize(content, AppJsonSerializerContext.Default.Settings);

            if (settings != null)
            {
                if (settings.SkipDuplicateMsgMs > 99999)
                    settings.SkipDuplicateMsgMs = 99999;
                else if (settings.SkipDuplicateMsgMs < 100)
                    settings.SkipDuplicateMsgMs = 2000;
            }
            return settings;
        }

        public static void SaveSettings(Settings settings)
        {
            string? directory = Path.GetDirectoryName(SettingsPath);
            if (directory == null)
                return;

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);


            var options = new JsonSerializerOptions(AppJsonSerializerContext.Default.Options)
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            string content = JsonSerializer.Serialize(settings, options);
            File.WriteAllText(SettingsPath, content, Encoding.UTF8);
        }
    }
}
