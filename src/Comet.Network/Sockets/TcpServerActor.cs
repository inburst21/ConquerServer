namespace Comet.Network.Sockets
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;
    using Comet.Network.Packets;
    using Comet.Network.Security;

    /// <summary>
    /// Actors are assigned to accepted client sockets to give connected clients a state
    /// across socket operations. This allows the server to handle multiple receive writes
    /// across single processing reads, and keep a buffer alive for faster operations.
    /// </summary>
    public abstract class TcpServerActor
    {
        // Fields and Properties
        public readonly Memory<byte> Buffer;
        public readonly ICipher Cipher;
        public readonly Socket Socket;
        public readonly uint Partition;
        private readonly object SendLock;

        /// <summary>
        /// Instantiates a new instance of <see cref="TcpServerActor"/> using an accepted
        /// client socket and preallocated buffer from the server listener.
        /// </summary>
        /// <param name="socket">Accepted client socket</param>
        /// <param name="buffer">Preallocated buffer for socket receive operations</param>
        /// <param name="cipher">Cipher for handling client encipher operations</param>
        /// <param name="partition">Packet processing partition, default is disabled</param>
        public TcpServerActor(
            Socket socket, 
            Memory<byte> buffer, 
            ICipher cipher, 
            uint partition = 0)
        {
            this.Buffer = buffer;
            this.Cipher = cipher;
            this.Socket = socket;
            this.Partition = partition;
            this.SendLock = new object();
        }

        /// <summary>
        /// Sends a packet to the game client after encrypting bytes. This may be called
        /// as-is, or overridden to provide channel functionality and thread-safety around
        /// the accepted client socket. By default, this method locks around encryption
        /// and sending data. 
        /// </summary>
        /// <param name="packet">Bytes to be encrypted and sent to the client</param>
        public virtual Task SendAsync(byte[] packet)
        {
            var encrypted = new byte[packet.Length];
            BitConverter.TryWriteBytes(packet, (ushort)packet.Length);
            lock (SendLock)
            {
                try 
                {
                    this.Cipher.Encrypt(packet, encrypted);
                    return this.Socket?.SendAsync(encrypted, SocketFlags.None) ?? Task.CompletedTask;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return Task.CompletedTask;
                }
            }
        }

        /// <summary>
        /// Sends a packet to the game client after encrypting bytes. This may be called
        /// as-is, or overridden to provide channel functionality and thread-safety around
        /// the accepted client socket. By default, this method locks around encryption
        /// and sending data. 
        /// </summary>
        /// <param name="packet">Packet to be encrypted and sent to the client</param>
        public virtual Task SendAsync(IPacket packet)
        {
            return this.SendAsync(packet.Encode());
        }

        /// <summary>
        /// Force closes the client connection.
        /// </summary>
        public virtual void Disconnect()
        {
            this.Socket?.Disconnect(false);
        }

        /// <summary>
        /// Returns the remote IP address of the connected client.
        /// </summary>
        public string IPAddress => 
            (this.Socket.RemoteEndPoint as IPEndPoint).Address.MapToIPv4().ToString();
    }    
}
