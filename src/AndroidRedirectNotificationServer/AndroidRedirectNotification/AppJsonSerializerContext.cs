using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AndroidRedirectNotification
{
    [JsonSerializable(typeof(Settings))]
    [JsonSerializable(typeof(MyNotificationData))]
    internal partial class AppJsonSerializerContext : JsonSerializerContext
    {
    }
}
