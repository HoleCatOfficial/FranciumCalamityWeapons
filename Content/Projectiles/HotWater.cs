using System;
using FranciumCalamityWeapons.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    public class HotWater : ModProjectile
    {
        public override string Texture => "FranciumCalamityWeapons/Content/Particles/ParticleDrawEntity";
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.4f, 0.4f, 0.4f);
            // Gravity effect
            Projectile.velocity.Y += 0.1f;

            int[] types1 = new int[]
            {
                PRTLoader.GetParticleID<Cloud1>(),
                PRTLoader.GetParticleID<Cloud2>(),
                PRTLoader.GetParticleID<Cloud3>()
            };

            
            PRTLoader.NewParticle(types1[Main.rand.Next(types1.Length)], Projectile.Center, new Vector2(0, -Math.Abs(Projectile.velocity.Y * 1.5f)), Color.White, Main.rand.NextFloat(6f, 16f));
            
            
            

            // Spawn dust
            for (int i = 0; i < 3; i++)
            {
                int dustIndex = Dust.NewDust(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.Water,
                    Projectile.velocity.X * 0.2f,
                    Projectile.velocity.Y * 0.2f,
                    100,
                    default,
                    1.2f
                );
                Main.dust[dustIndex].noGravity = true;
                Main.dust[dustIndex].scale = 0.8f + Main.rand.NextFloat() * 0.4f;
            }
        }
    }
}