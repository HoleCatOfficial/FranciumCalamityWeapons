using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.ID;

namespace FranciumCalamityWeapons.Common.Rarities
{
	public class CoolArsenalRarity : ModRarity
    {
        public override Color RarityColor => new Color(255, 64, 31); 

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0) 
            {
                return ItemRarityID.Blue;
            }

            return Type;
        }
    }
	
}