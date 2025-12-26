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

            PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], Projectile.Center, Vector2.Zero, Color.Black, 1, 60, ai2: 1);

			if (Main.rand.NextBool(3))
			{
				PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], Projectile.Center, Vector2.Zero, DTColorUtils.Pastel(Main.DiscoColor, 0.4f), 1, 60, ai2: 1);
			}

        }
        


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return true;
        }

		
	}
}