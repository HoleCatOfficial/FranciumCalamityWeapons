
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Scepter
{
	// This example is similar to the Wooden Arrow projectile
	public class PearlChunk : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// If this arrow would have strong effects (like Holy Arrow pierce), we can make it fire fewer projectiles from Daedalus Stormbow for game balance considerations like this:
			//ProjectileID.Sets.FiresFewerFromDaedalusStormbow[Type] = true;
		}

		public override void SetDefaults() {
			Projectile.width = 20; // The width of projectile hitbox
			Projectile.height = 14; // The height of projectile hitbox
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Generic;
			Projectile.timeLeft = 1200;
		}

		public override void AI() {
            // Initialize random rotation speed if not already set
            if (Projectile.localAI[0] == 0f) {
                Projectile.localAI[0] = Main.rand.NextFloat(0.05f, 0.15f) * (Main.rand.NextBool() ? 1 : -1); // Random speed and direction
            }

            // Apply rotation
            Projectile.rotation += Projectile.localAI[0];

            // Adjust rotation speed when falling
            if (Projectile.velocity.Y > 0f) { // Falling down
                Projectile.rotation += Projectile.localAI[0] * 0.5f; // Apply a multiplier to slow down rotation
            }

            // Gravity effect
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 5f) {
                Projectile.ai[0] = 5f;
                Projectile.velocity.Y += 0.1f;
            }

            // Cap falling speed
            if (Projectile.velocity.Y > 16f) {
                Projectile.velocity.Y = 16f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			return true;
        }

		public override void OnKill(int timeLeft) {

			SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt, Projectile.position); // Plays the basic sound most projectiles make when hitting blocks.
			//for (int i = 0; i < 5; i++) // Creates a splash of dust around the position the projectile dies.
			//{
				//Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Blood);
				//dust.noGravity = true;
				//dust.velocity *= 1.5f;
				//dust.scale *= 0.9f;
			//} 
		}
	}
}