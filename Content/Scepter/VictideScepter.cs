using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using FranciumCalamityWeapons.Content.Scepter;
using static Terraria.ModLoader.ModContent;
using System.Collections.Generic;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using DestroyerTest.Common;

namespace FranciumCalamityWeapons.Content.Scepter
{
	public class VictideScepter : ModItem
	{

        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }
        //Weapon Properties
        
        public override void SetDefaults() {
			Item.width = 34;
			Item.height = 34;
			Item.value = Item.sellPrice(gold: 2, silver: 50);
			Item.rare = ItemRarityID.Pink;
			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item25;
			Item.knockBack = 0;  // The knockback of your sword, this is dynamically adjusted in the projectile code.
			Item.autoReuse = true; // This determines whether the weapon has autoswing
			
            Item.crit = 10; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.ion
			Item.shoot = ModContent.ProjectileType<PinkCoral>(); // The sword as a projectile
            Item.shootSpeed = 10f;
			Item.noUseGraphic = false; // The sword is actually a "projectile", so the item should not be visible when used
			Item.damage = 7 + (int)Math.Round(ScepterClassStats.DamageModifier); // The damage of your sword, this is dynamically adjusted in the projectile code.
			Item.DamageType = ModContent.GetInstance<ScepterClass>();
        }

		
		public override bool AltFunctionUse(Player player) {
			return true;
		}

		public override bool CanUseItem(Player player) {
			// THROW AI
			if (player.altFunctionUse == 2) {
				Item.useStyle = ItemUseStyleID.Shoot; // Change to the desired use style
				Item.useTime = 60; // Adjust as needed
				Item.autoReuse = false;
				Item.useAnimation = 60; // Adjust as needed
				Item.shoot = ModContent.ProjectileType<VictideScepterThrown>(); // The projectile is what makes a shortsword work
				Item.shootSpeed = 25.0f; // This value bleeds into the behavior of the projectile as velocity, keep that in mind when tweaking values
				Item.noUseGraphic = true; // The sword is actually a "projectile", so the item should not be visible when used
				Item.noMelee = false; // The projectile will do the damage and not the item
				Item.UseSound = SoundID.Item169;
				Item.crit = 46;
                Item.damage = 7 + (int)Math.Round(ScepterClassStats.DamageModifier); // The damage of your sword, this is dynamically adjusted in the projectile code.
				Item.DamageType = ModContent.GetInstance<ScepterClass>();
			}
			// SHOOT AI
			else {
			Item.width = 34;
			Item.height = 34;
			Item.value = Item.sellPrice(gold: 2, silver: 50);
			Item.rare = ItemRarityID.Pink;
			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item25;
			Item.knockBack = 0;  // The knockback of your sword, this is dynamically adjusted in the projectile code.
			Item.autoReuse = true; // This determines whether the weapon has autoswing
            Item.crit = 10; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.ion
			Item.shoot = ModContent.ProjectileType<PinkCoral>(); // The sword as a projectile
            Item.shootSpeed = 10f;
			Item.noUseGraphic = false; // The sword is actually a "projectile", so the item should not be visible when used
			Item.damage = 7 + (int)Math.Round(ScepterClassStats.DamageModifier); // The damage of your sword, this is dynamically adjusted in the projectile code.
			Item.DamageType = ModContent.GetInstance<ScepterClass>();
            }
			return base.CanUseItem(player);
			
		}
		public override void UseItemFrame(Player player)
		{
			if (player.altFunctionUse == 2) // Throwing mode
			{
				float animationSpeed = 8.0f; // You can modify this to change the animation speed.

				// Calculate the progress, but limit it to a max of 1.0
				float progress = ((player.itemAnimationMax - player.itemAnimation) / (float)player.itemAnimationMax);
				progress = Math.Min(progress * animationSpeed, 1.0f); // Clamps progress to a max of 1

				// Start angle at 180 degrees (upwards)
				float startAngle = MathHelper.ToRadians(180f);

				// Declare endAngle here to ensure it's accessible outside of the if blocks
				float endAngle;

				// Set the end angle based on player direction
				if (player.direction == 1)
				{
					endAngle = MathHelper.ToRadians(270f); // Right side, end angle 270
				}
				else if (player.direction == -1)
				{
					endAngle = MathHelper.ToRadians(90f); // Left side, end angle 90
				}
				else
				{
					endAngle = startAngle; // Default case (shouldn't happen unless player.direction is unexpected)
				}

				// Interpolate between start and end angle
				float armRotation = MathHelper.Lerp(startAngle, endAngle, progress);

				// If the progress has reached the end, stop the arm from rotating further
				if (progress == 1.0f)
				{
					// Ensure the arm stays at the final angle and doesn't continue animating
					armRotation = endAngle;
				}

				// Apply the final rotation to the player's arm
				player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRotation);
			}
		}

		public override bool? UseItem(Player player)
		{
			if (player.altFunctionUse == 2) // Throwing mode
			{
				// Find the active instance of the thrown projectile
				foreach (Projectile proj in Main.projectile)
				{
					// Ensure the projectile exists, is owned by the player, and is the right type
					if (proj.active && proj.owner == player.whoAmI && proj.type == ModContent.ProjectileType<VictideScepterThrown>())
					{
						// Safely get the existenceTimer value
						if (proj.ModProjectile is VictideScepterThrown thrownProj)
						{
							// Set useTime to match the projectile's lifetime
							Item.useTime = thrownProj.existenceTimer;
						}
						break; // Stop searching once we find the right projectile
					}
				}

				return true;
			}

			return base.UseItem(player);
		}




		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// Check if another projectile of the same type is active
			foreach (Projectile proj in Main.projectile)
			{
				if (proj.active && proj.owner == player.whoAmI && proj.type == ModContent.ProjectileType<VictideScepterThrown>())
				{
					return false; // Prevent new projectile from being fired
				}
			}

			return true; // Allow firing if no other projectiles exist
		}

		/*public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			// Randomly set type to either the original (as defined by the ammo), a vanilla projectile, or a mod projectile.
			if (player.altFunctionUse != 2)
			{
				type = Main.rand.Next(new int[] {
					type,
					ModContent.ProjectileType<TealCoral>(),
					ModContent.ProjectileType<PinkCoral>(),
					ModContent.ProjectileType<PearlChunk>()
				});
			}
		}*/

		private int lastProjectileType = -1; // Store the last selected projectile type

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			// Randomly set type to either the original (as defined by the ammo), a vanilla projectile, or a mod projectile.
			if (player.altFunctionUse != 2) {
				int[] possibleTypes = new int[] {
					type,
					ModContent.ProjectileType<TealCoral>(),
					ModContent.ProjectileType<PinkCoral>(),
					ModContent.ProjectileType<PearlChunk>()
				};

				// Filter out the last projectile type
				List<int> filteredTypes = new List<int>(possibleTypes);
				if (lastProjectileType != -1) {
					filteredTypes.Remove(lastProjectileType);
				}

				// Select a new type from the filtered list
				type = filteredTypes[Main.rand.Next(filteredTypes.Count)];

				// Update the last projectile type
				lastProjectileType = type;

				// Reset the flag if all projectiles have been used
				if (filteredTypes.Count == 1) {
					lastProjectileType = -1; // Allow all projectiles to be selected again
				}
			}
		}

		public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient<SeaRemains>(6)
				.AddIngredient(ItemID.Wood, 6)
				.AddIngredient(ItemID.GoldBar, 4)
				.AddTile<EutrophicShelf>()
				.Register();
        }


		

    }
} 