// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) FTW! Masters
// Keep the headers and the patterns adopted by the project. If you changed anything in the file just insert
// your name below, but don't remove the names of who worked here before.
// 
// This project is a fork from Comet, a Conquer Online Server Emulator created by Spirited, which can be
// found here: https://gitlab.com/spirited/comet
// 
// Comet - Comet.Game - Base Npc.cs
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
using System.Threading.Tasks;
using Comet.Game.Database.Models;
using Comet.Game.Database.Repositories;
using Comet.Game.States.BaseEntities;
using Comet.Game.World.Maps;

#endregion

namespace Comet.Game.States.NPCs
{
    public abstract class BaseNpc : Role
    {
        #region Constants

        public const int
            NPC_NONE = 0, // Í¨ÓÃNPC
            SHOPKEEPER_NPC = 1, // ÉÌµêNPC
            TASK_NPC = 2, // ÈÎÎñNPC(ÒÑ×÷·Ï£¬½öÓÃÓÚ¼æÈÝ¾ÉÊý¾Ý)
            STORAGE_NPC = 3, // ¼Ä´æ´¦NPC
            TRUNCK_NPC = 4, // Ïä×ÓNPC
            FACE_NPC = 5, // ±äÍ·ÏñNPC
            FORGE_NPC = 6, // ¶ÍÔìNPC		(only use for client)
            EMBED_NPC = 7, // ÏâÇ¶NPC
            STATUARY_NPC = 9, // µñÏñNPC
            SYNFLAG_NPC = 10, // °ïÅÉ±ê¼ÇNPC
            ROLE_PLAYER = 11, // ÆäËûÍæ¼Ò		(only use for client)
            ROLE_HERO = 12, // ×Ô¼º			(only use for client)
            ROLE_MONSTER = 13, // ¹ÖÎï			(only use for client)
            BOOTH_NPC = 14, // °ÚÌ¯NPC		(CBooth class)
            SYNTRANS_NPC = 15, // °ïÅÉ´«ËÍNPC, ¹Ì¶¨ÄÇ¸ö²»ÒªÓÃ´ËÀàÐÍ! (ÓÃÓÚ00:00ÊÕ·Ñ)(LINKIDÎª¹Ì¶¨NPCµÄID£¬ÓëÆäËüÊ¹ÓÃLINKIDµÄ»¥³â)
            ROLE_BOOTH_FLAG_NPC = 16, // Ì¯Î»±êÖ¾NPC	(only use for client)
            ROLE_MOUSE_NPC = 17, // Êó±êÉÏµÄNPC	(only use for client)
            ROLE_MAGICITEM = 18, // ÏÝÚå»ðÇ½		(only use for client)
            ROLE_DICE_NPC = 19, // ÷»×ÓNPC
            ROLE_SHELF_NPC = 20, // ÎïÆ·¼Ü
            WEAPONGOAL_NPC = 21, // ÎäÆ÷°Ð×ÓNPC
            MAGICGOAL_NPC = 22, // Ä§·¨°Ð×ÓNPC
            BOWGOAL_NPC = 23, // ¹­¼ý°Ð×ÓNPC
            ROLE_TARGET_NPC = 24, // °¤´ò£¬²»´¥·¢ÈÎÎñ	(only use for client)
            ROLE_FURNITURE_NPC = 25, // ¼Ò¾ßNPC	(only use for client)
            ROLE_CITY_GATE_NPC = 26, // ³ÇÃÅNPC	(only use for client)
            ROLE_NEIGHBOR_DOOR = 27, // ÁÚ¾ÓµÄÃÅ
            ROLE_CALL_PET = 28, // ÕÙ»½ÊÞ	(only use for client)
            EUDEMON_TRAINPLACE_NPC = 29, // »ÃÊÞÑ±ÑøËù
            AUCTION_NPC = 30, // ÅÄÂòNPC	ÎïÆ·ÁìÈ¡NPC  LW
            ROLE_MINE_NPC = 31, // ¿óÊ¯NPC		
            ROLE_CTFBASE_NPC = 40,
            ROLE_3DFURNITURE_NPC = 101, // 3D¼Ò¾ßNPC 
            SYN_NPC_WARLETTER = 110; //Ôö¼ÓÐÂµÄ£Î£Ð£ÃÀàÐÍ¡¡×¨ÃÅÓÃÀ´¡¡ÏÂÕ½ÊéµÄ¡¡°ïÅÉ£Î£Ð£Ã

        #endregion

        protected BaseNpc(uint idNpc)
        {
            Identity = idNpc;
        }

        public virtual async Task<bool> InitializeAsync()
        {
            if (IsShopNpc())
            {
                ShopGoods = await GoodsRepository.GetAsync(Identity);
            }

            await EnterMap();
            return true;
        }

        #region Type Identity
        
        public virtual ushort Type { get; }

        public virtual uint OwnerType { get; set; }

        public virtual ushort Sort { get; }

        #endregion

        #region Position

        public async Task<bool> ChangePosAsync(uint idMap, ushort x, ushort y)
        {
            GameMap map = Kernel.MapManager.GetMap(idMap);
            if (map != null)
            {
                if (!map.IsStandEnable(x, y))
                    return false;

                await LeaveMap();
                m_idMap = idMap;
                m_posX = x;
                m_posY = y;
                await EnterMap();
                return true;
            }
            return false;
        }

        #endregion

        #region Map

        public override async Task EnterMap()
        {
            Map = Kernel.MapManager.GetMap(MapIdentity);
            if (Map != null)
            {
                await Map.AddAsync(this);
            }
        }

        public override async Task LeaveMap()
        {
            if (Map != null)
            {
                await Map.RemoveAsync(Identity);
            }
        }

        #endregion

        #region Task and Data

        public int GetData(string szAttr)
        {
            switch (szAttr.ToLower())
            {
                case "data0": return Data0;
                case "data1": return Data1;
                case "data2": return Data2;
                case "data3": return Data3;
            }
            return 0;
        }

        public void SetData(string szAttr, int value)
        {
            switch (szAttr.ToLower())
            {
                case "data0": Data0 = value; break;
                case "data1": Data1 = value; break;
                case "data2": Data2 = value; break;
                case "data3": Data3 = value; break;
            }
        }

        public virtual uint OwnerIdentity { get; set; }
        public virtual bool Vending { get; set; }
        public virtual uint Task0 { get; }
        public virtual uint Task1 { get; }
        public virtual uint Task2 { get; }
        public virtual uint Task3 { get; }
        public virtual uint Task4 { get; }
        public virtual uint Task5 { get; }
        public virtual uint Task6 { get; }
        public virtual uint Task7 { get; }

        public virtual int Data0 { get; set; }
        public virtual int Data1 { get; set; }
        public virtual int Data2 { get; set; }
        public virtual int Data3 { get; set; }

        #endregion

        #region Functions

        #region Shop

        public List<DbGoods> ShopGoods = new List<DbGoods>();

        #endregion

        #endregion

        #region Common Checks

        public bool IsShopNpc()
        {
            return Type == SHOPKEEPER_NPC;
        }

        public bool IsTaskNpc()
        {
            return Type == TASK_NPC;
        }

        public bool IsStorageNpc()
        {
            return Type == STORAGE_NPC;
        }

        public bool IsUserNpc()
        {
            return OwnerType == 1;
        }

        public bool IsSynNpc()
        {
            return OwnerType == 2;
        }

        public bool IsSynFlag()
        {
            return (Type == SYNFLAG_NPC || Type == ROLE_CTFBASE_NPC) && IsSynNpc();
        }

        public bool IsCtfFlag()
        {
            return IsSynFlag() && Type == ROLE_CTFBASE_NPC;
        }

        #endregion

        #region Management

        public virtual bool DelNpc()
        {
            return false;
        }

        #endregion
    }
}