using CalamityMod.Buffs.StatDebuffs;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Boss.NodeBoss.Ichor;
using FranciumCalamityWeapons.Common;
using FranciumCalamityWeapons.Content.Particles;
using InnoVault.PRT;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Peripherals.RGB;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    public class ColossusPhantom : ModProjectile
    {
        private enum AIState
        {
            Slowing,
            Dashing
        }

        private AIState State
        {
            get => (AIState)(int)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        private ref float Timer => ref Projectile.ai[1];

        public SoundStyle Hit = new SoundStyle("CalamityMod/Sounds/Item/HellkiteSmallHit", 3) with { Volume = 0.6f };

        public override void SetDefaults()
        {
            Projectile.width = 122;
            Projectile.height = 122;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 80;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Opus.DrawProjectileShadowsRotating(Projectile, 4, DTUtilsCalamity.DeuxiemeColor, Opacity: 0.35f);
            var T = TextureAssets.Projectile[Projectile.type].Value;

            Main.EntitySpriteDraw(T, Projectile.Center, null, DTUtilsCalamity.DeuxiemeColor, Projectile.rotation, T.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void AI()
        {
            NPC target = FindClosestNPC();
            Timer++;
            PRTLoader.NewParticle(PRTLoader.GetParticleID<DeuxiemeParticle2>(), Projectile.Center, Projectile.velocity * 0.15f * Main.rand.NextVector2Circular(3, 3), DTUtilsCalamity.DeuxiemeColor, 1f);

            switch (State)
            {
                case AIState.Slowing:
                    DoSlowingPhase(target);
                    break;
                case AIState.Dashing:
                    DoDashingPhase(target);
                    break;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return State == AIState.Dashing;
        }

        private void DoSlowingPhase(NPC target)
        {
            Projectile.rotation += Projectile.direction * Projectile.velocity.Length() * 0.1f;
            Projectile.velocity *= 0.96f;
            Projectile.timeLeft = 80;

            if (Projectile.velocity.Length() < 1f || Timer > 60f)
            {
                Timer = 0f;
                State = AIState.Dashing;
            }
        }

        private void DoDashingPhase(NPC target)
        {
            if (target == null || !target.active)
            {
                Projectile.Kill();
                return;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), Projectile.Center, Projectile.velocity * 0.15f, DTUtilsCalamity.DeuxiemeColor, 1f);

            if (Timer == 1f) // first tick of dashing phase
            {

                Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.velocity = direction * 50f;
                Projectile.netUpdate = true;
            }
        }

        private NPC FindClosestNPC()
        {
            NPC closest = null;
            float minDistance = float.MaxValue;

            foreach (NPC n in Main.npc)
            {
                if (n.active)
                {
                    float dist = Vector2.Distance(n.Center, Projectile.Center);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        closest = n;
                    }
                }
            }

            return closest;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(Hit);
            target.AddBuff(ModContent.BuffType<ArmorCrunch>(), 240);

            State = AIState.Slowing;
        }
    }
}