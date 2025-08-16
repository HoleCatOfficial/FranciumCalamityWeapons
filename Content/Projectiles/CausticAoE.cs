using System;
using CalamityMod.Buffs.StatDebuffs;
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
    public class CausticAoE : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.hide = true; // Optional: hide if just visual
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
        }

        public bool DidTheThing = false;
        public override void AI()
        {
            if (DidTheThing == false)
            {
                PRTLoader.NewParticle(PRTLoader.GetParticleID<Boom3>(), Projectile.Center, Vector2.Zero, new Color(140, 234, 87), 2f);
                for (int i = 0; i < 5; i++)
                {
                    PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticle2>(), Projectile.Center, new Vector2(Main.rand.NextFloat(1, -1), -4), new Color(140, 234, 87), Main.rand.NextFloat(0.04f, 1.4f));
                }
                DidTheThing = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 600);
        }
    }
}