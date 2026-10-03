using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;

namespace AndroidRedirectNotification
{
    internal partial class ViewMsgWindow : Window
    {
        public ViewMsgWindow()
        {
            InitializeComponent();
        }

        public ViewMsgWindow(string message, List<string>? base64Images = null) : this()
        {
            txtMessage.Text = message ?? string.Empty;

            if (base64Images != null)
            {
                var bitmaps = new List<Bitmap>();
                foreach (var base64 in base64Images)
                {
                    if (string.IsNullOrWhiteSpace(base64))
                        continue;

                    try
                    {
                        byte[] bytes = Convert.FromBase64String(base64);
                        using var ms = new MemoryStream(bytes);
                        bitmaps.Add(new Bitmap(ms));
                    }
                    catch (Exception ex)
                    {
                        ExceptionRecord.AddExceptionRecord(ex);
                    }
                }

                imgContainer.ItemsSource = bitmaps;
            }
        }

        private void ViewMsgWindow_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}