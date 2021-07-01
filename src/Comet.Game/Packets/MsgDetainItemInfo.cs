using Comet.Game.Database.Models;
using Comet.Game.States;
using Comet.Game.States.Items;
using Comet.Network.Packets;
using Comet.Shared;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Comet.Game.Packets
{
    public sealed class MsgDetainItemInfo : MsgBase<Client>
    {
        public const int MAX_REDEEM_DAYS = 7;
        public const int MAX_REDEEM_SECONDS = 60 * 60 * 24 * MAX_REDEEM_DAYS;

        public MsgDetainItemInfo(DbDetainedItem dbDetainItem, Item item, Mode mode)
        {
            Identity = dbDetainItem.Identity;
            ItemIdentity = item?.Identity ?? 1;
            ItemType = item?.Type ?? 410339;
            Amount = item?.Durability ?? 0;
            AmountLimit = item?.MaximumDurability ?? 0;
            Action = mode;
            SocketProgress = item?.SocketProgress ?? 0;
            SocketOne = item?.SocketOne ?? Item.SocketGem.NoSocket;
            SocketTwo = item?.SocketTwo ?? Item.SocketGem.NoSocket;
            Effect = item?.Effect ?? Item.ItemEffect.None;
            Addition = item?.Plus ?? 0;
            Blessing = (byte)(item?.Blessing ?? 0);
            Bound = item?.IsBound ?? false;
            Enchantment = item?.Enchantment ?? 0;
            Suspicious = item?.IsSuspicious() ?? false;
            Locked = item?.IsLocked() ?? false;
            Color = item?.Color ?? Item.ItemColor.Orange;
            if (Action == Mode.ClaimPage)
            {
                OwnerIdentity = dbDetainItem.HunterIdentity;
                OwnerName = dbDetainItem.HunterName;
                TargetIdentity = dbDetainItem.TargetIdentity;
                TargetName = dbDetainItem.TargetName;
            }
            else if (Action == Mode.DetainPage)
            {
                OwnerIdentity = dbDetainItem.TargetIdentity;
                OwnerName = dbDetainItem.TargetName;
                TargetIdentity = dbDetainItem.HunterIdentity;
                TargetName = dbDetainItem.HunterName;
            }            
            DetainDate = int.Parse(UnixTimestamp.ToDateTime(dbDetainItem.HuntTime).ToString("yyyyMMdd"));
            Expired = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")) - long.Parse(UnixTimestamp.ToDateTime(dbDetainItem.HuntTime).ToString("yyyyMMddHHmmss")) > MAX_REDEEM_DAYS * 1000000;
            Cost = dbDetainItem.RedeemPrice;
            RemainingDays = Math.Max(0, int.Parse(DateTime.Now.ToString("yyyyMMdd")) - int.Parse(UnixTimestamp.ToDateTime(dbDetainItem.HuntTime).ToString("yyyyMMdd")));
            }

        public uint Identity { get; set; }
        public uint ItemIdentity { get; set; }
        public uint ItemType { get; set; }
        public ushort Amount { get; set; }
        public ushort AmountLimit { get; set; }
        public Mode Action { get; set; }
        public uint SocketProgress { get; set; }
        public Item.SocketGem SocketOne { get; set; }
        public Item.SocketGem SocketTwo { get; set; }
        public Item.ItemEffect Effect { get; set; }
        public byte Addition { get; set; }
        public byte Blessing { get; set; }
        public bool Bound { get; set; }
        public byte Enchantment { get; set; }
        public bool Suspicious { get; set; }
        public bool Locked { get; set; }
        public Item.ItemColor Color { get; set; }
        public uint OwnerIdentity { get; set; }
        public string OwnerName { get; set; }
        public uint TargetIdentity { get; set; }
        public string TargetName { get; set; }
        public int DetainDate { get; set; }
        public bool Expired { get; set; }
        public int Cost { get; set; }
        public int RemainingDays { get; set; }

        public override void Decode(byte[] bytes)
        {
            PacketReader reader = new (bytes);
            Length = reader.ReadUInt16();
            Type = (PacketType) reader.ReadUInt16();
            Identity = reader.ReadUInt32();
            ItemIdentity = reader.ReadUInt32();
            ItemType = reader.ReadUInt32();
            Amount = reader.ReadUInt16();
            AmountLimit = reader.ReadUInt16();
            Action = (Mode) reader.ReadInt32();
            SocketProgress = reader.ReadUInt32();
            SocketOne = (Item.SocketGem)reader.ReadByte();
            SocketTwo = (Item.SocketGem)reader.ReadByte();
            Effect = (Item.ItemEffect) reader.ReadInt16();
            Addition = reader.ReadByte();
            Blessing = reader.ReadByte();
            Bound = reader.ReadBoolean();
            Enchantment = reader.ReadByte();
            Suspicious = reader.ReadBoolean();
            Locked = reader.ReadBoolean();
            reader.ReadByte();
            Color = (Item.ItemColor)reader.ReadByte();
            reader.ReadBytes(11);
            OwnerIdentity = reader.ReadUInt32();
            OwnerName = reader.ReadString(16);
            TargetIdentity = reader.ReadUInt32();
            TargetName = reader.ReadString(16);
            Cost = reader.ReadInt32();
            Expired = reader.ReadInt32() != 0;
            DetainDate = reader.ReadInt32();
            RemainingDays = reader.ReadInt32();
        }

        public override byte[] Encode()
        {
            PacketWriter writer = new ();
            writer.Write((ushort) PacketType.MsgDetainItemInfo);
            writer.Write(Identity);
            writer.Write(ItemIdentity);
            writer.Write(ItemType);
            writer.Write(Amount);
            writer.Write(AmountLimit);
            writer.Write((int)Action);
            writer.Write(SocketProgress);
            writer.Write((byte)SocketOne);
            writer.Write((byte)SocketTwo);
            writer.Write(new byte[5]);
            writer.Write((byte)Effect);
            writer.Write((byte)0);
            writer.Write(Addition);
            writer.Write(Blessing);
            writer.Write(Bound);
            writer.Write(Enchantment);
            writer.Write(Suspicious);
            writer.Write(Locked);
            writer.Write((byte)0);
            writer.Write((byte)Color);
            writer.Write(new byte[11]);
            writer.Write(OwnerIdentity);
            writer.Write(OwnerName, 16);
            writer.Write(TargetIdentity);
            writer.Write(TargetName, 16);
            writer.Write(Cost);
            writer.Write(Expired ? 1 : 0);
            writer.Write(DetainDate);
            writer.Write(RemainingDays);
            return writer.ToArray();
        }

        public override Task ProcessAsync(Client client)
        {
            return base.ProcessAsync(client);
        }

        public enum Mode
        {
            DetainPage,
            ClaimPage,
            ReadyToClaim
        }
    }
}
