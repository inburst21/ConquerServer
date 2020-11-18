// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgDataArray.cs
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

#define OLD_COMPOSE

#region References

using System.Collections.Generic;
using System.Threading.Tasks;
using Comet.Game.States;
using Comet.Game.States.Items;
using Comet.Network.Packets;
using Comet.Shared;

#endregion

namespace Comet.Game.Packets
{
    public sealed class MsgDataArray : MsgBase<Client>
    {
        public enum DataArrayMode : byte
        {
            Composition = 0
        }

        public MsgDataArray()
        {
            Type = PacketType.MsgDataArray;
        }

        public DataArrayMode Action { get; set; }
        public byte Count { get; set; }

        public List<uint> Items = new List<uint>();

        public override void Decode(byte[] bytes)
        {
            PacketReader reader = new PacketReader(bytes);
            Length = reader.ReadUInt16();
            Type = (PacketType)reader.ReadUInt16();
            Action = (DataArrayMode) reader.ReadByte();
            Count = reader.ReadByte();
            reader.ReadUInt16();
            for (int i = 0; i < Count; i++)
            {
                Items.Add(reader.ReadUInt32());
            }
        }

        public override byte[] Encode()
        {
            PacketWriter writer = new PacketWriter();
            writer.Write((ushort) Type);
            writer.Write((byte) Action);
            writer.Write((byte) Items.Count);
            writer.Write((ushort) 0);
            foreach (var item in Items)
            {
                writer.Write(item);
            }
            return writer.ToArray();
        }

        public override async Task ProcessAsync(Client client)
        {
            Character user = client.Character;

            switch (Action)
            {
                case DataArrayMode.Composition:
#if OLD_COMPOSE
                    if (Items.Count < 5 || (Items[1] == 0 && Items[2] == 0))
                        return;
                    
                    Item composeTarget = user.UserPackage[Items[0]];
                    Item composeMinor1 = user.UserPackage[Items[1]];
                    Item composeMinor2 = user.UserPackage[Items[2]];

                    if (composeTarget == null)
                        return;

                    byte oldComposition = composeTarget.Plus;

                    if (composeMinor1 == null || composeMinor2 == null) // need 2 minor items
                        return;

                    if (composeMinor1.GetItemSubType() != 730)
                    {
                        if (composeTarget.GetItemSort() == Item.ItemSort.ItemsortWeaponSingleHand
                            || composeTarget.GetItemSort() == Item.ItemSort.ItemsortWeaponDoubleHand)
                        {
                            if (composeTarget.IsBow() && !composeMinor1.IsBow())
                                return;

                            if (composeTarget.GetItemSort() != composeMinor1.GetItemSort())
                                return;
                        }
                        else
                        {
                            if (composeTarget.GetItemSubType() != composeMinor1.GetItemSubType())
                                return;
                        }
                    }

                    if (composeMinor2.GetItemSubType() != 730)
                    {
                        if (composeTarget.GetItemSort() == Item.ItemSort.ItemsortWeaponSingleHand
                            || composeTarget.GetItemSort() == Item.ItemSort.ItemsortWeaponDoubleHand)
                        {
                            if (composeTarget.IsBow() && !composeMinor2.IsBow())
                                return;

                            if (composeTarget.GetItemSort() != composeMinor2.GetItemSort())
                                return;
                        }
                        else
                        {
                            if (composeTarget.GetItemSubType() != composeMinor2.GetItemSubType())
                                return;
                        }
                    }

                    if (composeMinor1.Plus < composeTarget.Plus
                        || composeMinor2.Plus < composeTarget.Plus)
                        return;

                    Item composeGem1 = null;
                    Item composeGem2 = null;
                    bool useGems = false;

                    if (composeTarget.Plus >= 5)
                    {
                        if (Items[3] == 0 || Items[4] == 0)
                            return;

                        composeGem1 = user.UserPackage[Items[3]];
                        composeGem2 = user.UserPackage[Items[4]];

                        if (composeGem1 == null || composeGem2 == null) // need 2 gems if going to >+6
                            return;

                        if (composeGem1.GetItemSubType() != 700 || composeGem2.GetItemSubType() != 700) // must be gems
                            return;

                        useGems = true;
                    }

                    await user.UserPackage.SpendItemAsync(composeMinor1);
                    await user.UserPackage.SpendItemAsync(composeMinor2);
                    if (useGems)
                    {
                        await user.UserPackage.SpendItemAsync(composeGem1);
                        await user.UserPackage.SpendItemAsync(composeGem2);
                    }

                    composeTarget.ChangeAddition();

                    await user.SendAsync(new MsgItemInfo(composeTarget, MsgItemInfo.ItemMode.Update));
                    await composeTarget.SaveAsync();

                    if (oldComposition < composeTarget.Plus && composeTarget.Plus >= 6)
                    {
                        if (user.Gender == 1)
                            await user.SendAsync(string.Format(Language.StrComposeOverpowerMale, user.Name, composeTarget.Name, composeTarget.Plus));
                        else
                            await user.SendAsync(string.Format(Language.StrComposeOverpowerFemale, user.Name, composeTarget.Name, composeTarget.Plus));
                    }

#endif
                    break;
                default:
                    await Log.WriteLogAsync(LogLevel.Error, $"Invalid MsgDataArray Action: {Action}." +
                                                       $"{user.Identity},{user.Name},{user.Level},{user.MapIdentity}[{user.Map.Name}],{user.MapX},{user.MapY}");
                    return;
            }
        }
    }
}