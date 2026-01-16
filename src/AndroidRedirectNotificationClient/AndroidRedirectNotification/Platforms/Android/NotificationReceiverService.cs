using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Nfc;
using Android.OS;
using Android.Service.Notification;
using Android.Util;
using AndroidRedirectNotification;
using AndroidRedirectNotification.Platforms.Android;
using Java.Security.Interfaces;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Android.Graphics.Bitmap;
using static Microsoft.Maui.ApplicationModel.Platform;

// [Android.Runtime.Register("android/service/notification/NotificationListenerService", DoNotGenerateAcw = true)]
[Service(Name = "com.example.RedirectNotification.NotificationReceiverService", Permission = "android.permission.BIND_NOTIFICATION_LISTENER_SERVICE", Label = "Notification Receiver Service", Exported = true)]
[IntentFilter(new[] { "android.service.notification.NotificationListenerService" })]
public class NotificationReceiverService : NotificationListenerService
{
    public override void OnListenerConnected()
    {
        //Log.Info("NotificationReceiverService", "Listener Connected!");
        base.OnListenerConnected();
    }

    public override void OnNotificationPosted(StatusBarNotification? sbn)
    {
        //Log.Info("NotificationService", "Notification received!");
        base.OnNotificationPosted(sbn);
        if (sbn == null) 
            return;

        var notification = sbn.Notification;
        if (notification == null) 
            return;

        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        int id = sbn.Id;
        string tag = sbn.Tag ?? "";
        string packageName = sbn.PackageName ?? "";
        string? title = notification.Extras?.GetCharSequence(Notification.ExtraTitle);
        string? message = notification.Extras?.GetCharSequence(Notification.ExtraText);
        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(message))
            return;

        string pictureBase64 = "";
        string pictureIconBase64 = "";
#pragma warning disable CA1416
        if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
        {
            var picture = notification.Extras?.GetParcelable(Notification.ExtraPicture, Java.Lang.Class.FromType(typeof(Bitmap))) as Bitmap;
            if (picture != null)
                pictureBase64 = BitmapToBase64(picture, CompressFormat.Jpeg!, 80);

            var pictureIcon = notification.Extras?.GetParcelable(Notification.ExtraPictureIcon, Java.Lang.Class.FromType(typeof(Icon))) as Icon;
            if (pictureIcon != null)
            {
                var drawable = pictureIcon.LoadDrawable(this);
                if (drawable != null && drawable is BitmapDrawable bd && bd.Bitmap != null)
                {
                    pictureIconBase64 = BitmapToBase64(bd.Bitmap, CompressFormat.Jpeg!, 80);
                }
            }
        }
#pragma warning restore CA1416

        var actionTitles = notification.Actions?.Select(a => a.Title?.ToString() ?? "")?.ToList() ?? new List<string>();
        // bool isOngoing = notification.Flags.HasFlag(NotificationFlags.OngoingEvent);
        int notificationImportance = 3; // Default
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
#pragma warning disable CA1416
            var manager = (NotificationManager?)Android.App.Application.Context.GetSystemService(Context.NotificationService);
            var channel = manager?.GetNotificationChannel(notification.ChannelId);
            if (channel != null)
            {
                if (channel.Importance != NotificationImportance.Unspecified)
                    notificationImportance = (int)channel.Importance;
            }
#pragma warning restore CA1416
        }

        var jsonSerializerOptions = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        jsonSerializerOptions.Converters.Add(new JsonNumberEnumConverter<NotificationFlags>());
        string jsonText = JsonSerializer.Serialize(new MyNotificationData()
        {
            TimeStamp = timestamp,
            Id = id,
            Tag = tag,
            PackageName = packageName,
            AppName = GetApplicationName(packageName) ?? "",
            Title = title,
            Message = message,
            Picture = pictureBase64,
            PictureIcon = pictureIconBase64,
            Category = notification.Category ?? "",
            Importantce = notificationImportance,
            ActionTitles = actionTitles,
            Flags = Enum.GetValues(typeof(NotificationFlags)).Cast<NotificationFlags>().Where(f => notification.Flags.HasFlag(f)).Select(f => f.ToString()).ToList()
        }, jsonSerializerOptions);

        //Log.Info("NotificationService", $"Notification: {jsonText}");
        this.SendToServer(jsonText);
        this.UpdateUILabel(jsonText);
    }

    public override void OnNotificationRemoved(StatusBarNotification? sbn)
    {
        //Log.Info("NotificationService", "Notification removed.");
        base.OnNotificationRemoved(sbn);
    }

    private string? GetApplicationName(string packageName)
    {
        if (string.IsNullOrEmpty(packageName))
            return null;

        try
        {
            var packageManager = Android.App.Application.Context.PackageManager;
            var appInfo = packageManager?.GetApplicationInfo(packageName, 0);
            if (appInfo == null) 
                return null;
            return packageManager?.GetApplicationLabel(appInfo)?.ToString();
        }
        catch { return null; }
    }
    private void SendToServer(string message)
    {
        const int maxRetries = 1;
        const int retryDelayMs = 100;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    var connectionTask = client.ConnectAsync(MainPage.ServerIp, MainPage.ServerPort);
                    if (connectionTask.Wait(TimeSpan.FromMilliseconds(500))) 
                    {
                        using (NetworkStream networkStream = client.GetStream())
                        {
                            MyNetworkStream stream = new MyNetworkStream(networkStream);
                            byte[] buffer;
                            buffer = stream.Read();
                            byte[] rsaPublicKey = buffer;
                            byte[] aesKey = AES.MessageByteCryption.GenerateKey();
                            stream.Write(RSA.MessageByteCryption.EncryptRsa(aesKey, rsaPublicKey));
                            buffer = stream.Read();
                            var status = JsonSerializer.Deserialize<Dictionary<string, int>>(
                                Encoding.UTF8.GetString(
                                    AES.MessageByteCryption.Decrypt(buffer, aesKey)
                                )
                            );
                            if (status != null && (status.TryGetValue("Status", out int i32Status) && i32Status != 0))
                            {
                                byte[] encryptedMessage = AES.MessageByteCryption.Encrypt(Encoding.UTF8.GetBytes(message), aesKey);
                                stream.Write(encryptedMessage);
                            }
                        }
                        return; 
                    }
                    else
                    {
                        Log.Error("NotificationReceiverService", $"Connection timed out (Attempt {attempt}/{maxRetries})");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("NotificationReceiverService", $"Error sending notification (Attempt {attempt}/{maxRetries}): " + ex.Message);
            }

            if (attempt < maxRetries)
            {
                Log.Info("NotificationReceiverService", $"Retrying in {retryDelayMs / 1000d} seconds...");
                Thread.Sleep(retryDelayMs);
            }
        }

        Log.Error("NotificationReceiverService", "Failed to send notification after multiple attempts.");
    }

    private void UpdateUILabel(string message)
    {
        try
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                try
                {
                    if (MainPage.Instance != null)
                    {
                        MainPage.Instance.WelcomeLabel.Text = message;
                    }
                }
                catch { }
            });
        }
        catch { }
    }

    public static string BitmapToBase64(Bitmap bitmap, CompressFormat compressFormat, int quality)
    {
        using (var stream = new MemoryStream())
        {

            bitmap.Compress(compressFormat, 100, stream);

            byte[] bytes = stream.ToArray();
            return Convert.ToBase64String(bytes);
        }
    }
}