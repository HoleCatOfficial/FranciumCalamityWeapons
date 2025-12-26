
using System;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Resources;
using FranciumCalamityWeapons.Common.Rarities;
using FranciumCalamityWeapons.Content.Dusts;
using FranciumCalamityWeapons.Content.Projectiles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Melee
{
    public class NebulousIceDart : ModProjectile
    {
        public override string Texture => "FranciumCalamityWeapons/Content/Melee/NebulousIceDart";
        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.aiStyle = ProjAIStyleID.Beam; // Terra Beam AI style
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 8;
            Projectile.timeLeft = 600;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1;
        }

		public void OnHitNPC(NPC target, Player player, NPC.HitInfo hit, int damageDone)
		{
			SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/IceImpact"));
			
			target.AddBuff(ModContent.BuffType<GlacialState>(), 600);
		}

        public override void AI()
        {
            PRTLoader.NewParticle(PRTLoader.GetParticleID<SimpleParticle>(), Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Vector2.Zero, Color.White, Main.rand.NextFloat(0.100f, 1.000f));
        }
    }
	public class NebulousShock : ModItem
	{
		private bool isThrowingMode = false; // Tracks the current mode

        public override void SetDefaults() {
            SetSwingModeDefaults(); // Initialize with swing mode defaults
        }

        private void SetSwingModeDefaults() {
            Item.CloneDefaults(ItemID.TerraBlade);
			Item.width = 60; // The item texture's width.
			Item.height = 62; // The item texture's height.

			Item.useStyle = ItemUseStyleID.Swing; // The useStyle of the Item.
			Item.useTime = 20; // The time span of using the weapon. Remember in terraria, 60 frames is a second.
			Item.useAnimation = 20; // The time span of the using animation of the weapon, suggest setting it the same as useTime.
			Item.autoReuse = true; // Whether the weapon can be used more than once automatically by holding the use button.

			Item.DamageType = DamageClass.Melee; // Whether your item is part of the melee class.
			Item.damage = 1600; // The damage your item deals.
			Item.knockBack = 0; // The force of knockback of the weapon. Maximum is 20
			Item.crit = 54; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.

			Item.value = Item.buyPrice(gold: 1); // The value of the weapon in copper coins.
			Item.rare = ModContent.RarityType<NewCosmicRarity>(); // Give this item our custom rarity.
			Item.shoot = ModContent.ProjectileType<NebulousIceDart>();
        }

        private void SetThrowingModeDefaults() {
            Item.width = 60;
            Item.height = 62;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 70;
            Item.useAnimation = 70;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Melee;
            Item.damage = 1000;
            Item.knockBack = 30;
            Item.crit = 46;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item169;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<NebulousShockThrown>();
            Item.shootSpeed = 20f;
        }

        public override bool CanUseItem(Player player) {
            if (isThrowingMode) {
                SetThrowingModeDefaults();
            } else {
                SetSwingModeDefaults();
			}
            return base.CanUseItem(player);
        }

        public override bool AltFunctionUse(Player player) {
            // Allow alternate function use to toggle modes
            return true;
        }

        public override bool? UseItem(Player player) {
            if (player.altFunctionUse == 2) { // Check if the alternate function is being used
                isThrowingMode = !isThrowingMode; // Toggle the mode
                SoundEngine.PlaySound(SoundID.Item35); // Play a sound to indicate the mode change
                if (isThrowingMode) {
                    Main.NewText("Throwing", Color.White);
                } else {
                    Main.NewText("Shooting", Color.White);
                }
                return true; // Indicate that the alternate function was used
            }
            return base.UseItem(player);
        }

        public override void UseItemFrame(Player player) {
            if (isThrowingMode) {
                float animationSpeed = 8.0f;
                float progress = ((player.itemAnimationMax - player.itemAnimation) / (float)player.itemAnimationMax);
                progress = Math.Min(progress * animationSpeed, 1.0f);
                float startAngle = MathHelper.ToRadians(180f);
                float endAngle = player.direction == 1 ? MathHelper.ToRadians(270f) : MathHelper.ToRadians(90f);
                float armRotation = MathHelper.Lerp(startAngle, endAngle, progress);
                if (progress == 1.0f) {
                    armRotation = endAngle;
                }
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRotation);
            }
        }


		//public override void MeleeEffects(Player player, Rectangle hitbox) {
			//if (Main.rand.NextBool(3)) {
				// Emit dusts when the sword is swung
				//Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<Dusts.Sparkle>());
			//}
		//}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) {
			target.AddBuff(ModContent.BuffType<GlacialState>(), 600);
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
            CreateRecipe()
            .AddIngredient<CosmiliteBar>(10)
            .AddIngredient<EndothermicEnergy>(15)
            .AddIngredient<ShadeParticle>(3)
            .AddTile<CosmicAnvil>()
            .Register();
		}
	}
}