// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Account - Kernel.cs
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

using System.Collections.Generic;
using Comet.Account.Database.Models;

#endregion

namespace Comet.Account
{
    /// <summary>
    ///     Kernel for the server, acting as a central core for pools of models and states
    ///     initialized by the server. Used in database repositories to load data into memory
    ///     from essential tables or tables which require heavy post-processing. Used in the
    ///     server packet process methods for tracking client and world states.
    /// </summary>
    public static class Kernel
    {
        public static Dictionary<string, DbRealm> Realms;
    }
}