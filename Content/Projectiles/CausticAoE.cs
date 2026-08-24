using System;
using CalamityMod.Buffs.StatDebuffs;
using DestroyerTest.Content.Particles;
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
                DidTheThing = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 600);
        }
    }
}