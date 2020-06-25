// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgAction.cs
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
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Comet.Game.Database.Repositories;
using Comet.Game.States;
using Comet.Game.States.BaseEntities;
using Comet.Game.States.Items;
using Comet.Game.States.Relationship;
using Comet.Game.World.Maps;
using Comet.Network.Packets;
using Comet.Shared;

#endregion

namespace Comet.Game.Packets
{
    /// <remarks>Packet Type 1010</remarks>
    /// <summary>
    ///     Message containing a general action being performed by the client. Commonly used
    ///     as a request-response protocol for question and answer like exchanges. For example,
    ///     walk requests are responded to with an answer as to if the step is legal or not.
    /// </summary>
    public sealed class MsgAction : MsgBase<Client>
    {
        public MsgAction()
        {
            Type = PacketType.MsgAction;

            Timestamp = (uint) Environment.TickCount;
        }

        // Packet Properties
        public uint Timestamp { get; set; }
        public uint Identity { get; set; }
        public uint Command { get; set; }

        public ushort CommandX
        {
            get => (ushort) (Command - (CommandY << 16));
            set => Command = (uint) (CommandY << 16 | value);
        }

        public ushort CommandY
        {
            get => (ushort) (Command >> 16);
            set => Command = (uint) (value << 16) | Command;
        }

        public uint Argument { get; set; }

        public ushort ArgumentX
        {
            get => (ushort)(Argument - (ArgumentY << 16));
            set => Argument = (uint)(ArgumentY << 16 | value);
        }

        public ushort ArgumentY
        {
            get => (ushort)(Argument >> 16);
            set => Argument = (uint)(value << 16) | Argument;
        }
        public ushort Direction { get; set; }
        public ActionType Action { get; set; }

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
            Timestamp = reader.ReadUInt32();
            Identity = reader.ReadUInt32();
            Command = reader.ReadUInt32();
            Argument = reader.ReadUInt32();
            Direction = reader.ReadUInt16();
            Action = (ActionType) reader.ReadUInt16();
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
            writer.Write(Timestamp);
            writer.Write(Identity);
            writer.Write(Command);
            writer.Write(Argument);
            writer.Write(Direction);
            writer.Write((ushort) Action);
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
            Role target = null;
            Character user = client.Character;

            switch (Action)
            {
                case ActionType.LoginSpawn: // 74
                    Identity = client.Character.Identity;
                    Command = client.Character.MapIdentity;
                    ArgumentX = client.Character.MapX;
                    ArgumentY = client.Character.MapY;

                    await client.Character.EnterMap();
                    await client.SendAsync(this);

                    await GameAction.ExecuteActionAsync(1000000, user, null, null, "");
                    break;

                case ActionType.LoginInventory: // 75
                    await user.UserPackage.CreateAsync();
                    await user.UserPackage.SendAsync();
                    await user.Statistic.InitializeAsync();
                    await user.TaskDetail.InitializeAsync();

                    await client.SendAsync(this);
                    break;

                case ActionType.LoginRelationships: // 76
                    foreach (var dbFriend in await FriendRepository.GetAsync(user.Identity))
                    {
                        Friend friend = new Friend(user);
                        await friend.CreateAsync(dbFriend);
                        user.AddFriend(friend);
                    }
                    await user.SendAllFriendAsync();

                    foreach (var dbEnemy in await EnemyRepository.GetAsync(user.Identity))
                    {
                        Enemy enemy = new Enemy(user);
                        await enemy.CreateAsync(dbEnemy);
                        user.AddEnemy(enemy);
                    }
                    await user.SendAllEnemiesAsync();

                    await client.SendAsync(this);
                    break;

                case ActionType.LoginProficiencies: // 77
                    await client.Character.WeaponSkill.InitializeAsync();
                    await client.Character.WeaponSkill.SendAsync();
                    await client.SendAsync(this);
                    break;

                case ActionType.LoginSpells: // 78
                    await client.Character.MagicData.InitializeAsync();
                    await client.Character.MagicData.SendAllAsync();
                    await client.SendAsync(this);
                    break;

                case ActionType.CharacterDirection: // 79
                    await client.Character.SetDirectionAsync((FacingDirection) (Direction % 8), false);
                    await client.SendAsync(this);
                    break;

                case ActionType.CharacterEmote: // 81
                    await client.Character.SetActionAsync((EntityAction) Command, false);

                    if (user.Action == EntityAction.Cool)
                    {
                        int effect = 0;
                        for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin;
                            pos <= Item.ItemPosition.EquipmentEnd;
                            pos++)
                        {
                            Item item = user.UserPackage[pos];
                            if (item != null && 
                                (item.IsHelmet() || item.IsNeck() || item.IsRing() || item.IsWeapon() || item.IsArmor() || item.IsShield() || item.IsShoes()))
                            {
                                effect += item.GetQuality()%5;
                                if (item.IsBackswordType() || item.GetItemSort() == Item.ItemSort.ItemsortWeaponDoubleHand)
                                    effect += item.GetQuality() % 5;
                            }
                        }

                        if (effect == 7*4)
                        {
                            Command |= ((uint)user.Profession * 0x10000 + 0x01000000);
                        }
                        else if (effect >= 7*3)
                        {
                            Command |= ((uint)user.Profession * 0x10000);
                        }
                    }

                    await client.SendAsync(this);
                    break;

                case ActionType.MapPortal: // 85
                    uint idMap = 0;
                    Point tgtPos = new Point();
                    Point sourcePos = new Point(client.Character.MapX, client.Character.MapY);
                    if (!client.Character.Map.GetPassageMap(ref idMap, ref tgtPos, ref sourcePos))
                    {
                        client.Character.Map.GetRebornMap(ref idMap, ref tgtPos);
                    }
                    await client.Character.FlyMap(idMap, tgtPos.X, tgtPos.Y);
                    break;

                case ActionType.SpellAbortXp: // 93
                    if (client.Character.QueryStatus(StatusSet.START_XP) != null)
                        await client.Character.DetachStatus(StatusSet.START_XP);
                    break;

                case ActionType.CharacterRevive: // 94
                    if (user.IsAlive || !user.CanRevive())
                        return;

                    await user.Reborn(Command == 0);
                    break;

                case ActionType.CharacterPkMode: // 95
                    if (!Enum.IsDefined(typeof(PkModeType), (int) Command))
                        Command = (uint) PkModeType.Capture;

                    client.Character.PkMode = (PkModeType) Command;
                    await client.SendAsync(this);
                    break;

                case ActionType.LoginGuild: // 97
                    client.Character.Syndicate = Kernel.SyndicateManager.FindByUser(client.Identity);
                    await client.Character.SendSyndicateAsync();
                    if (client.Character.Syndicate != null)
                        await client.Character.Syndicate.SendRelationAsync(client.Character);
                    await client.SendAsync(this);
                    break;

                case ActionType.MapQuery: // 102
                    Character targetUser = Kernel.RoleManager.GetUser(Command);
                    if (targetUser != null)
                        await targetUser.SendSpawnToAsync(user);
                    break;

                case ActionType.BoothLeave: // 114
                    await user.Screen.SynchroScreenAsync();
                    break;

                case ActionType.CharacterObservation: // 117
                    targetUser = Kernel.RoleManager.GetUser(Command);
                    if (targetUser == null)
                        return;

                    for (Item.ItemPosition pos = Item.ItemPosition.EquipmentBegin;
                        pos <= Item.ItemPosition.EquipmentEnd;
                        pos++)
                    {
                        if (targetUser.UserPackage[pos] != null)
                        {
                            await user.SendAsync(new MsgItemInfo(targetUser.UserPackage[pos], MsgItemInfo.ItemMode.View));
                        }
                    }

                    await targetUser.SendAsync(string.Format(Language.StrObservingEquipment, user.Name));

                    await client.SendAsync(new MsgName
                    {
                        Identity = targetUser.Identity,
                        Action = StringAction.QueryMate,
                        Strings = new List<string>
                        {
                            targetUser.MateName
                        }
                    });
                    break;

                case ActionType.SpellAbortTransform: // 118
                    if (user.Transformation != null)
                        await user.ClearTransformation();
                    break;

                case ActionType.SpellAbortFlight:
                    if (user.QueryStatus(StatusSet.FLY) != null)
                        await user.DetachStatus(StatusSet.FLY);
                    break;

                case ActionType.RelationshipsEnemy: // 123
                    Enemy fetchEnemy = user.GetEnemy(Command);
                    if (fetchEnemy == null)
                    {
                        await user.SendAsync(this);
                        return;
                    }

                    await fetchEnemy.SendInfoAsync();
                    break;

                case ActionType.LoginComplete: // 130
                    await client.Character.SendMultipleExp();
                    await client.Character.SendBless();
                    await client.Character.SendNobilityInfo();
                    await user.Screen.SynchroScreenAsync();
                    await client.SendAsync(this);
                    break;

                case ActionType.MapJump: // 133
                    if (user != null) // todo handle ai 
                    {
                        if (!user.IsAlive)
                        {
                            await user.SendAsync(Language.StrDead, MsgTalk.TalkChannel.System, Color.Red);
                            return;
                        }

                        ushort newX = (ushort) Command;
                        ushort newY = (ushort) (Command >> 16);

                        if (user.GetDistance(newX, newY) >= 2 * Screen.VIEW_SIZE)
                        {
                            await user.SendAsync(Language.StrInvalidMsg, MsgTalk.TalkChannel.System, Color.Red);
                            await Kernel.RoleManager.KickoutAsync(user.Identity, "big jump");
                            return;
                        }

                        await user.ProcessOnMove();
                        await user.JumpPosAsync(newX, newY);
                        await user.SendAsync(this);
                        await user.Screen.UpdateAsync(this);
                    }

                    break;

                case ActionType.RelationshipsFriend: // 140
                    Friend fetchFriend = user.GetFriend(Command);
                    if (fetchFriend == null)
                    {
                        await user.SendAsync(this);
                        return;
                    }
                    await fetchFriend.SendInfoAsync();
                    break;

                case ActionType.CharacterDead: // 137
                    if (user.IsAlive)
                        return;

                    await user.SetGhost();
                    break;

                default:
                    await client.SendAsync(this);
                    if (client.Character.IsPm())
                    {
                        await client.SendAsync(new MsgTalk(client.Identity, MsgTalk.TalkChannel.Service,
                            $"Missing packet {Type}, Action {Action}, Length {Length}"));
                    }

                    await Log.WriteLog(LogLevel.Warning,
                        "Missing packet {0}, Action {1}, Length {2}\n{3}",
                        Type, Action, Length, PacketDump.Hex(Encode()));
                    break;
            }
        }

        /// <summary>
        ///     Defines actions that may be requested by the user, or given to by the server.
        ///     Allows for action handling as a packet subtype. Enums should be named by the
        ///     action they provide to a system in the context of the player actor.
        /// </summary>
        public enum ActionType
        {
            LoginSpawn = 74,
            LoginInventory,
            LoginRelationships,
            LoginProficiencies,
            LoginSpells,
            CharacterDirection,
            CharacterEmote = 81,
            MapPortal = 85,
            MapTeleport,
            CharacterLevelUp = 92,
            SpellAbortXp,
            CharacterRevive,
            CharacterDelete,
            CharacterPkMode,
            LoginGuild,
            MapMine = 99,
            MapTeamLeaderStar = 101,
            MapQuery,
            AbortMagic = 103,
            MapArgb = 104,
            MapTeamMemberStar = 106,
            Kickback = 108,
            SpellRemove,
            ProficiencyRemove,
            BoothSpawn,
            BoothSuspend,
            BoothResume,
            BoothLeave,
            ClientCommand = 116,
            CharacterObservation,
            SpellAbortTransform,
            SpellAbortFlight = 120,
            MapGold,
            RelationshipsEnemy = 123,
            ClientDialog = 126,
            LoginComplete = 130,
            MapEffect,
            RemoveEntity,
            MapJump,
            CharacterDead = 137,
            RelationshipsFriend = 140,
            CharacterAvatar = 142,
            SetGhost = 145,
        }
    }
}