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
using FranciumCalamityWeapons.Content.Projectiles;
using DestroyerTest.Common;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Tiles.Furniture.CraftingStations;

namespace FranciumCalamityWeapons.Content.Scepter
{
	public class ExarchicEnforcer : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
        }
        //Weapon Properties
        
		SoundStyle Throw = new SoundStyle("FranciumCalamityWeapons/Audio/VoidScepterThrow") with { Volume = 0.75f, PitchVariance = 0.8f };
		

        public override void SetDefaults()
		{
			Item.width = 148;
			Item.height = 132;
			Item.value = Item.sellPrice(gold: 2, silver: 50);
			Item.rare = ItemRarityID.Pink;
			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 0;  // The knockback of your sword, this is dynamically adjusted in the projectile code.
			Item.autoReuse = true; // This determines whether the weapon has autoswing

			Item.crit = 19; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.ion
			Item.shoot = ModContent.ProjectileType<ExarchicEnforcerHoldout>(); // The sword as a projectile
			Item.shootSpeed = 12f;
			Item.noUseGraphic = true; // The sword is actually a "projectile", so the item should not be visible when used
			Item.damage = 435 + (int)Math.Round(ScepterClassStats.DamageModifier); // The damage of your sword, this is dynamically adjusted in the projectile code.
			Item.DamageType = ModContent.GetInstance<ScepterClass>();
			Item.channel = true;
		}

		
		public override bool AltFunctionUse(Player player) {
			return true;
		}

		public override bool CanUseItem(Player player) {
            // THROW AI
            if (player.altFunctionUse == 2)
            {
                Item.useStyle = ItemUseStyleID.Shoot; // Change to the desired use style
                Item.useTime = 60; // Adjust as needed
                Item.autoReuse = false;
                Item.useAnimation = 60; // Adjust as needed
                Item.shoot = ModContent.ProjectileType<ExarchicEnforcerThrown>(); // The projectile is what makes a shortsword work
                Item.shootSpeed = 25.0f; // This value bleeds into the behavior of the projectile as velocity, keep that in mind when tweaking values
                Item.noUseGraphic = true; // The sword is actually a "projectile", so the item should not be visible when used
                Item.noMelee = false; // The projectile will do the damage and not the item
                Item.UseSound = Throw;
                Item.crit = 19;
                Item.damage = 435 + (int)Math.Round(ScepterClassStats.DamageModifier); // The damage of your sword, this is dynamically adjusted in the projectile code.
                Item.DamageType = ModContent.GetInstance<ScepterClass>();
                Item.channel = true;
            }
            // SHOOT AI
            else
            {
                Item.width = 148;
            	Item.height = 132;
                Item.value = Item.sellPrice(gold: 2, silver: 50);
                Item.rare = ItemRarityID.Pink;
                Item.useTime = 40;
                Item.useAnimation = 40;
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.knockBack = 0;  // The knockback of your sword, this is dynamically adjusted in the projectile code.
                Item.autoReuse = true; // This determines whether the weapon has autoswing
                Item.crit = 19; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.ion
                Item.shoot = ModContent.ProjectileType<ExarchicEnforcerHoldout>(); // The sword as a projectile
                Item.shootSpeed = 0.01f;
                Item.noUseGraphic = true; // The sword is actually a "projectile", so the item should not be visible when used
                Item.damage = 435 + (int)Math.Round(ScepterClassStats.DamageModifier); // The damage of your sword, this is dynamically adjusted in the projectile code.
                Item.DamageType = ModContent.GetInstance<ScepterClass>();
                Item.channel = true;
            }

			
			return player.ownedProjectileCounts[Item.shoot] < 1;
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

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.ownedProjectileCounts[type] < 1)
			{
				Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
			}

			if (player.ownedProjectileCounts[ModContent.ProjectileType<VoidEye>()] < 1)
			{
				if (player.altFunctionUse != 2)
				{
					Projectile.NewProjectile(source, Main.MouseWorld, new Vector2(0.001f, 0.001f), ModContent.ProjectileType<VoidEye>(), damage, knockback, player.whoAmI);
				}
			}
			return false; // Prevents vanilla from auto-firing
		}
		public override void AddRecipes()
        {
			if (ModLoader.TryGetMod("CalamityEntropy", out Mod CalamityEntropy))
		{
			if (CalamityEntropy.TryFind("VoidBar", out ModItem VB))
			{
				CreateRecipe()
				.AddIngredient<DarkPlasma>(8)
				.AddIngredient<EndothermicEnergy>(22)
				.AddIngredient<AscendantSpiritEssence>(14)
				.AddIngredient(VB.Type, 14)
				.AddTile<CosmicAnvil>()
				.Register();
			}
			}
			else
			{
				CreateRecipe()
				.AddIngredient<DarkPlasma>(8)
				.AddIngredient<EndothermicEnergy>(22)
				.AddIngredient<AscendantSpiritEssence>(14)
				.AddTile<CosmicAnvil>()
				.Register();
			}
        }
    }
} 