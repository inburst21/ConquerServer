// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - DetainEquipment.cs
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

using Comet.Game.States.Items;

#endregion

namespace Comet.Game.States
{
    public static class DetainEquipment
    {
        public const uint DETAIN_TIME = 60 * 60 * 24 * 7;

        public static uint GetDetainPrice(Item item)
        {
            uint dwPrice = 100;

            if (item.GetQuality() == 9) // if super +500CPs
                dwPrice += 500;

            switch (item.Plus) // (+n)
            {
                case 1:
                    dwPrice += 10;
                    break;
                case 2:
                    dwPrice += 20;
                    break;
                case 3:
                    dwPrice += 50;
                    break;
                case 4:
                    dwPrice += 150;
                    break;
                case 5:
                    dwPrice += 300;
                    break;
                case 6:
                    dwPrice += 900;
                    break;
                case 7:
                    dwPrice += 2700;
                    break;
                case 8:
                    dwPrice += 6000;
                    break;
                case 9:
                case 10:
                case 11:
                case 12:
                    dwPrice += 12000;
                    break;
            }

            if (item.IsWeapon()) // if weapon
            {
                if (item.SocketTwo > Item.SocketGem.NoSocket)
                    dwPrice += 1000;
                else if (item.SocketOne > Item.SocketGem.NoSocket)
                    dwPrice += 100;
            }
            else // if not
            {
                if (item.SocketTwo > Item.SocketGem.NoSocket)
                    dwPrice += 1500;
                else if (item.SocketOne > Item.SocketGem.NoSocket)
                    dwPrice += 5000;
            }

            return dwPrice;
        }
    }
}