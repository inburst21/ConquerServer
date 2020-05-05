// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - MsgRegister.cs
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
using System.Threading.Tasks;
using Comet.Game.Database.Models;
using Comet.Game.Database.Repositories;
using Comet.Game.States;
using Comet.Network.Packets;

#endregion

namespace Comet.Game.Packets
{
    #region References

    using static MsgTalk;

    #endregion

    /// <remarks>Packet Type 1001</remarks>
    /// <summary>
    ///     Message containing character creation details, such as the new character's name,
    ///     body size, and profession. The character name should be verified, and may be
    ///     rejected by the server if a character by that name already exists.
    /// </summary>
    public sealed class MsgRegister : MsgBase<Client>
    {
        // Registration constants
        private static readonly byte[] Hairstyles =
        {
            10, 11, 13, 14, 15, 24, 30, 35, 37, 38, 39, 40
        };

        // Packet Properties
        public string Username { get; set; }
        public string CharacterName { get; set; }
        public string MaskedPassword { get; set; }
        public ushort Mesh { get; set; }
        public ushort Class { get; set; }
        public uint Token { get; set; }

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
            Username = reader.ReadString(16);
            CharacterName = reader.ReadString(16);
            MaskedPassword = reader.ReadString(16);
            Mesh = reader.ReadUInt16();
            Class = reader.ReadUInt16();
            Token = reader.ReadUInt32();
        }

        /// <summary>
        ///     Process can be invoked by a packet after decode has been called to structure
        ///     packet fields and properties. For the server implementations, this is called
        ///     in the packet handler after the message has been dequeued from the server's
        ///     <see cref="PacketProcessor" />.
        /// </summary>
        /// <param name="client">Client requesting packet processing</param>
        public override async Task ProcessAsync(Client client)
        {
            // Validate that the player has access to character creation
            if (client.Creation == null || Token != client.Creation.Token ||
                !Kernel.Registration.Contains(Token))
            {
                await client.SendAsync(RegisterInvalid);
                client.Disconnect();
                return;
            }

            // Check character name availability
            if (await CharactersRepository.ExistsAsync(CharacterName))
            {
                await client.SendAsync(RegisterNameTaken);
                return;
            }

            // Validate character creation input
            if (!Enum.IsDefined(typeof(BodyType), Mesh) ||
                !Enum.IsDefined(typeof(BaseClassType), Class))
            {
                await client.SendAsync(RegisterInvalid);
                return;
            }

            // Create the character
            var character = new DbCharacter();
            character.AccountID = client.Creation.AccountID;
            character.Name = CharacterName;
            character.CurrentClass = (byte) Class;
            character.Mesh = Mesh;
            character.Silver = 1000;
            character.Level = 1;
            character.MapID = 1010;
            character.X = 61;
            character.Y = 109;
            character.Strength = 4;
            character.Agility = 6;
            character.Vitality = 12;
            character.Spirit = 0;
            character.HealthPoints =
                (ushort) (character.Strength * 3
                          + character.Agility * 3
                          + character.Spirit * 3
                          + character.Vitality * 24);
            character.ManaPoints = (ushort) (character.Spirit * 5);
            character.Registered = DateTime.Now;

            // Generate a random look for the character
            character.Avatar = (ushort) (character.Mesh < 1005
                ? await Kernel.NextAsync(1, 49)
                : await Kernel.NextAsync(201, 249));
            character.Hairstyle = (ushort) (
                await Kernel.NextAsync(3, 9) * 100 + Hairstyles[
                    await Kernel.NextAsync(0, Hairstyles.Length)]);

            try
            {
                // Save the character and continue with login
                await CharactersRepository.CreateAsync(character);
                Kernel.Registration.Remove(client.Creation.Token);
                await client.SendAsync(RegisterOk);
            }
            catch
            {
                await client.SendAsync(RegisterTryAgain);
            }
        }
    }
}