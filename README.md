# Android Redirect Notification

An end-to-end encrypted notification forwarding solution that seamlessly mirrors notifications from your Android device to your Windows desktop in real time.

---

## Features

- **End-to-End Encryption**: Every connection establishes an ephemeral RSA-2048 handshake and AES-CBC symmetric encryption, ensuring notification contents, titles, and media attachments remain confidential across local networks and Wi-Fi.
- **Native Windows Toast Integration**: Native integration with the Windows 10/11 Notification Platform and Action Center for desktop banner alerts.
- **Native AOT Compiled Server**: The Windows server compiles down to a native machine-code binary (Native AOT) with instant startup, zero JIT overhead, and self-contained Skia/HarfBuzz graphics runtimes.
- **Client-Side Duplicate Suppression**: High-performance deduplication on the mobile device suppresses repeated or pulsing notifications (e.g. typing indicators, progress bars) before encoding bitmaps or opening network connections—preserving phone battery and reducing network bandwidth.
- **Attachment & Media Support**: Forwards contact icons, notification picture attachments, action button titles, and notification flags.
- **Local Notification History**: The client maintains an offline notification log with configurable retention duration (seconds, minutes, hours, days, or permanent).

---

## Architecture & Technology Stack

### Windows Server
* **Framework**: .NET 9 (`net9.0-windows10.0.17763.0`)
* **UI**: [Avalonia UI](https://avaloniaui.net/) (Fluent Dark Theme, DataGrid, Compiled Bindings)
* **Compilation**: Native AOT (`PublishAot=true`) for standalone native execution
* **Notifications**: Windows WinRT `ToastNotificationManager` with explicit AppUserModelID registration
* **Serialization**: System.Text.Json compile-time source generation (`JsonSerializerContext`)
* **Cryptography**: RSA-2048 + AES-256-CBC

### Android Client
* **Framework**: .NET 8 MAUI (`net8.0-android`)
* **System Service**: Android `NotificationListenerService`
* **Deduplication Engine**: Monotonic tick-based debounce tracker with configurable window and content-only matching mode
* **Storage**: Local persistent JSON storage with automated cleanup based on retention rules

---

## Configuration & Usage

### 1. Server Setup
1. Launch `AndroidRedirectNotificationServer.exe`.
2. Click **Settings -> General** to configure:
   * **Port**: Default is `443` (or any custom TCP port).
   * **Show Windows Notifications**: Enable or disable Windows desktop toast banners.
3. Allow the chosen port through the Windows Firewall if prompted.

### 2. Client Setup
1. Install and open the Android application.
2. Grant **Notification Access** permissions when prompted in Android system settings.
3. In **Server Configuration**, specify:
   * **IP Address**: The local IP address of your Windows computer (e.g., `192.168.1.100`).
   * **Port**: Matching the port configured on the server.
   * **Keep History Duration**: How long past notifications are preserved locally.
   * **Send to Server**: Toggle real-time forwarding on/off.
   * **Skip Duplicate Messages**: Enable or disable client-side duplicate suppression.
   * **Duplicate Interval (Seconds)**: Debounce interval window (default: `2.0` seconds).
   * **Match Content Only**: Compare title and message content regardless of internal notification ID changes.
4. Tap **Apply Configuration**.

---

## Building from Source

### Prerequisites
* [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* C++ Desktop Development tools (Visual Studio 2022 / Build Tools) for Native AOT linking
* .NET MAUI Android workload (`dotnet workload install maui-android`)

### Build Server (Native AOT)
```bash
dotnet publish src/AndroidRedirectNotificationServer/AndroidRedirectNotification/AndroidRedirectNotification.csproj -c Release -r win-x64
```
The standalone native executable will be located in:
`src/AndroidRedirectNotificationServer/AndroidRedirectNotification/bin/Release/net9.0-windows10.0.17763.0/win-x64/publish/`

### Build Android Client
```bash
dotnet publish src/AndroidRedirectNotificationClient/AndroidRedirectNotification/AndroidRedirectNotification.csproj -f net8.0-android -c Release --no-restore
```
