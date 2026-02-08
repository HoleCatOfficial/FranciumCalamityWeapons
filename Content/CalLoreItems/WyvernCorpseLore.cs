using CalamityMod;
using CalamityMod.Items.LoreItems;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.CalLoreItems
{
    public class WyvernCorpseLore : LoreItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ModContent.RarityType<CrimsonSpecialRarity>();
            Item.maxStack = 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Item_WyvernCorpseTrophy>()
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }
}
