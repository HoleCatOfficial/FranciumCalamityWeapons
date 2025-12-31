
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
			Item.width = 200;
			Item.height = 200;
			Item.value = Item.sellPrice(gold: 25, silver: 60);
			Item.rare = ModContent.RarityType<NewCosmicRarity>();
			Item.useTime = 120;
			Item.useAnimation = 120;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 70;
			Item.autoReuse = true;
			Item.damage = 8000;
			Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<SupremacySwing>(); // The sword as a projectile
		}

		public override bool MeleePrefix()
		{
			return true; // return true to allow weapon to have melee prefixes (e.g. Legendary)
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<HeatDeath>(10)
				.AddIngredient<Overlord>(1)
				.AddIngredient<LoreDevourerofGods>(1)
				.AddTile<DraedonsForge>()
				.Register();
		}
	}
}