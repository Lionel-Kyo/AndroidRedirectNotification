using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AndroidRedirectNotification
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(Settings))]
    [JsonSerializable(typeof(MyNotificationData))]
    [JsonSerializable(typeof(Dictionary<string, int>))]
    internal partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }
}
