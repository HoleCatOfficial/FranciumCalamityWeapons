using System.Collections.Generic;
using System.IO;
using DestroyerTest.Common;
using FranciumCalamityWeapons.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace FranciumCalamityWeapons.Content.Projectiles
{
	// This example is similar to the Wooden Arrow projectile
	public class IsolationArrowProjectile : ModProjectile
	{
        public override void SetStaticDefaults()
        {
            // If this arrow would have strong effects (like Holy Arrow pierce), we can make it fire fewer projectiles from Daedalus Stormbow for game balance considerations like this:
            //ProjectileID.Sets.FiresFewerFromDaedalusStormbow[Type] = true;
            Main.projFrames[Type] = 8;
		}
        
        private void AnimateProjectile() {
            // Loop through the frames, assuming each frame lasts 5 ticks
            if (++Projectile.frameCounter >= 2) {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type]) {
                    Projectile.frame = 0;
                }
            }
        }

        public override void SetDefaults()
        {
            Projectile.width = 22; // The width of projectile hitbox
            Projectile.height = 52; // The height of projectile hitbox
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 1200;
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.penetrate = -1;
        }
       

        public override void AI()
        {
            AnimateProjectile();

          

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

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

            PRTLoader.NewParticle(types1[Main.rand.Next(types1.Length)], Projectile.Center + new Vector2(0, 40).RotatedBy(Projectile.rotation), Vector2.Zero, default, 0.3f);
            

        }
        


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return true;
        }

		public override void OnKill(int timeLeft) {
			PRTLoader.NewParticle(PRTLoader.GetParticleID<Boom2>(), Projectile.Center, Vector2.Zero, Color.White, 1);
		}
	}
}