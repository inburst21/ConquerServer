// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgItem.cs
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
using System.Drawing;
using System.Threading.Tasks;
using Comet.Game.States;
using Comet.Game.States.Items;
using Comet.Network.Packets;

#endregion

namespace Comet.Game.Packets
{
    /// <remarks>Packet Type 1009</remarks>
    /// <summary>
    ///     Message containing an item action command. Item actions are usually performed to
    ///     manage player equipment, inventory, money, or item shop purchases and sales. It
    ///     is serves a second purpose for measuring client ping.
    /// </summary>
    public sealed class MsgItem : MsgBase<Client>
    {
        public MsgItem()
        {
            Type = PacketType.MsgItem;
        }

        public MsgItem(uint identity, ItemActionType action, uint cmd = 0, uint param = 0)
        {
            Type = PacketType.MsgItem;

            Identity = identity;
            Command = cmd;
            Action = action;
            Timestamp = (uint)Environment.TickCount;
            Argument = param;
        }

        // Packet Properties
        public uint Identity { get; set; }
        public uint Command { get; set; }
        public uint Timestamp { get; set; }
        public uint Argument { get; set; }
        public ItemActionType Action { get; set; }

        /// <summary>
        ///     Decodes a byte packet into the packet structure defined by this message class.
        ///     Should be invoked to structure data from the client for processing. Decoding
        ///     follows TQ Digital's byte ordering rules for an all-binary protocol.
        /// </summary>
        /// <param name="bytes">Bytes from the packet processor or client socket</param>
        public override void Decode(byte[] bytes)
        {
            var reader = new PacketReader(bytes);
            Length = reader.ReadUInt16();
            Type = (PacketType) reader.ReadUInt16();
            Identity = reader.ReadUInt32();
            Command = reader.ReadUInt32();
            Action = (ItemActionType) reader.ReadUInt32();
            Timestamp = reader.ReadUInt32();
            Argument = reader.ReadUInt32();
        }

        /// <summary>
        ///     Encodes the packet structure defined by this message class into a byte packet
        ///     that can be sent to the client. Invoked automatically by the client's send
        ///     method. Encodes using byte ordering rules interoperable with the game client.
        /// </summary>
        /// <returns>Returns a byte packet of the encoded packet.</returns>
        public override byte[] Encode()
        {
            var writer = new PacketWriter();
            writer.Write((ushort) Type);
            writer.Write(Identity);
            writer.Write(Command);
            writer.Write((uint) Action);
            writer.Write(Timestamp);
            writer.Write(Argument);
            return writer.ToArray();
        }

        /// <summary>
        ///     Process can be invoked by a packet after decode has been called to structure
        ///     packet fields and properties. For the server implementations, this is called
        ///     in the packet handler after the message has been dequeued from the server's
        ///     <see cref="PacketProcessor{TClient}" />.
        /// </summary>
        /// <param name="client">Client requesting packet processing</param>
        public override async Task ProcessAsync(Client client)
        {
            Character user = client.Character;

            switch (Action)
            {
                case ItemActionType.InventoryRemove:
                    await user.DropItem(Identity, user.MapX, user.MapY);
                    break;

                case ItemActionType.InventoryDropSilver:
                    await user.DropSilver(Identity);
                    break;

                case ItemActionType.InventoryEquip:
                case ItemActionType.EquipmentWear:
                    if (!await user.UserPackage.UseItemAsync(Identity, (Item.ItemPosition) Command))
                        await user.SendAsync(Language.StrUnableToUseItem, MsgTalk.TalkChannel.TopLeft, Color.Red);
                    break;

                case ItemActionType.EquipmentRemove:
                    if (!await user.UserPackage.UnequipAsync((Item.ItemPosition)Command))
                        await user.SendAsync(Language.StrYourBagIsFull, MsgTalk.TalkChannel.TopLeft, Color.Red);
                    break;

                case ItemActionType.ClientPing:
                    await client.SendAsync(this);
                    break;

                default:
                    await client.SendAsync(this);
                    if (client.Character.IsGm())
                        await client.SendAsync(new MsgTalk(client.Identity, MsgTalk.TalkChannel.Service,
                            $"Missing packet {Type}, Action {Action}, Length {Length}"));
                    Console.WriteLine("Missing packet {0}, action {1}, Length {2}\n{3}",
                        Type, Action, Length, PacketDump.Hex(Encode()));
                    break;
            }
        }

        /// <summary>
        ///     Enumeration type for defining item actions that may be requested by the user,
        ///     or given to by the server. Allows for action handling as a packet subtype.
        ///     Enums should be named by the action they provide to a system in the context
        ///     of the player item.
        /// </summary>
        public enum ItemActionType
        {
            ShopPurchase = 1,
            ShopSell,
            InventoryRemove,
            InventoryEquip,
            EquipmentWear,
            EquipmentRemove,
            EquipmentSplit,
            EquipmentCombine,
            BankQuery,
            BankDeposit,
            BankWithdraw,
            InventoryDropSilver,
            EquipmentRepair = 14,
            EquipmentRepairAll,
            EquipmentImprove = 19,
            EquipmentLevelUp,
            BoothQuery,
            BoothSell,
            BoothRemove,
            BoothPurchase,
            EquipmentAmount,
            ClientPing = 27,
            EquipmentEnchant,
            BoothSellPoints
        }
    }
}