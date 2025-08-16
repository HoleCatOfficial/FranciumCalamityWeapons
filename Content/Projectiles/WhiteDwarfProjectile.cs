
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles
{
	// This projectile showcases advanced AI code. Of particular note is a showcase on how projectiles can stick to NPCs in a manner similar to the behavior of vanilla weapons such as Bone Javelin, Daybreak, Blood Butcherer, Stardust Cell Minion, and Tentacle Spike. This code is modeled closely after Bone Javelin.
	public class WhiteDwarfProjectile : ModProjectile
	{
        public override string Texture => "FranciumCalamityWeapons/Content/Melee/WhiteDwarf";
		// These properties wrap the usual ai arrays for cleaner and easier to understand code.
		// Are we sticking to a target?



		public override void SetStaticDefaults() {

		}

		public override void SetDefaults() {
			Projectile.width = 32; // The width of projectile hitbox
			Projectile.height = 32; // The height of projectile hitbox
			Projectile.aiStyle = 0; // The ai style of the projectile (0 means custom AI). For more please reference the source code of Terraria
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Ranged; // Makes the projectile deal ranged damage. You can set in to DamageClass.Throwing, but that is not used by any vanilla items
			Projectile.penetrate = 2; // How many monsters the projectile can penetrate.
			Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.alpha = 0; // The transparency of the projectile, 255 for completely transparent. Our custom AI below fades our projectile in. Make sure to delete this if you aren't using an aiStyle that fades in.
			Projectile.light = 0.5f; // How much light emit around the projectile
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = true; // Can the projectile collide with tiles?
			Projectile.hide = false; // Makes the projectile completely invisible. We need this to draw our projectile behind enemies/tiles in DrawBehind()
		}

		private const int GravityDelay = 45;

		public override void AI() {
			//Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			// Offset the rotation by 90 degrees because the sprite is oriented vertically.
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);

			// Spawn some random dusts as the javelin travels
			if (Main.rand.NextBool(3)) {
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, DustID.Phantasmal, Projectile.velocity.X * .2f, Projectile.velocity.Y * .2f, 200, Scale: 1.2f);
				dust.velocity += Projectile.velocity * 0.3f;
				dust.velocity *= 0.2f;
			}
			if (Main.rand.NextBool(4)) {
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, DustID.Phantasmal,
					0, 0, 254, Scale: 0.3f);
				dust.velocity += Projectile.velocity * 0.5f;
				dust.velocity *= 0.5f;
			}
		}

		public override void OnKill(int timeLeft) {

			SoundEngine.PlaySound(SoundID.Shatter, Projectile.position); // Play a death sound
			Vector2 usePos = Projectile.position; // Position to use for dusts

			// Offset the rotation by 90 degrees because the sprite is oriented vertically.
			Vector2 rotationVector = (Projectile.rotation - MathHelper.ToRadians(90f)).ToRotationVector2(); // rotation vector to use for dust velocity
			usePos += rotationVector * 16f;

			// Spawn some dusts upon javelin death
			for (int i = 0; i < 20; i++) {
				// Create a new dust
				Dust dust = Dust.NewDustDirect(usePos, Projectile.width, Projectile.height, DustID.Tin);
				dust.position = (dust.position + Projectile.Center) / 2f;
				dust.velocity += rotationVector * 2f;
				dust.velocity *= 0.5f;
				dust.noGravity = true;
				usePos -= rotationVector * 8f;
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			if (target == null || !target.active)
        	return; // Exit if no valid target exists

			float radius = 300f; // Adjust as needed
			int numStars = 6; // Number of projectiles
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
					ModContent.ProjectileType<StarSkeleton>(),
					Projectile.damage,
					Projectile.knockBack,
					Projectile.owner
				);
			}
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
			// For going through platforms and such, javelins use a tad smaller size
			width = height = 10; // notice we set the width to the height, the height to 10. so both are 10
			return true;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			// By shrinking target hitboxes by a small amount, this projectile only hits if it more directly hits the target.
			// This helps the javelin stick in a visually appealing place within the target sprite.
			if (targetHitbox.Width > 8 && targetHitbox.Height > 8) {
				targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
			}
			// Return if the hitboxes intersects, which means the javelin collides or not
			return projHitbox.Intersects(targetHitbox);
		}

	
	}

	public class StarSkeleton : ModProjectile
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
				Projectile.width = 32; // The width of projectile hitbox
				Projectile.height = 32; // The height of projectile hitbox

				Projectile.DamageType = DamageClass.Melee; // What type of damage does this projectile affect?
				Projectile.friendly = true; // Can the projectile deal damage to enemies?
				Projectile.hostile = false; // Can the projectile deal damage to the player?
				Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
				Projectile.light = 1f; // How much light emit around the projectile
				Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
				Projectile.tileCollide = false;
			}

			// Custom AI
			public override void AI() {
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
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
				Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(300)).ToRotationVector2() * length;
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

			public bool IsValidTarget(NPC target) {
				// This method checks that the NPC is:
				// 1. active (alive)
				// 2. chaseable (e.g. not a cultist archer)
				// 3. max life bigger than 5 (e.g. not a critter)
				// 4. can take damage (e.g. moonlord core after all it's parts are downed)
				// 5. hostile (!friendly)
				// 6. not immortal (e.g. not a target dummy)
				// 7. doesn't have solid tiles blocking a line of sight between the projectile and NPC
				return target.CanBeChasedBy() && Collision.CanHit(Projectile.Center, 1, 1, target.position, target.width, target.height);
			}
			public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
				target.AddBuff(BuffID.Confused, 120);
			}

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.RainbowRodHit,
				new ParticleOrchestraSettings { PositionInWorld = Main.rand.NextVector2FromRectangle(target.Hitbox) },
				Projectile.owner);
        }
    }
}