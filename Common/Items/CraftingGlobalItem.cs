using System;
using System.Collections.Generic;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using DestroyerTest.Content.Tiles.Riftplate;
using FranciumCalamityWeapons.Content.Resources;
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
            static Func<Recipe, bool> Vanilla(int itemID) => r => r.Mod is null && r.HasResult(itemID);
            static Func<Recipe, bool> Calamity(int itemID) => r => r.Mod is CalamityMod.CalamityMod && r.HasResult(itemID);
            static Action<Recipe> AddIngredient(int itemID, int stack = 1) => r => r.AddIngredient(itemID, stack);
            var edits = new Dictionary<Func<Recipe, bool>, Action<Recipe>>(128)
            {
                { Calamity(ModContent.ItemType<LifeAlloy>()), AddIngredient(ModContent.ItemType<Item_Riftplate>(), 1) },
            };
        }

    }
}