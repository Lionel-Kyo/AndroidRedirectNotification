using System.Diagnostics;

namespace AndroidRedirectNotification_PC
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
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Main());
        }
    }
}