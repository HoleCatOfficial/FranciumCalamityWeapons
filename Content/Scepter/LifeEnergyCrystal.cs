using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using CalamityMod.Buffs.StatDebuffs;
using InnoVault.PRT;
using FranciumCalamityWeapons.Content.Buffs;
using Microsoft.Build.Evaluation;
using OpusLib;

namespace FranciumCalamityWeapons.Content.Scepter
{
    public class LifeEnergyCrystal : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1200;
            Projectile.tileCollide = false;
            Projectile.ai[1] = -1f;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.velocity *= 0.94f;

            if (Projectile.ai[2] > 0)
            {
                Projectile.ai[2]--;
            }

            if (Projectile.timeLeft > 60 && Projectile.timeLeft < 1140)
            {
                //TODO: rename since its no longer exclusive to scepters
                foreach (Projectile scepter in Main.projectile)
                {
                    if (scepter.friendly && scepter.active && scepter.owner == Projectile.owner && scepter.type != Projectile.type)
                    {
                        if (scepter.Hitbox.Intersects(Projectile.Hitbox) && Projectile.ai[2] <= 0)
                        {
                            if (Projectile.ai[0] < 4)
                            {
                                SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/VoidScepterHit") with { MaxInstances = 0, Pitch = Projectile.ai[1] });
                                Projectile.ai[0]++;
                                Projectile.ai[1] += 0.25f;
                                Projectile.ai[2] = 20;
                                scepter.Kill();
                            }
                            else
                            {
                                SoundEngine.PlaySound(SoundID.Research, Projectile.Center);
                                Opus.RadialSpreadDust(DustID.FireworksRGB, 16, player.Center, 0, new Color(221, 189, 62), 1f, 3, true);
                                player.AddBuff(ModContent.BuffType<UltraRegen>(), 300);
                                scepter.Kill();
                                Projectile.Kill();
                            }
                        }
                    }
                }

                if (Main.rand.NextBool(6))
                {
                    PRTLoader.NewParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], Main.rand.NextVector2FromRectangle(Projectile.Hitbox), Projectile.velocity * 0.2f + Main.rand.NextVector2Circular(1, 1), new Color(159, 202, 172), 1, 60, ai2: 2);
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.FireworksRGB, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-2, -1), 70, new Color(159, 202, 172), 1f);
                    dust.noGravity = true;
                }
            }


            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha++;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.timeLeft > 60)
            {
                Projectile.alpha = 255;
                Projectile.Resize(100, 100);
                SoundEngine.PlaySound(SoundID.Item110, Projectile.Center);

                // Spawn a bunch of smoke dusts.
                for (int i = 0; i < 30; i++) {
                    Dust smokeDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                    smokeDust.velocity *= 1.4f;
                }

                // Spawn a bunch of fire dusts.
                for (int j = 0; j < 20; j++) {
                    Dust fireDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.FireworksRGB, 0f, 0f, 100, Color.Green, 3.5f);
                    fireDust.velocity *= 7f;
                }

                // Spawn a bunch of smoke gores.
                for (int k = 0; k < 2; k++) {
                    float speedMulti = 0.4f;
                    if (k == 1) {
                        speedMulti = 0.8f;
                    }

                    Gore smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                    smokeGore.velocity *= speedMulti;
                    smokeGore.velocity += Vector2.One;
                    smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                    smokeGore.velocity *= speedMulti;
                    smokeGore.velocity.X -= 1f;
                    smokeGore.velocity.Y += 1f;
                    smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                    smokeGore.velocity *= speedMulti;
                    smokeGore.velocity.X += 1f;
                    smokeGore.velocity.Y -= 1f;
                    smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                    smokeGore.velocity *= speedMulti;
                    smokeGore.velocity -= Vector2.One;
                }
            }
        }
    }
}