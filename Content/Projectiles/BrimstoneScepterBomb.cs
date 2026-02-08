
using DestroyerTest.Common;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using System.Collections.Generic;
using DestroyerTest.Content.Buffs;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using Terraria.Audio;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Dusts;
using GlowmaskHelper.Content;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    [AutoloadGlowmask]
	public class BrimstoneScepterBomb : ModProjectile
	{
		public override void SetStaticDefaults() {
		}
        

		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;

			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 100;
			Projectile.tileCollide = false;
            Projectile.penetrate = 1;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = projectileTexture.Height / Main.projFrames[Projectile.type];
            Rectangle frame = new Rectangle(
                0,
                frameHeight * Projectile.frame,
                projectileTexture.Width,
                frameHeight
            );

            Vector2 origin = new Vector2(projectileTexture.Width / 2f, frameHeight / 2f);


			Main.EntitySpriteDraw(
                projectileTexture,
                Projectile.Center - Main.screenPosition,
                frame,
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );

			return false;
		}

   

		public override void AI()
		{
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.Brimstone, 0f, 0f, 0, default, 1f);
			Projectile.rotation += (Projectile.velocity.Length() * 0.05f) * Projectile.direction;
		}

       
        public void Explosion()
        {
            Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BoomCloud>(), Projectile.Center, Vector2.Zero, Color.Red, 0.01f, 1.5f);
            Opus.RadialDustRandomDir((int) CalamityDusts.Brimstone, 10, Projectile.Center, 1, default, 1f, 2.6f);
        }

        public override void OnKill(int timeLeft)
        {
			Explosion();
			SoundEngine.PlaySound(DTAssetLib.Impacts.ExplosiveImpactSmall with { PitchVariance = 0.4f, MaxInstances = 0 }, Projectile.Center);
			Projectile exploded = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BrimlanceHellfireExplosion>(), Projectile.damage, 4, Projectile.owner);
            exploded.DamageType = DamageClass.Generic;
        }
    }
}