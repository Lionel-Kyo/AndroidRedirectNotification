using System.Diagnostics;
using System.Text;

namespace AndroidRedirectNotification
{
    internal static class Program
    {
        public static readonly Stopwatch ApplicationTime = Stopwatch.StartNew();
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
            Application.ThreadException += Application_ThreadException;
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Main());
        }

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

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            File.WriteAllText("./Error.log", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {e.Exception}\n", Encoding.UTF8);
            ExceptionRecord.AddExceptionRecord(e.Exception);
        }
    }
}