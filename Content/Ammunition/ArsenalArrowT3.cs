
using CalamityMod.Items.Materials;
using DestroyerTest.Content.Resources;
using FranciumCalamityWeapons.Common.Rarities;
using FranciumCalamityWeapons.Content.Projectiles.Ammo;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Ammunition
{
	public class ArsenalArrowT3 : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 99;
		}

		public override void SetDefaults() {
			Item.width = 18;
			Item.height = 60;
			Item.damage = 37;
			Item.DamageType = DamageClass.Ranged;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.knockBack = 8f;
			Item.rare = ModContent.RarityType<CoolArsenalRarity>();
            Item.value = 1660;
			Item.shoot = ModContent.ProjectileType<ArsenalArrowT3Projectile>();
			Item.shootSpeed = 16f;
			Item.ammo = AmmoID.Arrow;
		}

		public override void AddRecipes() {
			CreateRecipe(100)
				.AddIngredient<DubiousPlating>(70)
                .AddIngredient<LifeAlloy>(45)
                .AddIngredient<CosmiliteBar>(22)
                .AddIngredient<DarksunFragment>(16)
                .AddIngredient<Living_Shadow>(16)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}