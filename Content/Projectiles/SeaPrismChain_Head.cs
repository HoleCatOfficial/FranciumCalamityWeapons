using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    public class SeaPrismChain_Head : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }

        public override void SetDefaults()
        {
            Projectile.width = 30; // Width of the projectile hitbox
            Projectile.height = 32; // Height of the projectile hitbox
            Projectile.aiStyle = -1;
            Projectile.friendly = true; // Can damage enemies
            Projectile.hostile = false; // Does not damage players
            Projectile.penetrate = -1; // Infinite penetration
            Projectile.timeLeft = 140; // Lifetime of the projectile in ticks
            Projectile.ignoreWater = true; // Ignores water physics
            Projectile.tileCollide = false; // Does not collide with tiles
            Projectile.DamageType = DamageClass.Magic; // Set the damage type
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // Fade out the chain when timeLeft falls below 120
            if (Projectile.timeLeft < 120)
            {
                Projectile.alpha += 2; // Increase alpha to fade out (max value is 255)
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255; // Clamp alpha to 255
                }
            }
            //Projectile.rotation = Projectile.ai[0];
            // Copy behavior similar to CrystalVialShardShaft
            // You can add custom AI logic here if needed
            Lighting.AddLight(Projectile.Center, 0.2f, 0.2f, 0.2f); // Emits light (customize RGB values)
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 120);
        }
    }
}