using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace AndroidRedirectNotification
{
    internal class MyTcpListener : IDisposable
    {
        public event Action<MyNotificationData>? OnMessageReceived;
        private bool isRun;
        private TcpListener? listener;
        private Thread? listenerThread;
        public ushort Port { get; private set; }
        public MyTcpListener(ushort port) 
        {
            this.Port = port;
        }

        public void Start()
        {
            this.listener = new TcpListener(IPAddress.Any, this.Port);
            this.listener.Start();
            this.isRun = true;
            this.listenerThread = new Thread(this.Listening);
            this.listenerThread.IsBackground = true;
            this.listenerThread.Start();
        }

        public void Stop()
        {
            this.isRun = false;
            this.listener?.Dispose();
            this.listenerThread?.Join();
        }

        public void Dispose() => this.Stop();

        public void Listening()
        {
            if (this.listener == null)
                return;

            while (this.isRun)
            {
                try
                {
                    using (TcpClient client = this.listener.AcceptTcpClient())
                    using (NetworkStream networkStream = client.GetStream())
                    {
                        MyNetworkStream stream = new MyNetworkStream(networkStream);
                        byte[] buffer;
                        var rsaKeys = RSA.MessageByteCryption.GenerateRsaKeys();
                        stream.Write(rsaKeys.PublicKey);
                        buffer = stream.Read();
                        byte[] aesKey = RSA.MessageByteCryption.DecryptRsa(buffer, rsaKeys.PrivateKey);
                        stream.Write(AES.MessageByteCryption.Encrypt(
                            Encoding.UTF8.GetBytes(
                                JsonSerializer.Serialize(
                                    new Dictionary<string, int>()
                                    {
                                        { "Status", 1 }
                                    }
                                )
                            ), aesKey)
                        );
                        buffer = stream.Read();
                        byte[] utf8Message = AES.MessageByteCryption.Decrypt(buffer, aesKey);
                        string message = Encoding.UTF8.GetString(utf8Message);
                        var data = JsonSerializer.Deserialize<MyNotificationData>(message);
                        if (data != null && OnMessageReceived != null)
                            OnMessageReceived(data);
                    }
                }
                catch { }
            }
        }
    }
}
