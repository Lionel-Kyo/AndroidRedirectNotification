using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AndroidRedirectNotification
{
    internal class MyNetworkStream
    {
        public NetworkStream Stream { get; private set; }
        public MyNetworkStream(NetworkStream stream)
        {
            this.Stream = stream;
        }

        public void Write(Span<byte> bytes)
        {
            this.Stream.Write(BitConverter.GetBytes(bytes.Length));
            this.Stream.Write(bytes);
        }

        public async Task WriteAsync(byte[] bytes)
        {
            await this.Stream.WriteAsync(BitConverter.GetBytes(bytes.Length));
            await this.Stream.WriteAsync(bytes);
        }

        public async Task WriteAsync(ReadOnlyMemory<byte> bytes)
        {
            await this.Stream.WriteAsync(BitConverter.GetBytes(bytes.Length));
            await this.Stream.WriteAsync(bytes);
        }

        public byte[] Read()
        {
            byte[] buffer = new byte[4];
            int bytesRead = this.Stream.Read(buffer, 0, buffer.Length);
            if (bytesRead < 4)
                throw new IOException("Failed to read the message length.");

            int expectedLength = BitConverter.ToInt32(buffer, 0);
            if (expectedLength <= 0)
                throw new InvalidOperationException("Invalid data length received.");

            buffer = new byte[expectedLength];
            int totalRead = 0;

            while (totalRead < expectedLength)
            {
                int remaining = expectedLength - totalRead;
                int read = this.Stream.Read(buffer, totalRead, remaining);
                if (read == 0)
                    throw new IOException("Connection closed");

                totalRead += read;
            }

            return buffer;
        }

        public async Task<byte[]> ReadAsync()
        {
            byte[] buffer = new byte[4];
            int bytesRead = await this.Stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead < 4)
                throw new IOException("Failed to read the message length.");

            int expectedLength = BitConverter.ToInt32(buffer, 0);
            if (expectedLength <= 0)
                throw new InvalidOperationException("Invalid data length received.");

            buffer = new byte[expectedLength];
            int totalRead = 0;

            while (totalRead < expectedLength)
            {
                int remaining = expectedLength - totalRead;
                int read = await this.Stream.ReadAsync(buffer, totalRead, remaining);
                if (read == 0)
                    throw new IOException("Connection closed");

                totalRead += read;
            }

            return buffer;
        }
    }
}
