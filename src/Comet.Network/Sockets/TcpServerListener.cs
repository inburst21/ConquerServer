// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Network - TcpServerListener.cs
// Description:
// 
// Creator: FELIPEVIEIRAVENDRAMI [FELIPE VIEIRA VENDRAMINI]
// 
// Developed by:
// Felipe Vieira Vendramini <felipevendramini@live.com>
// 
// Programming today is a race between software engineers striving to build bigger and better
// idiot-proof programs, and the Universe trying to produce bigger and better idiots.
// So far, the Universe is winning.
// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

#region References

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Comet.Shared;

#endregion

namespace Comet.Network.Sockets
{
    /// <summary>
    ///     TcpServerListener implements an asynchronous TCP streaming socket server for high
    ///     performance server logic. Socket operations are processed in value tasks using
    ///     Socket Task Extensions. Inherits from a base class for providing socket operation
    ///     event handling to the non-abstract derived class of TcpServerListener.
    /// </summary>
    /// <typeparam name="TActor">Type of actor passed by the parent project</typeparam>
    public abstract class TcpServerListener<TActor> : TcpServerEvents<TActor>
        where TActor : TcpServerActor
    {
        // Fields and properties
        private readonly Semaphore AcceptanceSemaphore;
        private readonly ConcurrentStack<Memory<byte>> BufferPool;
        private readonly bool EnableKeyExchange;
        private readonly int FooterLength;
        private readonly TaskFactory ReceiveTasks;
        private readonly CancellationTokenSource ShutdownToken;
        private readonly Socket Socket;

        /// <summary>
        /// Instantiates a new instance of <see cref="TcpServerListener"/> with a new server
        /// socket for accepting remote or local client connections. Creates preallocated
        /// buffers for receiving data from clients without expensive allocations per receive
        /// operation.
        /// </summary>
        /// <param name="maxConn">Maximum number of clients connected</param>
        /// <param name="bufferSize">Preallocated buffer size in bytes</param>
        /// <param name="delay">Use Nagel's algorithm to delay sending smaller packets</param>
        /// <param name="exchange">Use a key exchange before receiving packets</param>
        /// <param name="footerLength">Length of the packet footer</param>
        protected TcpServerListener(
            int maxConn = 500,
            int bufferSize = 4096,
            bool delay = false,
            bool exchange = false,
            int footerLength = 0)
        {
            // Initialize and configure server socket
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.LingerState = new LingerOption(false, 0);
            Socket.NoDelay = !delay;

            // Initialize management mechanisms
            AcceptanceSemaphore = new Semaphore(maxConn, maxConn);
            BufferPool = new ConcurrentStack<Memory<byte>>();
            EnableKeyExchange = exchange;
            FooterLength = footerLength;
            ShutdownToken = new CancellationTokenSource();
            ReceiveTasks = new TaskFactory(ShutdownToken.Token);

            // Initialize preallocated buffer pool
            for (int i = 0; i < maxConn; i++)
                BufferPool.Push(new Memory<byte>(new byte[bufferSize]));
        }

        /// <summary>
        ///     Binds the server listener to a port and network interface. Specify "0.0.0.0"
        ///     as the endpoint address to bind to all interfaces on the host machine. Starts
        ///     the server listener and accepts new connections in a new task.
        /// </summary>
        /// <param name="port">Port number the server will bind to</param>
        /// <param name="address">Interface IPv4 address the server will bind to</param>
        /// <param name="backlog">Maximum connections backlogged for acceptance</param>
        /// <returns>Returns a new task for accepting new connections.</returns>
        public Task StartAsync(int port, string address = "0.0.0.0", int backlog = 100)
        {
            Socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            Socket.Listen(backlog);
            return AcceptingAsync();
        }

        /// <summary>
        ///     Accepting accepts client connections asynchronously as a new task. As a client
        ///     connection is accepted, it will be associated with a preallocated buffer and
        ///     a receive task. The accepted socket event will be called after accept.
        /// </summary>
        /// <returns>Returns task details for fault tolerance processing.</returns>
        private async Task AcceptingAsync()
        {
            while (this.Socket.IsBound && !this.ShutdownToken.IsCancellationRequested)
            {
                // Block if the maximum connections has been reached. Holds all connection
                // attempts in the backlog of the server socket until a client disconnects
                // and a new client can be accepted. Check shutdown every 5 seconds.
                if (this.AcceptanceSemaphore.WaitOne(TimeSpan.FromSeconds(5)))
                {
                    // Pop a preallocated buffer and accept a client
                    if (BufferPool.IsEmpty)
                        BufferPool.Push(new Memory<byte>(new byte[4096]));

                    this.BufferPool.TryPop(out var buffer);
                    var socket = await this.Socket.AcceptAsync();
                    var actor = await this.AcceptedAsync(socket, buffer);

                    await Log.WriteLogAsync(LogLevel.Debug, $"BufferPool remaining: {BufferPool.Count}");

                    // Start receiving data from the client connection
                    if (this.EnableKeyExchange)
                    {
                        var task = this.ReceiveTasks
                            .StartNew(this.ExchangingAsync, actor, this.ShutdownToken.Token)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        var task = this.ReceiveTasks
                            .StartNew(this.ReceivingAsync, actor, this.ShutdownToken.Token)
                            .ConfigureAwait(false);
                    }
                }
            }
        }

        /// <summary>
        /// Exchanging receives bytes from the accepted client socket when bytes become
        /// available as a raw buffer of bytes. This method is called once and then invokes
        /// <see cref="ReceivingAsync"/>.
        /// </summary>
        /// <param name="state">Created actor around the accepted client socket</param>
        /// <returns>Returns task details for fault tolerance processing.</returns>
        private async Task ExchangingAsync(object state)
        {
            // Initialize multiple receive variables
            var actor = state as TActor;
            int examined = 0;
            if (actor.Socket.Connected && !this.ShutdownToken.IsCancellationRequested)
            {
                try
                {
                    // Receive data from the client socket
                    examined = await actor.Socket.ReceiveAsync(
                        actor.Buffer.Slice(0),
                        SocketFlags.None,
                        this.ShutdownToken.Token);
                    if (examined == 0) return;
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode < SocketError.ConnectionAborted ||
                        e.SocketErrorCode > SocketError.Shutdown)
                        Console.WriteLine(e);

                    actor.Disconnect();
                    return;
                }

                // Decrypt traffic
                actor.Cipher.Decrypt(
                    actor.Buffer.Slice(0, examined).Span,
                    actor.Buffer.Slice(0, examined).Span);

                // Process the buffer
                if (!Exchanged(actor, actor.Buffer.Slice(0, examined).Span))
                {
                    actor.Disconnect();
                    return;
                }
            }

            // Start receiving packets
            await ReceivingAsync(state);
        }

        /// <summary>
        /// Receiving receives bytes from the accepted client socket when bytes become
        /// available. While the client is connected and the server hasn't issued the 
        /// shutdown signal, bytes will be received in a loop.
        /// </summary>
        /// <param name="state">Created actor around the accepted client socket</param>
        /// <returns>Returns task details for fault tolerance processing.</returns>
        private async Task ReceivingAsync(object state)
        {
            // Initialize multiple receive variables
            var actor = state as TActor;
            int consumed = 0, examined = 0, remaining = 0;
            while (actor.Socket.Connected && !this.ShutdownToken.IsCancellationRequested)
            {
                try
                {
                    // Receive data from the client socket
                    examined = await actor.Socket.ReceiveAsync(
                        actor.Buffer.Slice(remaining),
                        SocketFlags.None,
                        this.ShutdownToken.Token);
                    if (examined == 0) break;
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode < SocketError.ConnectionAborted ||
                        e.SocketErrorCode > SocketError.Shutdown)
                        Console.WriteLine(e);
                    break;
                }

                // Decrypt traffic
                actor.Cipher.Decrypt(
                    actor.Buffer.Slice(remaining, examined).Span,
                    actor.Buffer.Slice(remaining, examined).Span);

                // Handle splitting and processing of data
                if (!this.Splitting(actor, examined + remaining, ref consumed))
                {
                    actor.Disconnect();
                    break;
                }

                remaining = examined + remaining - consumed;
                actor.Buffer.Slice(consumed, remaining).CopyTo(actor.Buffer);
            }

            // Disconnect the client
            this.Disconnecting(actor);
        }

        /// <summary>
        /// Splitting splits the actor's receive buffer into multiple packets that can
        /// then be processed by Received individually. The default behavior of this method
        /// unless otherwise overridden is to split packets from the buffer using an unsigned
        /// short packet header for the length of each packet.
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="examined">Number of examined bytes from the receive</param>
        /// <param name="consumed">Number of consumed bytes by the split reader</param>
        /// <returns>Returns true if the client should remain connected.</returns>
        protected virtual bool Splitting(TActor actor, int examined, ref int consumed)
        {
            // Consume packets from the socket buffer
            consumed = 0;
            var buffer = actor.Buffer.Span;
            while (consumed + 2 < examined)
            {
                var length = BitConverter.ToUInt16(buffer.Slice(consumed, 2));
                var expected = consumed + length + this.FooterLength;
                if (expected > buffer.Length) return false;
                if (expected > examined) break;

                this.Received(actor, buffer.Slice(consumed, length + this.FooterLength));
                consumed += length + this.FooterLength;
            }

            return true;
        }

        /// <summary>
        ///     Disconnecting is called when the client is disconnecting from the server. Allows
        ///     the server to handle client events post-disconnect, and reclaim resources first
        ///     leased to the client on accept.
        /// </summary>
        /// <param name="actor">Actor being disconnected</param>
        private void Disconnecting(TActor actor)
        {
            // Reclaim resources and release back to server pools
            actor.Buffer.Span.Clear();

            BufferPool.Push(actor.Buffer);
            AcceptanceSemaphore.Release();

            // Complete processing for disconnect
            Disconnected(actor);
        }
    }
}