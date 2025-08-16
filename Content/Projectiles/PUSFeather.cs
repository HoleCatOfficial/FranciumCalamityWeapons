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
	public class PUSFeather : ModProjectile
	{
        public override void SetStaticDefaults()
        {
           

		}
        
    
        public override void SetDefaults()
        {
            Projectile.width = 14; // The width of projectile hitbox
            Projectile.height = 34; // The height of projectile hitbox
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.timeLeft = 1200;
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.penetrate = -1;
        }
       

        public override void AI()
        {

          

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

            PRTLoader.NewParticle(types1[Main.rand.Next(types1.Length)], Projectile.Center + new Vector2(0, 40).RotatedBy(Projectile.rotation), Vector2.Zero, default, 0.3f);
            if (Main.rand.NextBool(3))
			{// 1 in 3 chance to spawn a streak
			 // Make the color more pastel by blending with white
				Color baseColor = new Color(Main.DiscoR / 2, (byte)(Main.DiscoG / 1.25f), (byte)(Main.DiscoB / 1.5f));
				Color pastelColor = Color.Lerp(baseColor, Color.White, 0.5f); // 0.5f blends halfway to white
				PRTLoader.NewParticle(types2[Main.rand.Next(types2.Length)], Projectile.Center + new Vector2(0, 40).RotatedBy(Projectile.rotation), Vector2.Zero, pastelColor, 0.1f);
			}

        }
        


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return true;
        }

		
	}
}