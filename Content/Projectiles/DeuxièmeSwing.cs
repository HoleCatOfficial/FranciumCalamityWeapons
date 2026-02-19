
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles
{
	public class DeuxièmeSwing : ModProjectile
	{
		private enum AttackStage // What stage of the attack is being executed, see functions found in AI for description
		{
			Prepare,
			Execute,
			Unwind
		}

		private AttackStage CurrentStage 
        {
			get => (AttackStage)Projectile.localAI[0];
			set {
				Projectile.localAI[0] = (float)value;
				Timer = 0; // reset the timer when the projectile switches states
			}
		}

		private ref float InitialAngle => ref Projectile.ai[1]; // Angle aimed in (with constraints)
		private ref float Timer => ref Projectile.ai[2]; // Timer to keep track of progression of each stage
		private ref float Progress => ref Projectile.localAI[1]; // Position of sword relative to initial angle
		private ref float Size => ref Projectile.localAI[2];

		private float prepTime => 8f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
		private float hideTime => 20f / Owner.GetTotalAttackSpeed(Projectile.DamageType);

		private Player Owner => Main.player[Projectile.owner];

        private bool CanContinueSwing(Player player)
        {
            if (player.dead || player.CCed || !player.active)
            {
                return false;
            }
            else
            {
                return player.controlUseItem;
            }
        }

		List<float> OldRotations = new List<float>();
        List<float> OldScales = new List<float>();

		public override void SetStaticDefaults() 
        {
			ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
			ProjectileID.Sets.AllowsContactDamageFromJellyfish[Type] = true;
		}

		public override void SetDefaults() 
        {
			Projectile.width = 164; // Hitbox width of projectile
			Projectile.height = 164; // Hitbox height of projectile
			Projectile.friendly = true; // Projectile hits enemies
			Projectile.timeLeft = 10000; // Time it takes for projectile to expire
			Projectile.penetrate = -1; // Projectile pierces infinitely
			Projectile.tileCollide = false; // Projectile does not collide with tiles
			Projectile.ownerHitCheck = true; // Make sure the owner of the projectile has line of sight to the target (aka can't hit things through tile).
			Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>(); // Projectile is a melee projectile
		}

		public override void OnSpawn(IEntitySource source) 
        {
			Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
			
		}

		public override void SendExtraAI(BinaryWriter writer) 
        {
			// Projectile.spriteDirection for this projectile is derived from the mouse position of the owner in OnSpawn, as such it needs to be synced. spriteDirection is not one of the fields automatically synced over the network. All Projectile.ai slots are used already, so we will sync it manually.
			writer.Write((sbyte)Projectile.spriteDirection);
		}

		public override void ReceiveExtraAI(BinaryReader reader) 
        {
			Projectile.spriteDirection = reader.ReadSByte();
		}

		public override void AI() 
        {
			// Extend use animation until projectile is killed
			Owner.itemAnimation = 2;
			Owner.itemTime = 2;

			// Kill the projectile if the player dies or gets crowd controlled
			if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed) {
				Projectile.Kill();
				return;
			}

			// AI depends on stage and attack
			// Note that these stages are to facilitate the scaling effect at the beginning and end
			// If this is not desirable for you, feel free to simplify
			switch (CurrentStage) {
				case AttackStage.Prepare:
					Prepare();
					break;
				case AttackStage.Execute:
					Execute();
					break;
				default:
					Unwind();
					break;
			}

			SetSwordPosition();
			Timer++;
		}

		public override bool PreDraw(ref Color lightColor) 
        {
			// Calculate origin of sword (hilt) based on orientation and offset sword rotation (as sword is angled in its sprite)
			Vector2 origin;
			float rotationOffset;
			SpriteEffects effects;

			if (Projectile.spriteDirection > 0) {
				origin = new Vector2(0, Projectile.height);
				rotationOffset = MathHelper.ToRadians(45f);
				effects = SpriteEffects.None;
			}
			else {
				origin = new Vector2(Projectile.width, Projectile.height);
				rotationOffset = MathHelper.ToRadians(135f);
				effects = SpriteEffects.FlipHorizontally;
			}

			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);

			// Since we are doing a custom draw, prevent it from normally drawing
			return false;
		}

		// Find the start and end of the sword and use a line collider to check for collision with enemies
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) 
        {
			Vector2 start = Owner.MountedCenter;
			Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * Projectile.scale);
			float collisionPoint = 0f;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
		}

		// Do a similar collision check for tiles
		public override void CutTiles() 
        {
			Vector2 start = Owner.MountedCenter;
			Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
			Utils.PlotTileLine(start, end, 15 * Projectile.scale, DelegateMethods.CutTiles);
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) 
        {
			// Make knockback go away from player
			modifiers.HitDirectionOverride = target.position.X > Owner.MountedCenter.X ? 1 : -1;
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			SoundEngine.PlaySound(DTAssetLib.IdriGreatswordSlice(ChildSafety.Disabled) with { PitchVariance = 0.6f, MaxInstances = 0 });
			int splatterdir = target.position.X > Owner.MountedCenter.X ? 1 : -1;
			for (int i = 0; i < 7; i++)
			{
				PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticle>(), target.Center, new Vector2(Main.rand.NextFloat(2f, 6f) * splatterdir, 0).RotatedByRandom(0.1f), Color.Red * Main.rand.NextFloat(0.5f, 1f), 1f);
        	}	
			target.AddBuff(ModContent.BuffType<Shred>(), 180);
		}

		public void SetSwordPosition() 
        {
			Projectile.rotation = (InitialAngle + Projectile.spriteDirection * Progress) * Owner.direction;

			// Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
			Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
			Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

			// Adjust the position for reversed gravity.
			if (Owner.gravDir == -1f) {
				Projectile.rotation = 0f - Projectile.rotation;
				armPosition.Y = Owner.Bottom.Y + (Owner.position.Y - armPosition.Y);
			}

			armPosition.Y += Owner.gfxOffY;
			Projectile.Center = armPosition; // Set projectile to arm position
			Projectile.scale = Size * 1.2f * Owner.GetAdjustedItemScale(Owner.HeldItem); // Slightly scale up the projectile and also take into account melee size modifiers

			Owner.heldProj = Projectile.whoAmI; // set held projectile to this projectile
		}

		private void Prepare()
        {
            InitialAngle = (Main.MouseWorld - Owner.MountedCenter).ToRotation();
            Progress = 0f;
            Size = 1f;

            if (Timer >= prepTime)
            {
                CurrentStage = AttackStage.Execute;
            }
        }

		private float SPINSPEED = 0.01f; // radians per tick
		private int STimer = 0;
		public Vector2 swordTip;
		public Line SwordLine;
        private void Execute()
        {
			swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
			SwordLine = new Line(Owner.Center, swordTip);
			Vector2[] p = SwordLine.GetPointsAlongLine(10);

            if (CanContinueSwing(Owner))
            {
				if (SPINSPEED < 0.36f)
				{
					SPINSPEED += 0.008f;
				}
				else
				{
					foreach(Vector2 DustP in p)
					{
						Dust.NewDustPerfect(DustP, ModContent.DustType<ColorableNeonDust>(), SwordLine.GetLineRotation.ToRotationVector2() * 4f, 0, Color.Maroon);
					}
				}
                float speed = SPINSPEED * Owner.GetTotalAttackSpeed(Projectile.DamageType);
                Progress += speed * Projectile.spriteDirection;

                Size = 1f; // keep full size while spinning

                float speedRatio = Math.Min(1f, SPINSPEED / 0.36f); // Normalize to 0-1 range
				int soundInterval = (int)MathHelper.Lerp(200, 20, speedRatio); // Start at 200 ticks, go down to 20

				STimer++;
				if (STimer % soundInterval == 0)
				{
					SoundEngine.PlaySound(DTAssetLib.SwordSounds.StandardSwing with { PitchVariance = 1f });
				}

				if (STimer % 40 == 0 && SPINSPEED >= 0.36f)
                {
                    SoundEngine.PlaySound(DTAssetLib.ChargeBreak);
					Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, SwordLine.GetLineRotation.ToRotationVector2() * 8f, ModContent.ProjectileType<TenebrisFlamesFriendly>(), Projectile.damage / 3, 10, Owner.whoAmI);
				}
            }
            else
            {
                CurrentStage = AttackStage.Unwind;
            }
        }

		private void Unwind()
        {
			float speed = SPINSPEED * Owner.GetTotalAttackSpeed(Projectile.DamageType);
			Progress += speed * Projectile.spriteDirection;
            Size = 1f - MathHelper.SmoothStep(0, 1, Timer / hideTime);
			Projectile.Opacity = 1f - MathHelper.SmoothStep(0, 1, Timer / hideTime);

            if (Timer >= hideTime)
            {
                Projectile.Kill();
            }
        }

        public override void OnKill(int timeLeft)
        {
			OldRotations.Clear();
			OldScales.Clear();
        }
	}
}