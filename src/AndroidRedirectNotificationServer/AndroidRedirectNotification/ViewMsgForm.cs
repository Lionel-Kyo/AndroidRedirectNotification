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
using static System.Net.Mime.MediaTypeNames;

namespace AndroidRedirectNotification
{
    public partial class ViewMsgForm : Form
    {
        public ViewMsgForm(string message, List<string>? base64Images = null)
        {
            InitializeComponent();
            this.KeyPreview = true;

            var imagesHtmlBuilder = new StringBuilder();
            if (base64Images != null)
            {
                foreach (var base64 in base64Images)
                {
                    if (string.IsNullOrWhiteSpace(base64))
                        continue;

                    imagesHtmlBuilder.AppendLine($@"
<div style='margin-bottom:10px;'>
    <img src='data:image/png;base64,{base64}'style='max-width:100%; height:auto;' />
</div>");
                }
            }
            var imagesHtml = imagesHtmlBuilder.ToString();
            if (!string.IsNullOrEmpty(imagesHtml))
            {
                imagesHtml = $@"
<div class='images'>
    {imagesHtml}
</div>";
            }

            var messageHtml = string.IsNullOrEmpty(message) ? "" : $@"
<div class='message'>
    {WebUtility.HtmlEncode(message).Replace("\n", "<br>")}
</div>
";

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

    .image {{
        margin-top: 10px;
    }}

    img {{
        max-width: 100%;
        height: auto;
        border-radius: 6px;
    }}
</style>
</head>
<body>
{messageHtml}
{imagesHtml}
</body>
</html>";
            this.webBrowser.AllowWebBrowserDrop = false;
            this.webBrowser.IsWebBrowserContextMenuEnabled = true;
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
