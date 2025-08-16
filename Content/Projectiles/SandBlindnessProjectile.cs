using System;
using CalamityMod.Buffs.StatDebuffs;
using FranciumCalamityWeapons.Content.Debuffs;
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
    public class SandBlindnessProjectile : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.hide = true; // Optional: hide if just visual
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
        }
        public override void AI()
        {
            for (int t = 0; t < 4; t++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sandnado, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100, default, 2f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<SandBlindness>(), 600);
        }
    }
}