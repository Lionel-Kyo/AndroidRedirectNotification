using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidRedirectNotification
{
    internal class ExceptionRecordRowItem
    {
        public string DateTime { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public ExceptionRecord Record { get; set; } = null!;
    }
}
