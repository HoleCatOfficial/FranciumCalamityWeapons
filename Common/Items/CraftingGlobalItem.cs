using System;
using System.Collections.Generic;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using DestroyerTest.Content.Tiles.Riftplate;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Common.Items
{
    public class CraftingModification : ModSystem
    {
        //Calamity Recipe Code
        public override void AddRecipes()
        {

        }

        public override void PostAddRecipes()
        {
            foreach (Recipe recipe in Main.recipe)
            {
                if (recipe.HasResult<LifeAlloy>())
                {
                    recipe.AddIngredient(ModContent.ItemType<Item_Riftplate>(), 1);
                }
            }
        }

    }
}