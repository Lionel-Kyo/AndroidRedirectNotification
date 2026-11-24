# Android Redirect Notification

An end-to-end encrypted notification forwarding solution that seamlessly mirrors notifications from your Android device to your Windows desktop in real time.

---

## Key Features

- **Next-Gen Authenticated Cryptography**:
  - **Ed25519** public-key digital signatures: Server authenticates its identity to prevent Man-in-the-Middle (MITM) attacks.
  - **X25519** Elliptic-Curve Diffie-Hellman (ECDH): Generates ephemeral session keys per transmission for forward secrecy.
  - **HKDF-SHA256**: Secure key derivation combining ephemeral public keys.
  - **AES-256-GCM** (AEAD): Authenticated payload encryption guaranteeing confidentiality and data integrity.
- **Native AOT Compiled Windows Server**:
  - Compiles to a fully standalone machine-code binary with instant startup and zero JIT overhead.
  - Self-contained graphics runtimes: Embedded `libSkiaSharp`, `libHarfBuzzSharp`, `libsodium`, and `av_libGLESv2` with automated runtime extraction.
- **Windows WinRT Toast Notifications**:
  - Native integration with Windows 10/11 Action Center via WinRT `ToastNotificationManager`.
  - Automatic AppUserModelID (AUMID) registration.
  - Configurable toast suppression for notification updates and custom category/flag blocklists.
- **Smart Notification Lifecycle Tracking**:
  - Tracks notification status: `New`, `Updated`, and `Removed`.
  - Notification replacement in table view to keep the active list clean and organized.
  - Historical snapshot inspector: Review the update timeline, attachment pictures/icons, and raw Android notification metadata for every notification slot.
- **Client-Side Deduplication Engine**:
  - Android-side debouncing and active-slot tracking suppress rapid re-posts and progress spam before decoding bitmaps or establishing network sockets.
  - Configurable debounce interval and content-only matching mode.
  - Substantially reduces mobile battery drain, CPU usage, and network traffic.
- **Offline History with Retention Management**:
  - Local persistent notification store on the mobile client with configurable retention policies (seconds, minutes, hours, days, or unlimited).

---

## Architecture & Technology Stack

### 🖥️ Windows Server
* **Target Framework**: .NET 9 (`net9.0-windows10.0.17763.0`)
* **UI Engine**: [Avalonia UI 12.2](https://avaloniaui.net/) (Fluent Dark Theme, DataGrid, Compiled Bindings)
* **Compilation**: Native AOT (`PublishAot=true`) or Self-Contained Release
* **Cryptography**: [NSec.Cryptography](https://nsec.ca/) (libsodium) — Ed25519, X25519, HKDF-SHA256, AES-256-GCM
* **Serialization**: System.Text.Json source generation (`JsonSerializerContext`)
* **Notifications**: Windows WinRT `ToastNotificationManager`

### 📱 Android Client
* **Target Framework**: .NET 9 MAUI (`net9.0-android`)
* **System Service**: Android `NotificationListenerService` (`OnNotificationPosted` & `OnNotificationRemoved`)
* **Cryptography**: NSec.Cryptography with pinned server Ed25519 public key (`server_ed25519.pub`)
* **Deduplication**: Monotonic tick-based `DuplicateNotificationTracker` evaluating active notification slot state and content hashes
* **Storage**: Local JSON history with automated rolling cleanup

---

## Cryptography & Key Management

Each transmission follows a cryptographic handshake powered by libsodium:

```
Android Client                                              Windows Server
      |                                                           |
      | <------- [32-byte X25519 PubKey + 64-byte Ed25519 Sig] -- |  (Server sends ephemeral
      |                                                           |   key signed with its
      |  (Client verifies signature using server_ed25519.pub)     |   persistent Ed25519 key)
      |                                                           |
      | -- [32-byte Client X25519 Ephemeral PubKey] ------------> |
      |                                                           |
      +========== [X25519 ECDH + HKDF-SHA256 Key Derivation] =====+  (Both derive mutual
      |                                                           |   AES-256-GCM session key)
      | -- [AES-256-GCM Encrypted Notification Payload] --------> |
      |                                                           |
```

### Generating Custom Keypairs
A Python utility is included to generate new Ed25519 keypairs:
```bash
python src/generate_ed25519_keys.py
```
This writes `server_ed25519.key` (private key, embedded in server) and `server_ed25519.pub` (public key, distributed to client).

---

## Configuration & Usage

### 1. Server Configuration
1. Launch `AndroidRedirectNotificationServer.exe`.
2. Open **Settings -> General**:
   * **Port**: Default is `5000` (TCP).
   * **Show Windows Toast**: Toggle desktop toast banners.
   * **Show Windows Toast on Update**: Enable if you wish to see banners when existing notifications update.
   * **Blocked Categories**: Multi-select categories to ignore (e.g. `transport` for music players, `sys`, `service`).
   * **Blocked Flags**: Multi-select notification flags to ignore (e.g. `OngoingEvent`, `ForegroundService`).
3. Click **Apply**. Allow the port through Windows Firewall if prompted.

### 2. Client Configuration
1. Open the Android application and grant **Notification Access** permissions when prompted.
2. Configure connection settings:
   * **IP Address**: Local IP of your Windows PC (e.g., `192.168.1.100`).
   * **Port**: Matches server port (`5000`).
   * **Keep History Duration**: Retention duration for local logs (e.g., `12 Hours`).
   * **Send to Server**: Toggle network forwarding.
   * **Skip Duplicate Messages**: Enable client-side deduplication.
   * **Duplicate Interval (Seconds)**: Debounce suppression window (default: `2.0` seconds).
   * **Match Content Only**: When enabled, notifications with identical title and message are treated as duplicates regardless of internal notification ID or tag.
3. Tap **Apply Configuration**.

---

## Building & Publishing

### Quick Build Scripts
The repository includes automated batch scripts for quick builds:

- **Build Server (Native AOT)**:
  Double-click `src/AndroidRedirectNotificationServer/build win64 release Aot.bat`
- **Build Server (Self-Contained Release)**:
  Double-click `src/AndroidRedirectNotificationServer/build win64 release.bat`
- **Build Android APK**:
  Double-click `src/AndroidRedirectNotificationClient/build apk.bat`

### Manual CLI Commands

#### Build Windows Server (Native AOT)
```bash
cd src/AndroidRedirectNotificationServer
dotnet publish --runtime win-x64 --configuration Release /p:PublishAot=true /p:EnableTrimAnalyzer=false /p:SUPPRESS_NETSDK1175=true
```
The standalone binary will be generated at:
`src/AndroidRedirectNotificationServer/AndroidRedirectNotification/bin/Release/net9.0-windows10.0.17763.0/win-x64/publish/AndroidRedirectNotificationServer.exe`

#### Build Android Client (Release APK)
```bash
cd src/AndroidRedirectNotificationClient
dotnet publish AndroidRedirectNotification/AndroidRedirectNotification.csproj -f net9.0-android -c Release
```
