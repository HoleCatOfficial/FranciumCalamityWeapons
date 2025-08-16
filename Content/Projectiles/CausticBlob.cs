using CalamityMod.Buffs.StatDebuffs;
using DestroyerTest.Common;
using FranciumCalamityWeapons.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    public class CausticBlob : ModProjectile
    {
        public override void SetStaticDefaults() {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
        }

		public override void SetDefaults()
		{
			Projectile.width = 28; // The width of projectile hitbox
			Projectile.height = 52; // The height of projectile hitbox

			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 1f; // How much light emit around the projectile
			Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.tileCollide = true;
			Projectile.netImportant = true;
			Projectile.netUpdate = true;
			}

		public int trailLength = 10;
		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = new Color(140, 234, 87);

			SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

			// --- Draw the main projectile ---
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			Main.EntitySpriteDraw(
				projectileTexture,
				Projectile.Center - Main.screenPosition,
				null,
				lightColor,
				Projectile.rotation,
				projectileTexture.Size() / 2,
				Projectile.scale,
				SpriteEffects.None,
				0
			);

			// --- Draw glow + trail ---
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			Texture2D glowTexture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/SimpleParticle").Value;
			Main.EntitySpriteDraw(
				glowTexture,
				Projectile.Center - Main.screenPosition,
				null,
				lightColor,
				Projectile.rotation,
				glowTexture.Size() / 2,
				Projectile.scale,
				SpriteEffects.None,
				0
			);

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


			Texture2D longtrailTexture = ModContent.Request<Texture2D>("DestroyerTest/Content/Particles/Trail2").Value;
			Vector2 trailOrigin = new(longtrailTexture.Width / 2f, longtrailTexture.Height / 2f);

			for (int i = 0; i < trailLength && i < Projectile.oldPos.Length; i++)
			{
				float fade = (float)(trailLength - i) / trailLength;
				Color trailColor = lightColor * fade * 0.3f;
				trailColor.A = (byte)(fade * 100);

				Vector2 drawPosition = Projectile.oldPos[i] + (Projectile.Size / 2f) - Main.screenPosition;
				float scaleFactor = 0.1f;
				Main.EntitySpriteDraw(longtrailTexture, drawPosition, null, trailColor, Projectile.rotation, trailOrigin, (Projectile.scale * fade) * scaleFactor, SpriteEffects.None, 0);
			}

			// Restore normal batch
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			return false;
		}

        // Custom AI
        public override void AI()
        {
			SoundStyle[] Options = new SoundStyle[]
				{
					SoundID.LiquidsWaterLava with { Volume = Main.rand.NextFloat(0.01f, 0.2f), PitchVariance = 2f },
					SoundID.LiquidsHoneyLava with { Volume = Main.rand.NextFloat(0.01f, 0.2f), PitchVariance = 2f },
				};
				if (Main.rand.NextBool(3))
				{
					SoundEngine.PlaySound(Main.rand.Next(Options), Projectile.Center);
				}
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticle2>(), Projectile.Center, new Vector2(Main.rand.NextFloat(1, -1), -4), new Color(140, 234, 87), Main.rand.NextFloat(0.04f, 1.4f));
            Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f; // Limit the maximum fall speed
            }
            Projectile.velocity.X *= 0.99f;
			}


        public override void OnKill(int timeLeft)
        {
            for (int t = 0; t < 14; t++)
            {
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticle2>(), Projectile.Center, new Vector2(Main.rand.NextFloat(1, -1), -4), new Color(140, 234, 87), Main.rand.NextFloat(0.04f, 1.4f));
            }
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<CausticAoE>(), 30, 0f, Projectile.owner);
            SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/CausticImpactSmall") with { PitchVariance = 2f, Volume = Main.rand.NextFloat(0.2f, 1.9f) }, Projectile.Center);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Irradiated>(), 240);
        }	
    }
}