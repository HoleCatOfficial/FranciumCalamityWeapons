
using CalamityMod;
using CalamityMod.Items.LoreItems;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.Tiles.Furniture.CraftingStations;
using FranciumCalamityWeapons.Common.Rarities;
using FranciumCalamityWeapons.Content.Melee;
using FranciumCalamityWeapons.Content.Projectiles;
using FranciumCalamityWeapons.Content.Resources;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.FalseVacuumSet
{
	// ExampleCustomSwingSword is an example of a sword with a custom swing using a held projectile
	// This is great if you want to make melee weapons with complex swing behavior
	public class Supremacy : ModItem
	{
		public override void SetDefaults()
		{
			// Common Properties
			Item.width = 200;
			Item.height = 200;
			Item.value = Item.sellPrice(gold: 25, silver: 60);
			Item.rare = ModContent.RarityType<NewCosmicRarity>();


			// Use Properties
			// Note that useTime and useAnimation for this item don't actually affect the behavior because the held projectile handles that. 
			// Each attack takes a different amount of time to execute
			// Conforming to the item useTime and useAnimation makes it much harder to design
			// It does, however, affect the item tooltip, so don't leave it out.
			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = ItemUseStyleID.Shoot;

			// Weapon Properties
			Item.knockBack = 70;  // The knockback of your sword, this is dynamically adjusted in the projectile code.
			Item.autoReuse = true; // This determines whether the weapon has autoswing
			Item.damage = 5000; // The damage of your sword, this is dynamically adjusted in the projectile code.
			Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>(); // Deals melee damage
			Item.noMelee = true;  // This makes sure the item does not deal damage from the swinging animation
			Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand

			// Projectile Properties
			Item.shoot = ModContent.ProjectileType<SupremacySwing>(); // The sword as a projectile
		}

		public override bool MeleePrefix()
		{
			return true; // return true to allow weapon to have melee prefixes (e.g. Legendary)
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<HeatDeath>(27)
				.AddIngredient<Overlord>(1)
				.AddIngredient<LoreAwakening>(1)
				.AddTile<DraedonsForge>()
				.Register();
		}
	}
}