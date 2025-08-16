using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using FranciumCalamityWeapons.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles
{
	public class NightFlameProjectile : ModProjectile
	{
		public override string Texture => "FranciumCalamityWeapons/Content/Particles/ParticleDrawEntity";

		public override void SetDefaults()
		{
			Projectile.width = 80; // The width of projectile hitbox
			Projectile.height = 80; // The height of projectile hitbox

			Projectile.DamageType = DamageClass.Generic;
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 1f; // How much light emit around the projectile
			Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.tileCollide = false;
			Projectile.alpha = 160;
			Projectile.netImportant = true;
			Projectile.netUpdate = true;
			Projectile.penetrate = 3;
		}
		
		private NPC HomingTarget {
			get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
			set {
				Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
			}
		}

		public override void AI()
		{
			Vector2 RandPos = Projectile.Center + Main.rand.NextVector2Circular(100, 100);

			Vector2 StarOffsetUp = Projectile.Center + new Vector2(0, -50).RotatedBy(Projectile.rotation);
			Vector2 StarOffsetDown = Projectile.Center + new Vector2(0, 50).RotatedBy(Projectile.rotation);

			
			float sine = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f); // -1 to 1
			float t = (sine + 1f) / 2f; // 0 to 1
			Vector2 StarOffset = Vector2.Lerp(StarOffsetUp, StarOffsetDown, t);


			int[] types1 = new int[]
			{
				PRTLoader.GetParticleID<BlackFire1>(),
				PRTLoader.GetParticleID<BlackFire2>(),
				PRTLoader.GetParticleID<BlackFire3>(),
				PRTLoader.GetParticleID<BlackFire4>(),
				PRTLoader.GetParticleID<BlackFire5>(),
				PRTLoader.GetParticleID<BlackFire6>(),
				PRTLoader.GetParticleID<BlackFire7>()
			};

			int[] types2 = new int[]
			{
				PRTLoader.GetParticleID<ColoredFire1>(),
				PRTLoader.GetParticleID<ColoredFire2>(),
				PRTLoader.GetParticleID<ColoredFire3>(),
				PRTLoader.GetParticleID<ColoredFire4>(),
				PRTLoader.GetParticleID<ColoredFire5>(),
				PRTLoader.GetParticleID<ColoredFire6>(),
				PRTLoader.GetParticleID<ColoredFire7>()
			};

			PRTLoader.NewParticle(types1[Main.rand.Next(types1.Length)], Projectile.Center, Vector2.Zero, default, 1);

			if (Main.rand.NextBool(3))
			{// 1 in 3 chance to spawn a streak
			 // Make the color more pastel by blending with white
				Color baseColor = new Color(Main.DiscoR / 2, (byte)(Main.DiscoG / 1.25f), (byte)(Main.DiscoB / 1.5f));
				Color pastelColor = Color.Lerp(baseColor, Color.White, 0.5f); // 0.5f blends halfway to white
				PRTLoader.NewParticle(types2[Main.rand.Next(types2.Length)], Projectile.Center, Vector2.Zero, pastelColor, 1);
				PRTLoader.NewParticle(PRTLoader.GetParticleID<StarParticle>(), StarOffset, Vector2.Zero, pastelColor, 1);
			}

			PRTLoader.NewParticle(PRTLoader.GetParticleID<Ember>(), RandPos, Vector2.Zero, new Color(Main.DiscoR / 2, (byte)(Main.DiscoG / 1.25f), (byte)(Main.DiscoB / 1.5f)), 1);

			// Fade out the projectile over time	
			float maxDetectRadius = 400f; // The maximum radius at which a projectile can detect a target

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
			Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(3)).ToRotationVector2() * length;
			Projectile.rotation = Projectile.velocity.ToRotation();
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
			{
				if (calamityMod.TryFind("GodSlayerInferno", out ModBuff GSI))
				{
					target.AddBuff(GSI.Type, 480);
				}
			}
		}
	}

	
}