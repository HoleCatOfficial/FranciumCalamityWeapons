
using CalamityMod.Items.Materials;
using FranciumCalamityWeapons.Common.Rarities;
using FranciumCalamityWeapons.Content.Projectiles.Ammo;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Ammunition
{
	public class ArsenalArrowT1 : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 99;
		}

		public override void SetDefaults() {
			Item.width = 14;
			Item.height = 42;
			Item.damage = 5;
			Item.DamageType = DamageClass.Ranged;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.knockBack = 1.5f;
			Item.rare = ModContent.RarityType<CoolArsenalRarity>();
            Item.value = 140;
			Item.shoot = ModContent.ProjectileType<ArsenalArrowT1Projectile>();
			Item.shootSpeed = 4f;
			Item.ammo = AmmoID.Arrow;
		}

		public override void AddRecipes() {
			CreateRecipe(100)
				.AddIngredient<DubiousPlating>(40)
                .AddIngredient<EnergyCore>(16)
				.AddTile(TileID.HeavyWorkBench)
				.Register();
		}
	}
}