﻿using Comet.Account.Database;
using Comet.Account.Database.Models;
using Comet.Account.States;
using Comet.Network.Packets.Internal;
using Comet.Shared;
using System.Linq;
using System.Threading.Tasks;

namespace Comet.Account.Packets
{
    public sealed class MsgAccServerPlayerExchange : MsgAccServerPlayerExchange<GameServer>
    {
        public override async Task ProcessAsync(GameServer client)
        {
            DbRealm realm = Kernel.Realms.Values.FirstOrDefault(x => x.Name.Equals(ServerName));
            if (realm == null)
            {
                await Log.WriteLogAsync(LogLevel.Warning, $"Invalid server name [{ServerName}] tried to update data from [{client.IPAddress}].");
                return;
            }

            if (realm.Server == null)
            {
                await Log.WriteLogAsync(LogLevel.Warning, $"{ServerName} is not connected and tried to update player status from [{client.IPAddress}].");
                return;
            }

            foreach (var info in Data)
            {
                DbRecordUser user = await DbRecordUser.GetByIdAsync(info.Identity, realm.RealmID);
                if (user == null)
                    user = new DbRecordUser();

                user.Name = info.Name;
                user.MateId = 0; // 
                user.Level = info.Level;
                user.Experience = 0;
                user.Profession = (byte)info.Profession;
                user.NewProfession = (byte)info.PreviousProfession;
                user.OldProfession = (byte)info.FirstProfession;
                user.Metempsychosis = info.Metempsychosis;
                user.Strength = info.Force;
                user.Agility = info.Speed;
                user.Vitality = info.Health;
                user.Spirit = info.Soul;
                user.AdditionalPoints = info.AdditionPoints;
                user.SyndicateIdentity = info.SyndicateIdentity;
                user.SyndicatePosition = info.SyndicatePosition;
                user.FamilyIdentity = info.FamilyIdentity;
                user.FamilyRank = info.FamilyPosition;
                user.NobilityDonation = info.Donation;
                user.NobilityRank = 0;
                user.SupermanCount = 0;
                user.Money = info.Money;
                user.ConquerPoints = info.ConquerPoints;
                //user.ConquerPointsMono = info.ConquerPointsMono;
                user.WarehouseMoney = 0;
                user.DeletedAt = null;

                await BaseRepository.SaveAsync(user);
            }
        }
    }
}
