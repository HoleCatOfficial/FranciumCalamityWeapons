

using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Rarity;
using FranciumCalamityWeapons.Common.Rarities;
using FranciumCalamityWeapons.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.FalseVacuumSet
{
    public class IntestinalLacerator : ModItem
    {
        public override void SetDefaults()
        {
            // This method quickly sets the whip's properties.
            // Mouse over to see its parameters.
            Item.DefaultToWhip(ModContent.ProjectileType<IntestinalLaceratorProj>(), 280, 2, 3);
            Item.width = 36;
            Item.height = 30;
            Item.autoReuse = true;

            Item.rare = ItemRarityID.Red;
        }

        // Makes the whip receive melee prefixes
        public override bool MeleePrefix()
        {
            return true;
        }
        
        public override void AddRecipes()
        {
           CreateRecipe()
				.AddIngredient<BloodstoneCore>(6)
				.AddIngredient<UelibloomBar>(2)
				.AddTile<BotanicPlanter>()
				.Register();
		}
	}
}