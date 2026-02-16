using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AndroidRedirectNotification
{
    internal class ExceptionRecord
    {
        public static event Action<ExceptionRecord>? OnRecordAdded;
        public static event Action? OnRecordsCleared;
        private static object exceptionRecordsLock = new object();
        private static List<ExceptionRecord> exceptionRecords = [];

        public DateTime DateTime { get; set; }
        public Exception Exception { get; set; }

        public ExceptionRecord(DateTime datetime, Exception exception) 
        { 
            this.DateTime = DateTime.Now;
            this.Exception = exception;
        }

        public ExceptionRecord(Exception exception) : this(DateTime.Now, exception)
        {
        }

        public static void AddExceptionRecord(Exception ex)
        {
            var record = new ExceptionRecord(ex);
            lock (exceptionRecordsLock)
            {
                exceptionRecords.Add(record);
            }

            OnRecordAdded?.Invoke(record);
        }

        public static void UseExceptionRecords(Action<ReadOnlyMemory<ExceptionRecord>> action)
        {
            lock (exceptionRecordsLock)
            {
                var records = new ExceptionRecord[exceptionRecords.Count];
                exceptionRecords.CopyTo(0, records, 0, exceptionRecords.Count);
                action(new ReadOnlyMemory<ExceptionRecord>(records));
            }
        }

        public static void ClearExceptionRecords()
        {
            exceptionRecords = new List<ExceptionRecord>();
            OnRecordsCleared?.Invoke();
        }
    }
}
