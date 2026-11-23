using System;
using System.IO;
using System.Runtime.InteropServices;

namespace AndroidRedirectNotification
{
    internal static class ToastHelper
    {
        public const string AppId = "AndroidRedirectNotification";

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern int SetCurrentProcessExplicitAppUserModelID([MarshalAs(UnmanagedType.LPWStr)] string AppID);

        public static void Initialize()
        {
            try
            {
                // 1. Explicitly associate the current process with this AppId
                SetCurrentProcessExplicitAppUserModelID(AppId);

                // 2. Register the AUMID in HKCU\SOFTWARE\Classes\AppUserModelId so Windows Toast Notification platform recognizes it
                using (var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey($@"SOFTWARE\Classes\AppUserModelId\{AppId}"))
                {
                    if (key != null)
                    {
                        key.SetValue("DisplayName", "Android Redirect Notification", Microsoft.Win32.RegistryValueKind.String);

                        string? exePath = Environment.ProcessPath;
                        if (!string.IsNullOrEmpty(exePath) && File.Exists(exePath))
                        {
                            key.SetValue("IconUri", exePath, Microsoft.Win32.RegistryValueKind.String);
                        }
                    }
                }

                // 3. Ensure notifications are enabled for this app in Windows Settings
                using (var settingsKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey($@"SOFTWARE\Microsoft\Windows\CurrentVersion\Notifications\Settings\{AppId}"))
                {
                    if (settingsKey != null)
                    {
                        if (settingsKey.GetValue("Enabled") == null)
                            settingsKey.SetValue("Enabled", 1, Microsoft.Win32.RegistryValueKind.DWord);
                        if (settingsKey.GetValue("ShowInActionCenter") == null)
                            settingsKey.SetValue("ShowInActionCenter", 1, Microsoft.Win32.RegistryValueKind.DWord);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionRecord.AddExceptionRecord(ex);
            }
        }
    }
}
