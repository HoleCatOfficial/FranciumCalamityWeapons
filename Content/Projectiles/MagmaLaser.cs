using FranciumCalamityWeapons.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles
{

    public class MagmaLaser : ModProjectile
    {

        public override string Texture => "FranciumCalamityWeapons/Content/Projectiles/MagmaLaser";


        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4; // Set the number of frames in the sprite sheet
        }

        public override void SetDefaults()
        {
            Projectile.width = 16; // The width of projectile hitbox
            Projectile.height = 16; // The height of projectile hitbox
            Projectile.DamageType = DamageClass.Magic; // What type of damage does this projectile affect?
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.light = 1f; // How much light emit around the projectile
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.penetrate = 1;
        }

        public override void OnKill(int timeLeft)
        {

            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.TintableDustLighted, 0, 0, 0, new Color(224, 108, 29), 1f);
        }
        // Custom AI
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.4f, 0.4f, 0.4f);
            AnimateProjectile();
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        private void AnimateProjectile()
        {
            // Loop through the frames, assuming each frame lasts 5 ticks
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 600);
        }
    }
}