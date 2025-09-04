using System;
using CalamityMod.Items.Weapons.Melee;
using FranciumCalamityWeapons.Content.Resources;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Common.Items
{
    public class CraftingGlobalItem : GlobalItem
    {
        public override void OnCreated(Item item, ItemCreationContext context)
        {
            if (item.type == ModContent.ItemType<HeatDeath>() && context is RecipeItemCreationContext)
            {
                SoundEngine.PlaySound(SoundID.Item4);

            }
        }

        public override void AddRecipes()
        {
            foreach (var item in Main.item)
            {
                if (item.type == ModContent.ItemType<ElementalLance>())
                {
                    
                }
            }
        }

    }
}