using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using System;
using System.Collections.ObjectModel;

namespace AndroidRedirectNotification
{
    internal partial class ExceptionRecordViewerWindow : Window
    {
        public ObservableCollection<ExceptionRecordRowItem> ExceptionRecords { get; } = new();

        public ExceptionRecordViewerWindow()
        {
            InitializeComponent();
            dgv.ItemsSource = ExceptionRecords;

            ExceptionRecord.OnRecordAdded += AddRecord;
            ExceptionRecord.OnRecordsCleared += ClearRecord;

            ExceptionRecord.UseExceptionRecords(records =>
            {
                foreach (var record in records.Span)
                {
                    AddRecord(record);
                }
            });
        }

        private void AddRecord(ExceptionRecord record)
        {
            // Thread-safe dispatch ensures records added from background threads won't crash the UI thread
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                ExceptionRecords.Add(new ExceptionRecordRowItem
                {
                    DateTime = $"{record.DateTime: yyyy-MM-dd HH:mm:ss}",
                    Name = record.Exception.GetType().Name,
                    Message = record.Exception.Message,
                    Record = record
                });
            });
        }

        private void ClearRecord()
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                ExceptionRecords.Clear();
            });
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            ExceptionRecord.OnRecordAdded -= AddRecord;
            ExceptionRecord.OnRecordsCleared -= ClearRecord;
            base.OnClosing(e);
        }

        private void ExceptionRecordViewer_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}