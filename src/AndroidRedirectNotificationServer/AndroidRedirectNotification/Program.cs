using Avalonia;
using Avalonia.Markup.Xaml;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AndroidRedirectNotification
{

    internal static class Program
    {
        public static readonly Stopwatch ApplicationTime = Stopwatch.StartNew();
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();

        private static void CurrentDomain_FirstChanceException(object? sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            File.AppendAllText("FirstChance.log", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {e.Exception}\n", Encoding.UTF8);
            ExceptionRecord.AddExceptionRecord(e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                File.WriteAllText("./Error.log", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {ex}\n", Encoding.UTF8);
                ExceptionRecord.AddExceptionRecord(ex);
            }
        }

        private static void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            File.WriteAllText("./Error.log", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {e.Exception}\n", Encoding.UTF8);
            ExceptionRecord.AddExceptionRecord(e.Exception);
        }
    }
}