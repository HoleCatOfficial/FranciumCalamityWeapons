using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.player.Accessory;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    public class ProximaCrescent : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.tileCollide = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
           
            return true;
        }

        public int Overshootcooldown = 0;

        

        public override void AI()
        {

            if (Overshootcooldown > 0)
            {
                Overshootcooldown--;
            }
            if (Projectile.timeLeft >= 120)
            {
                Vector2 toMouse = Main.MouseWorld - Projectile.Center;
                float distance = toMouse.Length();

                if (distance > 1f)
                {
                    toMouse.Normalize();
                }

                if (Projectile.timeLeft >= 120)
                {
                    if (distance > 60f)
                    {
                        // sweep toward cursor
                        float speed = MathHelper.Lerp(6f, 20f, Utils.GetLerpValue(0f, 400f, distance, true));
                        Vector2 desiredVelocity = toMouse * speed;

                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.08f);
                    }
                    else
                    {
                        // eager orbit near cursor
                        float time = Main.GlobalTimeWrappedHourly;

                        Vector2 orbitOffset = new Vector2(
                            (float)Math.Cos(time * 4f),
                            (float)Math.Sin(time * 4f)
                        ) * 20f;

                        Vector2 targetPos = Main.MouseWorld + orbitOffset;
                        Vector2 toOrbit = targetPos - Projectile.Center;

                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, toOrbit * 0.1f, 0.1f);

                        // occasional overshoot burst
                        if (Overshootcooldown <= 0 && Main.rand.NextBool(40))
                        {
                            Projectile.velocity += toMouse * 10f;
                            Overshootcooldown = 30;
                        }
                    }
                }
                else
                {
                    Projectile.velocity *= 0.98f;
                }
            }
            else
            {
                Projectile.velocity *= 0.99f;
            }
            

            Player player = Main.player[Projectile.owner];

            Projectile.rotation += 0.5f * Projectile.direction;

            if (Main.rand.NextBool(3))
            {
                //Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ((int)CalamityDusts.Nightwither), 0f, 0f, 0, default, 1.2f);
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(DTAssetLib.SwordSounds.QuickSwing, Projectile.Center);
            target.AddBuff(ModContent.BuffType<Nightwither>(), 240);
        }
    }
}
