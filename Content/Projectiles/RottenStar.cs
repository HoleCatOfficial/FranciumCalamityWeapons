using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BreadLibrary.Core.Graphics.Particles;
using CalamityMod.Buffs.DamageOverTime;
using DestroyerTest.Common;
using DestroyerTest.Common.Interfaces;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.Particles;
using FranciumCalamityWeapons.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    public class RottenStar : ModProjectile, IHomingProjectile
    {

        public override string Texture => DTUtils.NoTexture;

        bool IHomingProjectile.TracksNPCs => true;

        bool IHomingProjectile.TracksPlayers => false;

        float IHomingProjectile.HomingTurnSpeed => 5;

        bool IHomingProjectile.UsesHomingAcceleration => true;

        float IHomingProjectile.HomingAccelAmount => 1.004f;

        float IHomingProjectile.HomingMaxAccel => 40f;

        float IHomingProjectile.DetectRadius => 2800;

        bool IHomingProjectile.CanHome => DelayTimer >= 30 && DelayTimer < 540;

        public float DelayTimer;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
            ProjectileID.Sets.TrailCacheLength[Type] = 160;
            ProjectileID.Sets.TrailingMode[Type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;

            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
        }

        public float trailOffset = 0f;
        public override bool PreDraw(ref Color lightColor)
        {
            trailOffset += 0.04f;


            SpriteBatch spriteBatch = Main.spriteBatch;

            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(6, true).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 15, Color.White with { A = 0 }, trailOffset);

            Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, Color.White with { A = 0 }, true, 0f, 0.9f, 0.9f);

            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 30 && Projectile.ManualCanHitFriendly(target);
        }

        public override void AI()
        {
            Projectile.ResetExcessTrailPoints();

            DelayTimer++;

            Projectile.rotation += Projectile.direction * 0.07f;

            Color[] HalfOpacityMap = DTUtilsCalamity.UC_Colormap.Select(n =>
            {
                n.R = (byte)(n.R * 0.08f);
                n.G = (byte)(n.G * 0.08f);
                n.B = (byte)(n.B * 0.08f);
                n.A = (byte)(n.A * 0.08f);
                return n;
            }
            ).ToArray();

            PointGlowPreMultiplied Glow = new();
            Glow.Initialize(Projectile.Center, Projectile.velocity * 0.1f, DTUtilsCalamity.UC_Colormap[3] * 0.5f, 1.5f, 30);
            ParticleEngine.BehindProjectiles.Add(Glow);

            LerpingFire Fire = new();
            Fire.PrepareFire(Projectile.Center, Projectile.velocity * 0.1f, DTUtils.RandomDirection(2), 0.01f, HalfOpacityMap, 0.7f, 30, FireDrawMode.Additive);
            ParticleEngine.BehindProjectiles.Add(Fire);

            LerpingFire Fire2 = new();
            Fire2.PrepareFire(Projectile.Center, Projectile.velocity * 0.1f, DTUtils.RandomDirection(2), 0.01f, HalfOpacityMap, 0.25f, 30, FireDrawMode.Additive);
            ParticleEngine.BehindProjectiles.Add(Fire2);

            Lighting.AddLight(Projectile.Center, ColorLib.TenebrisGradient.ToVector3() * 0.2f);

            if (DelayTimer < 30 || DelayTimer > 540)
            {
                return;
            }

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 600);
            target.AddBuff(ModContent.BuffType<Plague>(), 600);
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 600);
            target.AddBuff(ModContent.BuffType<Defilement>(), 600);
            target.AddBuff(ModContent.BuffType<Withering>(), 600);
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                LerpingFire Fire = new();
                Fire.PrepareFire(Projectile.Center, Main.rand.NextVector2Circular(8, 8), DTUtils.RandomDirection(2), 0.01f, DTUtilsCalamity.UC_Colormap, 1f, 120, FireDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(Fire);
            }
        }

    }
}
