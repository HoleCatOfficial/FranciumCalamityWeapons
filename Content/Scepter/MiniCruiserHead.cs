using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using FranciumCalamityWeapons.Content.Particles;
using System;
using CalamityMod;
using Terraria.Graphics.Shaders;
using CalamityMod.Graphics.Primitives;

namespace FranciumCalamityWeapons.Content.Scepter
{
	// This Example show how to implement simple homing projectile
	// Can be tested with ExampleCustomAmmoGun
	public class MiniCruiserHead : ModProjectile
	{
		// Store the target NPC using Projectile.ai[0]
		private NPC HomingTarget {
			get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
			set {
				Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
			}
		}

		public ref float DelayTimer => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
            Main.projFrames[Projectile.type] = 4;
        }

		public override void SetDefaults()
		{
			Projectile.width = 74; // The width of projectile hitbox
			Projectile.height = 80; // The height of projectile hitbox

			Projectile.DamageType = DamageClass.Generic; // What type of damage does this projectile affect?
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 1f; // How much light emit around the projectile
			Projectile.timeLeft = 240; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.tileCollide = false;
			Projectile.netImportant = true;
			Projectile.netUpdate = true;
		}
        
        public void DrawConnectorLine(Vector2 start, Vector2 end, Texture2D texture, Color color)
        {
            Vector2 direction = end - start;
            float length = direction.Length();
            direction.Normalize();
            texture ??= TextureAssets.MagicPixel.Value;
            SpriteBatch spriteBatch = Main.spriteBatch;

            float rotation = direction.ToRotation();

            // Assuming your texture is a chain segment, like 16px long
            float segmentLength = texture.Height * 0.75f; // or Width, depending on the texture orientation
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (float i = 0; i < length; i += segmentLength)
            {
                Vector2 position = start + direction * i;

                Main.spriteBatch.Draw(
                    texture,
                    position - Main.screenPosition,
                    null,
                    color,
                    rotation + MathHelper.PiOver2, // Adjust if your texture points upward
                    new Vector2(texture.Width / 2f, texture.Height / 2f), // Origin at center
                    1f, // Scale
                    SpriteEffects.None,
                    0f
                );
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
        
		public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            lightColor = Color.Lerp(ExarchicEnforcerThrown.VoidBarColors.Colors[3], ExarchicEnforcerThrown.VoidBarColors.Colors[4], 0.5f);
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

            Texture2D pixel = Terraria.GameContent.TextureAssets.MagicPixel.Value;

            //DrawConnectorLine(Projectile.Center, player.Center, ModContent.Request<Texture2D>("FranciumCalamityWeapons/Content/Particles/SpiritTrail").Value, lightColor);

            for (int i = 0; i < TrailPositions.Count - 1; i++)
            {
                Vector2 start = TrailPositions[i] - Main.screenPosition;
                Vector2 end = TrailPositions[i + 1] - Main.screenPosition;
                Vector2 diff = end - start;

                float length = diff.Length();
                if (length < 0.5f)
                    continue; // skip tiny wiggle segments

                float rotation = diff.ToRotation();

                float width = MathHelper.Lerp(0.01f, 0.0007f, i / (float)TrailLength);
                float alpha = MathHelper.Lerp(1f, 0f, i / (float)TrailLength);
                Color color = lightColor * alpha;

                // Instead of stepping pixel by pixel, just draw one scaled pixel segment:
                Main.spriteBatch.Draw(
                    pixel,
                    start,
                    null,
                    color,
                    rotation,
                    new Vector2(pixel.Width / 2, pixel.Height / 2), // Origin is at the left-middle of the scaled pixel
                    new Vector2(length, width),
                    SpriteEffects.None,
                    0f
                );
            }
            return true; // Let the default system handle the base projectile drawing
        }

        private void AnimateProjectile() {
            // Loop through the frames, assuming each frame lasts 5 ticks
            if (++Projectile.frameCounter >= 5) {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type]) {
                    Projectile.frame = 0;
                }
            }
        }

		public List<Vector2> TrailPositions = new();
        public List<float> TrailRotations = new();
        private const int TrailLength = 40;// Custom AI
		public override void AI() {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			Lighting.AddLight(Projectile.Center, ExarchicEnforcerThrown.VoidBarColors.Colors[4].ToVector3() * 1.0f);
            AnimateProjectile();

			 // Add center & rotation
            TrailPositions.Insert(0, Projectile.Center);
            TrailRotations.Insert(0, Projectile.rotation);

            // Cap trail
            while (TrailPositions.Count > TrailLength)
                TrailPositions.RemoveAt(TrailPositions.Count - 1);
            while (TrailRotations.Count > TrailLength)
                TrailRotations.RemoveAt(TrailRotations.Count - 1);

            //PRTLoader.NewParticle(PRTLoader.GetParticleID<StarParticle>(), Projectile.Center, Vector2.Zero, Color.Red, 0.5f);

            if (DelayTimer < 20)
                    {
                        DelayTimer += 1;
                        return;
                    }
		
			float maxDetectRadius = 1400f; // The maximum radius at which a projectile can detect a target

            if (player.channel == true)
            {
                Projectile.timeLeft = 240;
                Vector2 toMouse = Main.MouseWorld - Projectile.Center;
                if (toMouse.Length() > 48f)
                {
                    toMouse.Normalize();
                    toMouse *= 1f; // acceleration speed

                    Projectile.velocity += toMouse;
                    if (Projectile.velocity.Length() > 24f) // cap the speed
                        Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 24f;

                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }
                else
                {
                    Vector2 direction = Main.MouseWorld - Projectile.Center;
                    Projectile.velocity = direction * 5; // or direction * some small factor
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }
            }

            if (player.channel == false)
                {

                    // First, we find a homing target if we don't have one
                    if (HomingTarget == null)
                    {
                        HomingTarget = FindClosestNPC(maxDetectRadius);
                    }

                    // If we have a homing target, make sure it is still valid. If the NPC dies or moves away, we'll want to find a new target
                    if (HomingTarget != null && !IsValidTarget(HomingTarget))
                    {
                        HomingTarget = null;
                    }

                    // If we don't have a target, don't adjust trajectory
                    if (HomingTarget == null)
                        return;

                    // If found, we rotate the projectile velocity in the direction of the target.
                    // We only rotate by 3 degrees an update to give it a smooth trajectory. Increase the rotation speed here to make tighter turns
                    float length = Projectile.velocity.Length();
                    float targetAngle = Projectile.AngleTo(HomingTarget.Center);
                    Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(12)).ToRotationVector2() * length;
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }
			
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

		public bool IsValidTarget(NPC target) {
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
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.player[Main.myPlayer];
			//SoundEngine.PlaySound(SoundID.Item88, target.Center);
			PRTLoader.NewParticle(PRTLoader.GetParticleID<BloomRingSharp2>(), target.Center, Vector2.Zero, ExarchicEnforcerThrown.VoidBarColors.Colors[4], 1f);
		}
	}
}