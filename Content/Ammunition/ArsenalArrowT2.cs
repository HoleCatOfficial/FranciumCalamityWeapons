
using CalamityMod.Items.Materials;
using FranciumCalamityWeapons.Common.Rarities;
using FranciumCalamityWeapons.Content.Projectiles.Ammo;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Ammunition
{
	public class ArsenalArrowT2 : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 99;
		}

		public override void SetDefaults() {
			Item.width = 14;
			Item.height = 50;
			Item.damage = 10;
			Item.DamageType = DamageClass.Ranged;
			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.knockBack = 2f;
			Item.rare = ModContent.RarityType<CoolArsenalRarity>();
            Item.value = 420;
			Item.shoot = ModContent.ProjectileType<ArsenalArrowT2Projectile>();
			Item.shootSpeed = 8f;
			Item.ammo = AmmoID.Arrow;
		}

		public override void AddRecipes() {
			CreateRecipe(100)
				.AddIngredient<DubiousPlating>(50)
                .AddIngredient(ItemID.MythrilBar, 18)
                .AddIngredient(ItemID.SoulofNight, 16)
                .AddIngredient(ItemID.RocketI, 10)
                .AddIngredient(ItemID.ExplosivePowder, 10)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}