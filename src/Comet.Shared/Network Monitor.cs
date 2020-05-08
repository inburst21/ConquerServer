// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Shared - Network Monitor.cs
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

using System.Threading;
using System.Threading.Tasks;

#endregion

namespace Comet.Shared
{
    /// <summary>
    ///     A monitor for the networking I/O. From COPS V6 Enhanced Edition.
    /// </summary>
    public sealed class NetworkMonitor
    {
        /// <summary>
        ///     The title of the console.
        /// </summary>
        private const string FORMAT_S = "(↑{0:F2} kbps, ↓{1:F2} kbps)";

        private long m_TotalRecvBytes = 0;
        /// <summary>
        ///     The number of bytes received by the server.
        /// </summary>
        private int m_RecvBytes = 0;

        private long m_TotalSentBytes = 0;
        /// <summary>
        ///     The number of bytes sent by the server.
        /// </summary>
        private int m_SentBytes = 0;

        /// <summary>
        ///     Called by the timer.
        /// </summary>
        public Task<string> UpdateStatsAsync(int interval)
        {
            double download = m_RecvBytes / (double) interval * 8.0 / 1024.0;
            double upload = m_SentBytes / (double) interval * 8.0 / 1024.0;

            m_RecvBytes = 0;
            m_SentBytes = 0;

            return Task.FromResult(string.Format(FORMAT_S, upload, download));
        }

        /// <summary>
        ///     Signal to the monitor that aLength bytes were sent.
        /// </summary>
        /// <param name="aLength">The number of bytes sent.</param>
        public void Send(int aLength)
        {
            Interlocked.Add(ref m_SentBytes, aLength);
            Interlocked.Add(ref m_TotalSentBytes, aLength);
        }

        /// <summary>
        ///     Signal to the monitor that aLength bytes were received.
        /// </summary>
        /// <param name="aLength">The number of bytes received.</param>
        public void Receive(int aLength)
        {
            Interlocked.Add(ref m_RecvBytes, aLength);
            Interlocked.Add(ref m_TotalRecvBytes, aLength);
        }
    }
}