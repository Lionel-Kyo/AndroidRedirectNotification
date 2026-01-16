using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AndroidRedirectNotification
{
    public partial class ViewMsgForm : Form
    {
        public ViewMsgForm(string message)
        {
            InitializeComponent();
            this.KeyPreview = true;
            this.webBrowser.DocumentText = $@"
<!DOCTYPE html>
<html>
<head>
<meta charset='utf-8'>
<style>
    body {{
        font-family: 'Segoe UI Emoji', 'Segoe UI', sans-serif;
        font-size: 20px;
        margin: 0;
        padding: 8px;
        background-color: white;
    }}
</style>
</head>
<body>
{WebUtility.HtmlEncode(message)}
</body>
</html>";
            this.webBrowser.AllowWebBrowserDrop = false;
            this.webBrowser.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser.ScriptErrorsSuppressed = true;
        }

        private void ViewMsgForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
