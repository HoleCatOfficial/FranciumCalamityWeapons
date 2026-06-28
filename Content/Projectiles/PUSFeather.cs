using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using FranciumCalamityWeapons.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Particles;
using System.Collections.Generic;
using System.IO;
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

            LerpingFire fire = new LerpingFire();
            fire.PrepareFire(Projectile.Center, Vector2.Zero, DTUtils.RandomDirection(2), 0.2f, Color.Indigo, Color.Navy, 1f, 100, FireDrawMode.Additive);
            ParticleEngine.BehindProjectiles.Add(fire);

        }
        


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return true;
        }

		
	}
}