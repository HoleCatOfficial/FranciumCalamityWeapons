using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles
{
public class TenebrousChakramIdleScythe : ModProjectile
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
			}

			public override void SetDefaults() {
				Projectile.width = 48; // The width of projectile hitbox
				Projectile.height = 32; // The height of projectile hitbox

				Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>(); // What type of damage does this projectile affect?
				Projectile.friendly = true; // Can the projectile deal damage to enemies?
				Projectile.hostile = false; // Can the projectile deal damage to the player?
				Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
				Projectile.light = 1f; // How much light emit around the projectile
				Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
				Projectile.tileCollide = false;
			}

            // Custom AI
            public override void AI() {
                float maxDetectRadius = 400f; // The maximum radius at which a projectile can detect a target
                float minSpeed = 1f; // Minimum speed when idle
                float maxSpeed = 16f; // Maximum speed when homing
                float slowDownRate = 0.96f; // How quickly to slow down (closer to 1 = slower)
                float speedUpRate = 0.15f; // How quickly to speed up when homing (closer to 1 = faster)

                // First, we find a homing target if we don't have one
                if (HomingTarget == null) {
                    HomingTarget = FindClosestNPC(maxDetectRadius);
                }

                // If we have a homing target, make sure it is still valid. If the NPC dies or moves away, we'll want to find a new target
                if (HomingTarget != null && !IsValidTarget(HomingTarget)) {
                    HomingTarget = null;
                }

                if (HomingTarget == null) {
                    // No target: gradually slow down to a stop (but not below minSpeed)
                    if (Projectile.velocity.Length() > minSpeed) {
                        Projectile.velocity *= slowDownRate;
                        if (Projectile.velocity.Length() < minSpeed) {
                            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * minSpeed;
                        }
                    }
                    else if (Projectile.velocity.Length() < minSpeed) {
                        Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * minSpeed;
                    }
                    return;
                }

                // If found, speed up and home in on the target
                Vector2 toTarget = HomingTarget.Center - Projectile.Center;
                float desiredSpeed = MathHelper.Lerp(Projectile.velocity.Length(), maxSpeed, speedUpRate);
                Vector2 desiredVelocity = toTarget.SafeNormalize(Vector2.Zero) * desiredSpeed;

                // Smoothly rotate velocity toward the target
                float targetAngle = toTarget.ToRotation();
                float currentAngle = Projectile.velocity.ToRotation();
                float newAngle = currentAngle.AngleTowards(targetAngle, MathHelper.ToRadians(10));
                Projectile.velocity = newAngle.ToRotationVector2() * desiredSpeed;
                Projectile.rotation = 0.6f * Projectile.direction;
                
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
    }
}