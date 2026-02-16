using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using Windows.Storage.Streams;

namespace AndroidRedirectNotification
{
    internal class MyTcpListener : IDisposable
    {
        public event Action<MyNotificationData>? OnMessageReceived;
        private Task? listeningTask;
        private TcpListener? listener;
        private CancellationTokenSource? cancelToken;
        private RSA.CryptionKeys rsaKeys;
        public ushort Port { get; private set; }
        public MyTcpListener(ushort port) 
        {
            this.Port = port;
            this.rsaKeys = RSA.MessageByteCryption.GenerateRsaKeys();
        }

        public void Start()
        {
            this.listener = new TcpListener(IPAddress.Any, this.Port);
            this.listener.Start();
            this.cancelToken = new CancellationTokenSource();
            this.listeningTask = StartListeningAsync(this.cancelToken.Token);
        }

        public async Task StopAsync()
        {
            if (this.cancelToken == null)
                return;

            this.cancelToken.Cancel();
            this.listener?.Stop();

            if (this.listeningTask != null)
                await this.listeningTask;

            this.cancelToken = null;
            this.listener = null;
            this.listeningTask = null;
        }

        public void Dispose()
        {
            _ = StopAsync();
        }

        private async Task StartListeningAsync(CancellationToken token)
        {
            if (this.listener == null)
                return;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    TcpClient client = await listener.AcceptTcpClientAsync(token);

                    _ = Task.Run(() => HandleClientAsync(client), token);
                }
                catch (Exception ex) { ExceptionRecord.AddExceptionRecord(ex); }
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            try
            {
                byte[] buffer;
                using (client)
                {
                    using (NetworkStream networkStream = client.GetStream())
                    {
                        MyNetworkStream stream = new MyNetworkStream(networkStream);
                        await stream.WriteAsync(this.rsaKeys.PublicKey);
                        buffer = await stream.ReadAsync();
                        byte[] aesKey = RSA.MessageByteCryption.DecryptRsa(buffer, this.rsaKeys.PrivateKey);
                        await stream.WriteAsync(AES.MessageByteCryption.Encrypt(
                            Encoding.UTF8.GetBytes(
                                JsonSerializer.Serialize(
                                    new Dictionary<string, int>()
                                    {
                                        { "Status", 1 }
                                    }
                                )
                            ), aesKey)
                        );
                        buffer = await stream.ReadAsync();
                        byte[] utf8Message = AES.MessageByteCryption.Decrypt(buffer, aesKey);
                        string message = Encoding.UTF8.GetString(utf8Message);
                        var data = JsonSerializer.Deserialize<MyNotificationData>(message);
                        if (data != null && OnMessageReceived != null)
                            OnMessageReceived(data);
                    }
                }
            }
            catch (Exception ex) { ExceptionRecord.AddExceptionRecord(ex); }
        }
    }
}
