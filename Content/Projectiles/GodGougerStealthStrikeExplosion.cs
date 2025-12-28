using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using FranciumCalamityWeapons.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    public class GodGougerStealthStrikeExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 13;
        }
        public override void SetDefaults()
        {
            Projectile.width = 118;
            Projectile.height = 118;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        private void AnimateProjectile() {
            // Loop through the frames, assuming each frame lasts 5 ticks
            if (++Projectile.frameCounter >= 60 / 13) {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type]) {
                    Projectile.Kill();
                }
            }
        }

        public bool DidTheThing = false;
        public override void AI()
        {
            AnimateProjectile();
            if (DidTheThing == false)
            {
                Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), Projectile.Center, Vector2.Zero, Color.PaleTurquoise, 0.01f, 0.75f);

                SoundEngine.PlaySound(DTAssetLib.Impacts.IceImpact, Projectile.Center);
                Opus.RadialDustRandomDir(DustID.TintableDustLighted, 8, Projectile.Center, 0, Color.Pink, 1f, 2.4f);
                Opus.RadialDustRandomDir(DustID.TintableDustLighted, 8, Projectile.Center, 0, Color.PaleTurquoise, 1f, 2.4f);
                DidTheThing = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BrainRot>(), 600);
        }
    }
}