using CalamityMod.Buffs.StatDebuffs;
using DestroyerTest.Common;
using FranciumCalamityWeapons.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    public class CausticShot : ModProjectile
    {
        // Store the target NPC using Projectile.ai[0]
        private NPC HomingTarget {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }

        public ref float DelayTimer => ref Projectile.ai[1];

        public override void SetStaticDefaults() {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
        }

		public override void SetDefaults()
		{
			Projectile.width = 10; // The width of projectile hitbox
			Projectile.height = 22; // The height of projectile hitbox

			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 1f; // How much light emit around the projectile
			Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.tileCollide = true;
			Projectile.netImportant = true;
			Projectile.netUpdate = true;
			}

		public int trailLength = 10;
		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = new Color(140, 234, 87);

			SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

			// --- Draw the main projectile ---
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			Main.EntitySpriteDraw(
				projectileTexture,
				Projectile.Center - Main.screenPosition,
				null,
				lightColor,
				Projectile.rotation,
				projectileTexture.Size() / 2,
				Projectile.scale,
				SpriteEffects.None,
				0
			);

			// --- Draw glow + trail ---
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			Texture2D glowTexture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/SimpleParticle").Value;
			Main.EntitySpriteDraw(
				glowTexture,
				Projectile.Center - Main.screenPosition,
				null,
				lightColor,
				Projectile.rotation,
				glowTexture.Size() / 2,
				Projectile.scale,
				SpriteEffects.None,
				0
			);

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


			Texture2D longtrailTexture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/Trail2").Value;
			Vector2 trailOrigin = new(longtrailTexture.Width / 2f, longtrailTexture.Height / 2f);

			for (int i = 0; i < trailLength && i < Projectile.oldPos.Length; i++)
			{
				float fade = (float)(trailLength - i) / trailLength;
				Color trailColor = lightColor * fade * 0.3f;
				trailColor.A = (byte)(fade * 100);

				Vector2 drawPosition = Projectile.oldPos[i] + (Projectile.Size / 2f) - Main.screenPosition;
				float scaleFactor = 0.1f;
				Main.EntitySpriteDraw(longtrailTexture, drawPosition, null, trailColor, Projectile.rotation, trailOrigin, (Projectile.scale * fade) * scaleFactor, SpriteEffects.None, 0);
			}

			// Restore normal batch
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			return false;
		}

			// Custom AI
			public override void AI() {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticle2>(), Projectile.Center, new Vector2(Main.rand.NextFloat(1, -1), -4), new Color(140, 234, 87), Main.rand.NextFloat(0.04f, 1.4f));

				SoundStyle[] Options = new SoundStyle[]
				{
					SoundID.LiquidsWaterLava with { Volume = Main.rand.NextFloat(0.01f, 0.2f), PitchVariance = 2f },
					SoundID.LiquidsHoneyLava with { Volume = Main.rand.NextFloat(0.01f, 0.2f), PitchVariance = 2f },
				};
				if (Main.rand.NextBool(3))
				{
					SoundEngine.PlaySound(Main.rand.Next(Options), Projectile.Center);
				}
				if (DelayTimer < 40)
				{
					DelayTimer += 1;
					return;
				}

				float maxDetectRadius = 400f; // The maximum radius at which a projectile can detect a target

				// First, we find a homing target if we don't have one
				if (HomingTarget == null) {
					HomingTarget = FindClosestNPC(maxDetectRadius);
				}

				// If we have a homing target, make sure it is still valid. If the NPC dies or moves away, we'll want to find a new target
				if (HomingTarget != null && !IsValidTarget(HomingTarget)) {
					HomingTarget = null;
				}

				// If we don't have a target, don't adjust trajectory
				if (HomingTarget == null)
					return;

				// If found, we rotate the projectile velocity in the direction of the target.
				// We only rotate by 3 degrees an update to give it a smooth trajectory. Increase the rotation speed here to make tighter turns
				float length = Projectile.velocity.Length();
				float targetAngle = Projectile.AngleTo(HomingTarget.Center);
				float rotamount = 0;
				rotamount += 4;
				rotamount = MathHelper.Clamp(rotamount, 0, 180);
				Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(rotamount)).ToRotationVector2() * length;
				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			}

			// Finding the closest NPC to attack within maxDetectDistance range
			// If not found then returns null
			public NPC FindClosestNPC(float maxDetectDistance) {
				NPC closestNPC = null;

				// Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
				float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

				// Loop through all NPCs
				foreach (var target in Main.ActiveNPCs) {
					// Check if NPC able to be targeted. 
					if (IsValidTarget(target)) {
						// The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
						float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

						// Check if it is within the radius
						if (sqrDistanceToTarget < sqrMaxDetectDistance) {
							sqrMaxDetectDistance = sqrDistanceToTarget;
							closestNPC = target;
						}
					}
				}

				return closestNPC;
			}

		public override void OnKill(int timeLeft)
		{
			for (int t = 0; t < 14; t++)
			{
				PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticle2>(), Projectile.Center, new Vector2(Main.rand.NextFloat(1, -1), -4), new Color(140, 234, 87), Main.rand.NextFloat(0.04f, 1.4f));
			}
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CausticAoE>(), 30, 0f, Projectile.owner);
			SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/CausticImpactSmall") with { PitchVariance = 2f, Volume = Main.rand.NextFloat(0.2f, 1.9f) }, Projectile.Center);	
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 240);
        }

			public bool IsValidTarget(NPC target)
        {
            // This method checks that the NPC is:
            // 1. active (alive)
            // 2. chaseable (e.g. not a cultist archer)
            // 3. max life bigger than 5 (e.g. not a critter)
            // 4. can take damage (e.g. moonlord core after all it's parts are downed)
            // 5. hostile (!friendly)
            // 6. not immortal (e.g. not a target dummy)
            // 7. doesn't have solid tiles blocking a line of sight between the projectile and NPC
            return target.CanBeChasedBy();
        }
			
    }
}