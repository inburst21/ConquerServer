﻿// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MessageBox.cs
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

using System.Threading.Tasks;
using Comet.Core;
using Comet.Game.Packets;

#endregion

namespace Comet.Game.States
{
    public class MessageBox
    {
        private TimeOut m_expiration = new TimeOut();
        protected Character m_owner;

        protected MessageBox(Character owner)
        {
            m_owner = owner;
        }

        public virtual string Message { get; protected set; }

        public virtual int TimeOut { get; protected set; }

        public bool HasExpired => TimeOut > 0 && m_expiration.IsTimeOut();

        public virtual Task OnAcceptAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task OnCancelAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task OnTimerAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task SendAsync()
        {
            m_expiration.Startup(TimeOut);
            return m_owner.SendAsync(new MsgTaskDialog
            {
                InteractionType = MsgTaskDialog.TaskInteraction.MessageBox,
                Text = Message,
                OptionIndex = 255,
                Data = (ushort) TimeOut
            });
        }
    }
}