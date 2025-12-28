using System.Collections.Generic;
using CalamityMod.Buffs.DamageOverTime;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using GlowmaskHelper.Content;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using System;

namespace FranciumCalamityWeapons.Content.Projectiles.Ammo
{
	[AutoloadGlowmask()]
	public class ArsenalArrowT3Projectile : ModProjectile
	{
		public override void SetStaticDefaults() {
			ProjectileID.Sets.FiresFewerFromDaedalusStormbow[Type] = true;
		}

		public override void SetDefaults() {
			Projectile.width = 30;
			Projectile.height = 30;

			Projectile.arrow = true;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.timeLeft = 1200;
		}

        public override bool PreDraw(ref Color lightColor)
        {
			Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Type].Value.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public Color PhaseSlayerOrange = new Color(255, 64, 31);

		public override void AI() 
		{
			Projectile.rotation = Projectile.velocity.ToRotation();
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Laceration>(), 300);
			target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 300);
        }

		public override void OnKill(int timeLeft) {
			SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/PyreMineBlast") with { MaxInstances = 1, Volume = 0.35f, PitchVariance = 0.2f }, Projectile.position);
			for (int i = 0; i < 5; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Iron);
				dust.noGravity = true;
				dust.velocity *= 1.5f;
				dust.scale *= 0.9f;
			}

            float el = 0;
            el += 0.1f;
            float t = el / 60;
            t = Math.Clamp(t, 0f, 1f);

            Color blastColor = Color.Lerp(Color.White, PhaseSlayerOrange, t);
            Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), Projectile.Center, Vector2.Zero, blastColor, 0.01f, 1f);

            Projectile.Resize(300, 300);

            // Spawn a bunch of smoke dusts.
            for (int i = 0; i < 30; i++) {
                Dust smokeDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                smokeDust.velocity *= 2.2f;
            }

            // Spawn a bunch of fire dusts.
            for (int j = 0; j < 20; j++) {
                Dust fireDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.FireworksRGB, 0f, 0f, 100, PhaseSlayerOrange, 3f);
                fireDust.velocity *= 8f;
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