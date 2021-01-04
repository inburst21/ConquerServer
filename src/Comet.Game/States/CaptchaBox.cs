// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - CaptchaBox.cs
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
    public sealed class CaptchaBox
    {
        private TimeOut m_Expiration = new TimeOut();
        private Character m_Owner;

        public CaptchaBox(Character owner)
        {
            m_Owner = owner;
        }

        public long Value1 { get; private set; }
        public long Value2 { get; private set; }
        public long Result { get; private set; }

        public Task OnAcceptAsync()
        {
            if (Value1 + Value2 != Result)
                return Kernel.RoleManager.KickOutAsync(m_Owner.Identity, "Wrong captcha reply");
            return Task.CompletedTask;
        }

        public Task OnCancelAsync()
        {
            if (Value1 + Value2 == Result)
                return Kernel.RoleManager.KickOutAsync(m_Owner.Identity, "Wrong captcha reply");
            return Task.CompletedTask;
        }

        public Task OnTimerAsync()
        {
            if (m_Expiration.IsActive() && m_Expiration.IsTimeOut())
                return Kernel.RoleManager.KickOutAsync(m_Owner.Identity, "No captcha reply");
            return Task.CompletedTask;
        }

        public async Task GenerateAsync()
        {
            Value1 = await Kernel.NextAsync(int.MaxValue) % 10;
            Value2 = await Kernel.NextAsync(int.MaxValue) % 10;
            if (await Kernel.ChanceCalcAsync(50, 100))
                Result = Value1 + Value2;
            else 
                Result = Value1 + Value2 + await Kernel.NextAsync(int.MaxValue) % 10;
            
            await m_Owner.SendAsync(new MsgTaskDialog
            {
                InteractionType = MsgTaskDialog.TaskInteraction.MessageBox,
                Text = string.Format(Language.StrBotCaptchaMessage, Value1, Value2, Result),
                OptionIndex = 255
            });
            m_Expiration.Startup(60);
        }
    }
}