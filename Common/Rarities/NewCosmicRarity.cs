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
	public class NewCosmicRarity : ModRarity
    {
        public override Color RarityColor => new Color(0, 0, 0); // Change color as needed

        public override int GetPrefixedRarity(int offset, float valueMult)
        {
            if (offset > 0) 
            {
                return ItemRarityID.Expert; // Upgrade to next rarity if necessary
            }

            return Type; // No lower tier, so return itself
        }
    }
	
}