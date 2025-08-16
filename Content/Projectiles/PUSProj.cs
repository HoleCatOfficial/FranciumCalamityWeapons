using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using FranciumCalamityWeapons.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles
{
	// This example is similar to the Wooden Arrow projectile
	public class PUSProj : ModProjectile
	{
        
		public override void SetStaticDefaults()
		{
			// If this arrow would have strong effects (like Holy Arrow pierce), we can make it fire fewer projectiles from Daedalus Stormbow for game balance considerations like this:
			//ProjectileID.Sets.FiresFewerFromDaedalusStormbow[Type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 102; // The width of projectile hitbox
			Projectile.height = 102; // The height of projectile hitbox
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 1200;
			Projectile.penetrate = -1;
			Projectile.netImportant = true;
			Projectile.netUpdate = true;
		}



        public override void AI()
        {
            // The code below was adapted from the ProjAIStyleID.Arrow behavior. Rather than copy an existing aiStyle using Projectile.aiStyle and AIType,
            // like some examples do, this example has custom AI code that is better suited for modifying directly.
            // See https://github.com/tModLoader/tModLoader/wiki/Basic-Projectile#what-is-ai for more information on custom projectile AI.

            // The projectile is rotated to face the direction of travel
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

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

            PRTLoader.NewParticle(types1[Main.rand.Next(types1.Length)], Projectile.Center + new Vector2(-80, 80).RotatedBy(Projectile.rotation), Vector2.Zero, default, 0.3f);
            if (Main.rand.NextBool(3))
			{// 1 in 3 chance to spawn a streak
			 // Make the color more pastel by blending with white
				Color baseColor = new Color(Main.DiscoR / 2, (byte)(Main.DiscoG / 1.25f), (byte)(Main.DiscoB / 1.5f));
				Color pastelColor = Color.Lerp(baseColor, Color.White, 0.5f); // 0.5f blends halfway to white
				PRTLoader.NewParticle(types2[Main.rand.Next(types2.Length)], Projectile.Center + new Vector2(-80, 80).RotatedBy(Projectile.rotation), Vector2.Zero, pastelColor, 0.1f);
			}

		}

		public override void OnKill(int timeLeft) {
			
			
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			SoundEngine.PlaySound(SoundID.Item65, Projectile.position); // Plays the basic sound most projectiles make when hitting blocks.
            Color baseColor = new Color(Main.DiscoR / 2, (byte)(Main.DiscoG / 1.25f), (byte)(Main.DiscoB / 1.5f));
			Color pastelColor = Color.Lerp(baseColor, Color.White, 0.5f);
            PRTLoader.NewParticle(PRTLoader.GetParticleID<Boom2>(), Projectile.Center, Vector2.Zero, pastelColor, 1);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Color baseColor = new Color(Main.DiscoR / 2, (byte)(Main.DiscoG / 1.25f), (byte)(Main.DiscoB / 1.5f));
			Color pastelColor = Color.Lerp(baseColor, Color.White, 0.5f);
            PRTLoader.NewParticle(PRTLoader.GetParticleID<Boom2>(), Projectile.Center, Vector2.Zero, pastelColor, 1);
            for (int i = 0; i < 5; i++)
            {
                float angle = MathHelper.TwoPi / 5 * i;
                Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 8f;
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    velocity,
                    ModContent.ProjectileType<PUSFeather>(),
                    100,
                    3,
                    Projectile.owner
                );
            }
			return true;
		}
	}
}