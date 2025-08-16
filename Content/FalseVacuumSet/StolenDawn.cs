using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using FranciumCalamityWeapons.Content.Projectiles;
using FranciumCalamityWeapons.Content.Resources;
using Terraria.Audio;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.LoreItems;
using CalamityMod.Tiles.Furniture.CraftingStations;

namespace FranciumCalamityWeapons.Content.FalseVacuumSet
{
	public class StolenDawn : ModItem
	{
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults() {
			Item.width = 182;
			Item.height = 38;
			Item.autoReuse = true;
			Item.damage = 6200;
			Item.DamageType = DamageClass.Default;
			Item.knockBack = 0f;
			Item.rare = ModContent.RarityType<ShimmeringRarity>();
			Item.shootSpeed = 10f;
			Item.useAnimation = 5;
			Item.useTime = 5;
			Item.UseSound = new SoundStyle("FranciumCalamityWeapons/Audio/StolenDawnUse") with
			{
				Volume = 0.8f,
				PitchVariance = 0.6f,
				MaxInstances = 10,
			};
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.value = Item.buyPrice(gold: 1);
			Item.shoot = ModContent.ProjectileType<NightFlameProjectile>();
			
			Item.useAmmo = ItemID.Gel;
		}
		
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;

			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) {
				position += muzzleOffset;
			}
		}

        public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<HeatDeath>(18)
				.AddIngredient<CleansingBlaze>(1)
				.AddIngredient<LoreAwakening>(1)
				.AddTile<DraedonsForge>()
				.Register();
		}

	}
}
