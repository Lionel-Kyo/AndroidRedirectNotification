using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using AndroidX.Core.App;

namespace AndroidRedirectNotification
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //Log.Info("NotificationService", "base.OnCreate(savedInstanceState)");
            //if (!IsNotificationListenerEnabled())
            //{
            //    OpenNotificationAccessSettings();
            //}
            try
            {
                Android.Content.Intent startService = new Android.Content.Intent(Android.App.Application.Context, typeof(NotificationReceiverService));
                NotificationReceiverService notificationReceiverService = new NotificationReceiverService();
                notificationReceiverService.StartService(startService);
            }
            catch { }
        }

        private bool IsNotificationListenerEnabled()
        {
            var enabledListeners = Android.Provider.Settings.Secure.GetString(ContentResolver, "enabled_notification_listeners");
            if (enabledListeners == null || this.PackageName == null)
                return false;
            return enabledListeners != null && enabledListeners.Contains(PackageName ?? "");
        }

        private void OpenNotificationAccessSettings()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                var intent = new Intent(Android.Provider.Settings.ActionNotificationListenerSettings);
                StartActivity(intent);
            }
        }
    }
}
