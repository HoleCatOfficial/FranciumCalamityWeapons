using CalamityMod;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Tiles.Furniture.CraftingStations;
using DestroyerTest.Content.RogueItems;
using FranciumCalamityWeapons.Content.Projectiles;
using FranciumCalamityWeapons.Content.Resources;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.FalseVacuumSet
{
	public class PasserUmbraeSeparatricis : ModItem
	{
		public override void SetDefaults() {
			// Alter any of these values as you see fit, but you should probably keep useStyle on 1, as well as the noUseGraphic and noMelee bools

			// Common Properties
			Item.rare = ItemRarityID.Pink;
			Item.value = Item.sellPrice(silver: 5);
			Item.maxStack = 1;
            Item.width = 102;
            Item.height = 102;

			// Use Properties
            Item.useStyle = ItemUseStyleID.Swing;
			Item.useAnimation = 10;
			Item.useTime = 10;
			Item.UseSound = new SoundStyle("FranciumCalamityWeapons/Audio/ThrowWoosh1") with
			{
				Volume = 0.8f,
				PitchVariance = 0.6f,
				MaxInstances = 10,
			};
			Item.autoReuse = true;
			Item.consumable = false;

			// Weapon Properties			
			Item.damage = 2700;
			Item.knockBack = 5f;
			Item.noUseGraphic = true; // The item should not be visible when used
			Item.noMelee = true; // The projectile will do the damage and not the item
			Item.DamageType = ModContent.GetInstance<RogueDamageClass>();

			// Projectile Properties
			Item.shootSpeed = 40f;
			Item.shoot = ModContent.ProjectileType<PUSProj>(); // The projectile that will be thrown
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (player.Calamity().StealthStrikeAvailable())
			{
				SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/SparrowAoESpawn"), player.Center);
				Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SparrowAoE>(), damage, 0);
			}
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override bool? UseItem(Player player)
		{
			player.GetModPlayer<ScreenshakePlayer>().screenshakeMagnitude = 8;
			player.GetModPlayer<ScreenshakePlayer>().screenshakeTimer = 24;
			return true;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient<HeatDeath>(13)
				.AddIngredient<P_Noctis>()
				.AddIngredient<Wrathwing>(1)
				.AddIngredient<LoreAwakening>(1)
				.AddTile<DraedonsForge>()
				.Register();
        }
	}
}