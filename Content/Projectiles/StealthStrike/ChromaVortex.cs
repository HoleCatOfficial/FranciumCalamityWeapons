using CalamityMod;
using DestroyerTest.Content.RogueItems;
using FranciumCalamityWeapons.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles.StealthStrike
{
    // This projectile showcases advanced AI code. Of particular note is a showcase on how projectiles can stick to NPCs in a manner similar to the behavior of vanilla weapons such as Bone Javelin, Daybreak, Blood Butcherer, Stardust Cell Minion, and Tentacle Spike. This code is modeled closely after Bone Javelin.
    public class ChromaVortex : ModProjectile
    {

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 140; // The width of projectile hitbox
            Projectile.height = 140; // The height of projectile hitbox
            Projectile.friendly = true; // Can the projectile deal damage to enemies?
            Projectile.hostile = false; // Can the projectile deal damage to the player?
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>(); // Makes the projectile deal ranged damage. You can set in to DamageClass.Throwing, but that is not used by any vanilla items
            Projectile.penetrate = -1; // How many monsters the projectile can penetrate.
            Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)

            Projectile.light = 0.5f; // How much light emit around the projectile
            Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
            Projectile.tileCollide = false; // Can the projectile collide with tiles?
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
        }

        private const int GravityDelay = 45;

        public override void AI()
        {

            // Run either the Sticky AI or Normal AI
            // Separating into different methods helps keeps your AI clean
            NormalAI();

        }

        private void NormalAI()
        {
            // Offset the rotation by 90 degrees because the sprite is oriented vertically.
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);

            // Spawn some random dusts as the javelin travels
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, DustID.Vortex, Projectile.velocity.X * .2f, Projectile.velocity.Y * .2f, 400, Scale: 1.2f);
                dust.velocity += Projectile.velocity * 0.3f;
                dust.velocity *= 0.2f;
            }
            if (Main.rand.NextBool(4))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, DustID.Vortex, 0, 0, 200, Scale: 0.3f);
                dust.velocity += Projectile.velocity * 0.5f;
                dust.velocity *= 0.5f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.VortexDebuff, 260);
            for (int H = 0; H < 4; H++)
            {
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticle>(), target.Center, new Vector2(Main.rand.NextFloat(-2, 2), -6), new Color(0, 242, 170), 1);
            }
        }

    }
}