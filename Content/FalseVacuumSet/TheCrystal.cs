
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Tiles.Furniture.CraftingStations;
using FranciumCalamityWeapons.Content.Projectiles;
using FranciumCalamityWeapons.Content.Resources;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.FalseVacuumSet
{
	public class TheCrystal : ModItem
	{
        // You can use a vanilla texture for your item by using the format: "Terraria/Item_<Item ID>".
        public override string Texture => "FranciumCalamityWeapons/Content/FalseVacuumSet/EyeofNirvana";
		

		public override void SetDefaults() {
			// Start by using CloneDefaults to clone all the basic item properties from the vanilla Last Prism.
			// For example, this copies sprite size, use style, sell price, and the item being a magic weapon.
			Item.CloneDefaults(ItemID.LastPrism);
			Item.mana = 16;
			Item.damage = 600;
			Item.shoot = ModContent.ProjectileType<TheCrystalHoldout>();
			Item.shootSpeed = 30f;

		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<HeatDeath>(18)
				.AddIngredient<DarkSpark>(1)
				.AddIngredient<LoreAwakening>(1)
				.AddTile<DraedonsForge>()
				.Register();
		}

		// Because this weapon fires a holdout projectile, it needs to block usage if its projectile already exists.
		public override bool CanUseItem(Player player) {
			return player.ownedProjectileCounts[ModContent.ProjectileType<TheCrystalHoldout>()] <= 0;
		}
	}
}