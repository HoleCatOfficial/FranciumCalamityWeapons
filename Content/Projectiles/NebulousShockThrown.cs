using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using System;
using FranciumCalamityWeapons.Content.Melee;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    public class NebulousShockThrown : ModProjectile
    {
        private bool returning = false;
        private int flightTime = 0;

        public int HitCount = 0;

        private int soundCooldown = 0; // Initialize a cooldown timer

        public int existenceTimer = 0;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
        }

        public override void SetDefaults()
        {
            
            Projectile.friendly = true;
            Projectile.penetrate = -1; // Infinite pierce
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600; // 10 seconds max lifespan
            Projectile.width = 60;
            Projectile.height = 62;
            Projectile.tileCollide = false;
          
            
        }

        int trailLength = 10; // Adjust for desired effect
		public override bool PreDraw(ref Color lightColor)
			{
				// Set lightColor to a reddish hue and adjust its transparency based on the projectile's time left
				lightColor = Color.GhostWhite;
				if (Projectile.timeLeft < 30)
				{
					lightColor *= ((float)Projectile.timeLeft / 30f); // Fade out glow as projectile nears expiration
				}

				// Prepare for sprite drawing
				SpriteBatch spriteBatch = Main.spriteBatch;
				Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

				// End previous spriteBatch before starting new ones
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

				// Draw the main projectile
				spriteBatch.Draw(projectileTexture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, projectileTexture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

				// End the AlphaBlend draw and start the Additive blend for the glow effect
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

				// Draw the large colored glow
				Texture2D glowTexture = ModContent.Request<Texture2D>("FranciumCalamityWeapons/Content/Particles/Cyclone1").Value;
                if (returning)
                {
                    spriteBatch.Draw(glowTexture, Projectile.Center - Main.screenPosition, null, lightColor * 0.4f, Projectile.rotation, glowTexture.Size() / 2, 0.1f * Projectile.scale, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(glowTexture, Projectile.Center - Main.screenPosition, null, lightColor * 0.4f, Projectile.rotation, glowTexture.Size() / 2, 0.1f * Projectile.scale, SpriteEffects.FlipHorizontally, 0);
                }

                spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

				// Draw the large colored glow
				Texture2D glowTexture2 = ModContent.Request<Texture2D>("FranciumCalamityWeapons/Content/Particles/BloomRing").Value;
                    
                spriteBatch.Draw(glowTexture2, Projectile.Center - Main.screenPosition, null, lightColor * 0.4f, Projectile.rotation, glowTexture2.Size() / 2, 0.4f * Projectile.scale, SpriteEffects.None, 0);

				// Now, render the **low-opacity red TRAIL** (no white trail)
				Texture2D longtrailTexture = ModContent.Request<Texture2D>("FranciumCalamityWeapons/Content/Particles/Trail2").Value; 
				Vector2 trailOrigin = new Vector2(longtrailTexture.Width / 2, longtrailTexture.Height / 2);

				for (int i = 0; i < trailLength && i < Projectile.oldPos.Length; i++)
					{
						float fade = (float)(trailLength - i) / trailLength;

						// Make sure transparency blending is correct
						Color trailColor = lightColor * fade * 0.3f;
						trailColor.A = (byte)(fade * 100); // Instead of setting it to 0

						Vector2 drawPosition = Projectile.oldPos[i] + (Projectile.Size / 4) - Main.screenPosition;
						float scaleFactor = 0.3f; // Adjust the factor to make it smaller
						spriteBatch.Draw(longtrailTexture, drawPosition, null, trailColor, Projectile.rotation, trailOrigin, (Projectile.scale * fade) * scaleFactor, SpriteEffects.None, 0);
					}

				// Finalize drawing
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

				return false; // Prevents default projectile rendering
			}

        public override void AI()
        {
            existenceTimer++;

            // Check for duplicate projectiles
            Projectile youngestProjectile = null;
            int lowestExistenceTime = int.MaxValue;
            int count = 0;

            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == Projectile.type && proj.owner == Projectile.owner)
                {
                    count++;
                    if (proj.ModProjectile is NebulousShockThrown otherProj)
                    {
                        if (otherProj.existenceTimer < lowestExistenceTime)
                        {
                            lowestExistenceTime = otherProj.existenceTimer;
                            youngestProjectile = proj;
                        }
                    }
                }
            }

            // If more than one exists, kill the youngest (lowest existenceTimer)
            if (count > 1 && youngestProjectile != null && youngestProjectile == Projectile)
            {
                Projectile.Kill();
                return; // Exit early to prevent further execution
            }

            // Decrease the cooldown timer on each tick
            if (soundCooldown > 0)
            {
                soundCooldown--;
            }

            // Play the sound every 30 ticks
            if (soundCooldown <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item169);
                soundCooldown = 30; // Reset the cooldown to 30 ticks
            }
            
            

            
            Player player = Main.player[Projectile.owner];

          

            // Always spinning
            Projectile.rotation += 0.4f * Projectile.direction;

              // Generate flying dust effect
            if (Main.rand.NextBool(3)) // 33% chance per tick
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.BlueMoss, Projectile.velocity * 0.2f, 100, default, 1.2f);
                dust.noGravity = true;
                dust.fadeIn = 1.5f;
            }

            
            
            if (!returning)
            {
                // OutPhase: Count time before returning
                flightTime++;
                if (flightTime >= 60)
                    {
                        returning = true;
                    }
            }


            if (returning)
            {
                ArmCatchAnimate(player);
                // InPhase: Smooth return using Lerp
                Vector2 returnDirection = player.Center - Projectile.Center;
                float speed = MathHelper.Lerp(Projectile.velocity.Length(), 15f, 0.08f); // Smooth acceleration
                Projectile.velocity = returnDirection.SafeNormalize(Vector2.Zero) * speed;

                // If close enough, remove the projectile
                if (returnDirection.LengthSquared() < 16f) // 4 pixels radius
                {
                    HitCount = 0;
                    existenceTimer = 0;
                    Projectile.Kill();
                }
            }
        }

        public void ArmCatchAnimate(Player player)
        {
            // Calculate the direction vector from the player to the projectile
            Vector2 directionToProjectile = Projectile.Center - player.Center;

            // Normalize the direction vector to get a unit vector
            directionToProjectile.Normalize();

            // Calculate the angle between the player's direction and the direction to the projectile
            float angleDifference = MathHelper.WrapAngle(directionToProjectile.ToRotation() - player.direction * MathHelper.PiOver2);

            // Adjust arm rotation based on the player's facing direction
            if (player.direction == 1)
            {
                // Player is facing right, so we use the angle difference as is
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, angleDifference);
            }
            else if (player.direction == -1)
            {
                // Player is facing left, so flip the angle by pi (180 degrees) to reach the opposite direction
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, angleDifference + MathHelper.Pi);
            }
        }






        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (ModLoader.TryGetMod("DestroyerTest", out Mod DestroyerTest))
            {
                if (DestroyerTest.TryFind("HaepiensBlizzard", out ModBuff HB))
                    {
                    target.AddBuff(HB.Type, 120);
                    }
            }
            SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/IceImpact"));
            HitCount += 1;
            returning = true; // Immediately start returning when hitting something

            if (target == null || !target.active)
        	return; // Exit if no valid target exists

			float radius = 120f; // Adjust as needed
			int numStars = 7; // Number of projectiles
			for (int i = 0; i < numStars; i++)
			{
				float angle = MathHelper.TwoPi / numStars * i; // Distribute evenly in a circle
				Vector2 spawnPos = target.Center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
				Vector2 defaultmovement = new Vector2(0, -6);

				// Spawn the StarSkeleton projectiles at calculated positions
				Projectile.NewProjectile(
					Projectile.GetSource_FromThis(),
					spawnPos,
					defaultmovement, // No movement needed
					ModContent.ProjectileType<IceDartHoming>(),
					Projectile.damage,
					Projectile.knockBack,
					Projectile.owner
				);
			}
        }

    }
}

