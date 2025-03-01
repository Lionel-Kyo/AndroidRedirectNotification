using Microsoft.Maui.Controls;
using System.Net;

namespace AndroidRedirectNotification
{
    public partial class MainPage : ContentPage
    {
        public static MainPage? Instance { get; private set; }
        public static string ServerIp { get; private set; } = "192.168.1.1";
        public static ushort ServerPort { get; private set; } = 443;

        public Label WelcomeLabel => welcomeLabel;
        public Entry IpEditEntry => ipEditEntry;
        public Entry PortEditEntry => portEntry;

        public MainPage()
        {
            InitializeComponent(); 
            Instance = this;
            ServerIp = Preferences.Get("server_ip", "192.168.1.1");
            ServerPort = (ushort)Preferences.Get("server_port", 443);
            this.ipEditEntry.Text = ServerIp;
            this.PortEditEntry.Text = ServerPort.ToString();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private static bool IsValidIpAddress(string input)
        {
            return IPAddress.TryParse(input, out _);
        }

        private static bool IsValidHostName(string input)
        {
            try
            {
                var addresses = Dns.GetHostAddresses(input);
                return addresses.Length > 0;
            }
            catch { }
            return false;
        }

        private async void OnEditBtnClicked(object sender, EventArgs e)
        {
            string newIp = this.ipEditEntry.Text;
            string newPort = this.portEntry.Text;
            if (!IsValidIpAddress(newIp) && !IsValidHostName(newIp))
            {
                await DisplayAlert("Error", "Invalid ip or host.", "OK");
                return;
            }
            if (!ushort.TryParse(newPort, out ushort u16Port) || u16Port == 0)
            {
                await DisplayAlert("Error", "Invalid port.", "OK");
                return;
            }
            ServerIp = newIp;
            ServerPort = u16Port;
            Preferences.Set("server_ip", newIp);
            Preferences.Set("server_port", u16Port);
            await DisplayAlert("Infomation", "Applied server infomation.", "OK");
            return;
        }
    }

}
