using Android.App;
using Android.Content;
using Android.Service.Notification;
using Android.Util;
using AndroidRedirectNotification;
using Java.Security.Interfaces;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using static Microsoft.Maui.ApplicationModel.Platform;

// [Android.Runtime.Register("android/service/notification/NotificationListenerService", DoNotGenerateAcw = true)]
[Service(Name = "com.kyokyoapp.NotificationReceiverService", Permission = "android.permission.BIND_NOTIFICATION_LISTENER_SERVICE", Label = "Notification Receiver Service", Exported = true)]
[IntentFilter(new[] { "android.service.notification.NotificationListenerService" })]
public class NotificationReceiverService : NotificationListenerService
{
    public override void OnListenerConnected()
    {
        //Log.Info("NotificationService", "Listener Connected!");
        base.OnListenerConnected();
    }

    public override void OnNotificationPosted(StatusBarNotification? sbn)
    {
        //Log.Info("NotificationService", "Notification received!");
        base.OnNotificationPosted(sbn);
        string packageName = sbn?.PackageName ?? "";
        string? title = sbn?.Notification?.Extras?.GetCharSequence("android.title")?.ToString();
        string? message = sbn?.Notification?.Extras?.GetCharSequence("android.text")?.ToString();

        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(message))
            return;

        string jsonText = JsonSerializer.Serialize(new Dictionary<string, string>()
        {
            { "PackageName", packageName },
            { "AppName", GetApplicationName(packageName) ?? "" },
            { "Title", title },
            { "Message", message },
        });

        //Log.Info("NotificationService", $"Notification: {jsonText}");
        this.SendToServer(jsonText);
        this.UpdateUILabel(jsonText);
    }

    public override void OnNotificationRemoved(StatusBarNotification? sbn)
    {
        //Log.Info("NotificationService", "Notification removed.");
        base.OnNotificationRemoved(sbn);
    }

    public string? GetApplicationName(string packageName)
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
                        using (NetworkStream stream = client.GetStream())
                        {
                            byte[] buffer = new byte[65536];
                            int bytesRead;
                            bytesRead = stream.Read(buffer, 0, buffer.Length);
                            byte[] rsaPublicKey = buffer.Take(bytesRead).ToArray();
                            byte[] aesKey = AES.MessageByteCryption.GenerateKey();
                            stream.Write(RSA.MessageByteCryption.EncryptRsa(aesKey, rsaPublicKey));
                            bytesRead = stream.Read(buffer, 0, buffer.Length);
                            var status = JsonSerializer.Deserialize<Dictionary<string, int>>(
                                Encoding.UTF8.GetString(
                                    AES.MessageByteCryption.Decrypt(
                                        buffer.Take(bytesRead).ToArray(), aesKey
                                    )
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
                        Log.Error("NotificationService", $"Connection timed out (Attempt {attempt}/{maxRetries})");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("NotificationService", $"Error sending notification (Attempt {attempt}/{maxRetries}): " + ex.Message);
            }

            if (attempt < maxRetries)
            {
                Log.Info("NotificationService", $"Retrying in {retryDelayMs / 1000d} seconds...");
                Thread.Sleep(retryDelayMs);
            }
        }

        Log.Error("NotificationService", "Failed to send notification after multiple attempts.");
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
}