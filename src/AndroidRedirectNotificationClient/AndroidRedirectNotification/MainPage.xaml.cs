using System.Collections.ObjectModel;
using System.Net;

namespace AndroidRedirectNotification;

public partial class MainPage : ContentPage
{
    public static MainPage? Instance { get; private set; }
    public static string ServerIp { get; private set; } = "192.168.1.1";
    public static ushort ServerPort { get; private set; } = 443;

    public Entry IpEditEntry => ipEditEntry;
    public Entry PortEditEntry => portEntry;

    public ObservableCollection<NotificationLog> Notifications { get; set; } = new();

    public MainPage()
    {
        InitializeComponent();
        Instance = this;

        historyCollectionView.ItemsSource = Notifications;

        ServerIp = Preferences.Get("server_ip", "192.168.1.1");
        ServerPort = (ushort)Preferences.Get("server_port", 443);
        this.ipEditEntry.Text = ServerIp;
        this.portEntry.Text = ServerPort.ToString();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    public void AddNotification(string message, string title, DateTimeOffset utcDateTime)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Notifications.Insert(0, new NotificationLog
            {
                Title = title,
                Message = message,
                Timestamp = utcDateTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss")
            });
        });
    }

    public static void LogNotification(string message, string title, DateTimeOffset utcDateTime)
    {
        Instance?.AddNotification(message, title, utcDateTime);
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
            await DisplayAlert("Server Configuration", "Invalid ip or host.", "OK");
            return;
        }

        if (!ushort.TryParse(newPort, out ushort u16Port) || u16Port == 0)
        {
            await DisplayAlert("Server Configuration", "Invalid port.", "OK");
            return;
        }

        ServerIp = newIp;
        ServerPort = u16Port;
        Preferences.Set("server_ip", newIp);
        Preferences.Set("server_port", u16Port);

        await DisplayAlert("Server Configuration", "Applied server information.", "OK");
    }

    private async void OnClearHistoryClicked(object sender, EventArgs e)
    {
        if (Notifications.Count == 0)
        {
            await DisplayAlert("Clear History", "No notification record.", "OK");
        }
        else
        {
            bool isConfirmed = await DisplayAlert(
                "Clear History",
                "Are you sure you want to delete all notification records?",
                "Clear",
                "Cancel"
            );

            if (isConfirmed)
            {
                Notifications.Clear();
            }
        }
    }
}

public class NotificationLog
{
    public string Title { get; set; } = "";
    public string Message { get; set; } = "";
    public string Timestamp { get; set; } = "";
}