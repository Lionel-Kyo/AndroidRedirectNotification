using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AndroidRedirectNotification
{
    internal partial class ExcerptionRecordViewer : Form
    {
        private object dgvLock;

        public ExcerptionRecordViewer()
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.dgvLock = new object();
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
            lock (this.dgvLock)
            {
                int i = this.dgv.Rows.Add();
                DataGridViewRow row = this.dgv.Rows[i];
                row.Cells["dgvDateTime"].Value = $"{record.DateTime: yyyy-MM-dd HH:mm:ss}";
                row.Cells["dgvName"].Value = record.Exception.GetType().Name;
                row.Cells["dgvMessage"].Value = record.Exception.Message;
                row.Cells["dgvRecord"].Value = record;
            }
        }

        private void ClearRecord()
        {
            lock (this.dgvLock)
            {
                this.dgv.Rows.Clear();
            }
        }

        private void ExcerptionRecordViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            ExceptionRecord.OnRecordAdded -= AddRecord;
            ExceptionRecord.OnRecordsCleared -= ClearRecord;
        }

        private void ExcerptionRecordViewer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
