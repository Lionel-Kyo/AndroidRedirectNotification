using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AndroidRedirectNotification
{
    internal static class NativeLibraryHelper
    {
        public static void Initialize()
        {
            try
            {
                var skiaAssembly = typeof(SkiaSharp.SKImageInfo).Assembly;
                NativeLibrary.SetDllImportResolver(skiaAssembly, ResolveDll);

                var harfBuzzAssembly = typeof(HarfBuzzSharp.Buffer).Assembly;
                NativeLibrary.SetDllImportResolver(harfBuzzAssembly, ResolveDll);

                var avaloniaWin32Assembly = typeof(Avalonia.Win32PlatformOptions).Assembly;
                NativeLibrary.SetDllImportResolver(avaloniaWin32Assembly, ResolveDll);

                var nsecAssembly = typeof(NSec.Cryptography.Algorithm).Assembly;
                NativeLibrary.SetDllImportResolver(nsecAssembly, ResolveDll);
            }
            catch (Exception ex)
            {
                ExceptionRecord.AddExceptionRecord(ex);
            }
        }

        private static IntPtr ResolveDll(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (libraryName.Equals("av_libGLESv2", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine();
            }
            // 1. Try standard load first (same directory, system paths)
            if (NativeLibrary.TryLoad(libraryName, assembly, searchPath, out IntPtr handle))
                return handle;

            // 2. Check application directory
            string appDir = AppContext.BaseDirectory;
            string fileName = libraryName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ? libraryName : libraryName + ".dll";
            string localPath = Path.Combine(appDir, fileName);
            if (File.Exists(localPath) && NativeLibrary.TryLoad(localPath, out handle))
                return handle;

            // 3. Check alongside the executable process path
            string? processDir = Path.GetDirectoryName(Environment.ProcessPath);
            if (!string.IsNullOrEmpty(processDir))
            {
                string procPath = Path.Combine(processDir, fileName);
                if (File.Exists(procPath) && NativeLibrary.TryLoad(procPath, out handle))
                    return handle;
            }

            // 4. Check cached LocalAppData directory
            string cacheDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AndroidRedirectNotification", "native");
            string cachedFile = Path.Combine(cacheDir, fileName);
            if (File.Exists(cachedFile) && NativeLibrary.TryLoad(cachedFile, out handle))
                return handle;

            // 5. Try to extract from embedded resources if available
            try
            {
                var currentAssembly = typeof(NativeLibraryHelper).Assembly;
                using var stream = currentAssembly.GetManifestResourceStream($"AndroidRedirectNotification.Native.{fileName}");
                if (stream != null)
                {
                    if (!Directory.Exists(cacheDir))
                        Directory.CreateDirectory(cacheDir);

                    using (var fs = new FileStream(cachedFile, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        stream.CopyTo(fs);
                    }

                    if (NativeLibrary.TryLoad(cachedFile, out handle))
                        return handle;
                }
            }
            catch { }

            return IntPtr.Zero;
        }
    }
}
