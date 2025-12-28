using System;
using System.Collections.Generic;
using CalamityMod.Buffs.StatDebuffs;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Scepter
{
	public class LunarElectricityArc : ModProjectile
	{
        public override string Texture => DTUtils.NoTexture;

		public override void SetStaticDefaults()
		{
		}

		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Generic;
			Projectile.timeLeft = 1200;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
		}

		public float trailOffset = 0f;
		public override bool PreDraw(ref Color lightColor)
		{
			lightColor = new Color(94, 229, 163);
			trailOffset += 0.15f;


			SpriteBatch spriteBatch = Main.spriteBatch;
			DTUtils Utility = new DTUtils();

			Opus.StartSpriteBatchForTrails(spriteBatch, BlendState.NonPremultiplied, SpriteSortMode.Immediate);
			
			if (TrailPositions.Count > 1)
			{
				List<ColoredVertex> ve = new List<ColoredVertex>();
				List<ColoredVertex> ve2 = new List<ColoredVertex>();
				float a = 0;

				for (int i = TrailPositions.Count - 1; i > 0; i--)
				{
					float u = i / (float)(TrailPositions.Count - 1);
					float widthFactor = (float)Math.Sin(u * MathHelper.Pi);

					float width = 32f * widthFactor;

					Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
					Vector2 perp = dir.RotatedBy(MathHelper.PiOver2);
					Vector2 offset  = perp * width;
					Vector2 offset2 = -perp * width;

					DTUtils.AddStrips(ve, TrailPositions, i, offset, offset2, u, lightColor, trailOffset);
				}

				for (int i = TrailPositions2.Count - 1; i > 0; i--)
				{
					float u = i / (float)(TrailPositions.Count - 1);
					float widthFactor = (float)Math.Sin(u * MathHelper.Pi);

					float width = 32f * widthFactor;

					Vector2 dir = (TrailPositions[i] - TrailPositions[i - 1]).ToRotation().ToRotationVector2();
					Vector2 perp = dir.RotatedBy(MathHelper.PiOver2);
					Vector2 offset  = perp * width;
					Vector2 offset2 = -perp * width;

					DTUtils.AddStrips(ve2, TrailPositions2, i, offset, offset2, u, Color.White, trailOffset);
				}


				GraphicsDevice gd = Main.graphics.GraphicsDevice;
				if (ve.Count >= 3)
				{
					gd.Textures[0] = DTAssetLib.Streak(3).Value;
					gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
					gd.Textures[0] = DTAssetLib.ZapTrail.Value;
					gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve2.ToArray(), 0, ve2.Count - 2);
				}
			}

			Opus.ReturnToDefaultDrawing(spriteBatch);

			return false;
		}

		public List<Vector2> TrailPositions = new();
        public List<float> TrailRotations = new();

		public List<Vector2> TrailPositions2 = new();
        public List<float> TrailRotations2 = new();
        private const int TrailLength = 200;
        private void CacheTrail()
        {
            Vector2 lastPos = TrailPositions.Count > 0 ? TrailPositions[0] : Projectile.Center;
			Vector2 newPos  = Projectile.Center;

			float dist = Vector2.Distance(lastPos, newPos);
			float step = 1f; // how closely to sample. tweak this!

			if (dist > 0f)
			{
				int segments = (int)(dist / step);

				for (int i = 1; i <= segments; i++)
				{
					Vector2 pos = Vector2.Lerp(lastPos, newPos, i / (float)segments);
					TrailPositions.Insert(0, pos);
					TrailRotations.Insert(0, Projectile.rotation);

					TrailPositions2.Insert(0, pos);
					TrailRotations2.Insert(0, Projectile.rotation);
				}
			}
			else
			{
				TrailPositions.Insert(0, newPos);
				TrailRotations.Insert(0, Projectile.rotation);

				TrailPositions2.Insert(0, newPos);
				TrailRotations2.Insert(0, Projectile.rotation);
			}


			// Cap trail
			while (TrailPositions.Count > TrailLength)
			{
				TrailPositions.RemoveAt(TrailPositions.Count - 1);
			}
			while (TrailRotations.Count > TrailLength)
			{
				TrailRotations.RemoveAt(TrailRotations.Count - 1);
			}
			while (TrailPositions2.Count > TrailLength)
			{
				TrailPositions2.RemoveAt(TrailPositions2.Count - 1);
			}
			while (TrailRotations2.Count > TrailLength)
			{
				TrailRotations2.RemoveAt(TrailRotations2.Count - 1);
			}

			foreach(Vector2 pt in TrailPositions)
			{
				Lighting.AddLight(pt, new Color(94, 229, 163).ToVector3() * 0.3f);
			}
        }

		public override void AI()
		{
			CacheTrail();
		}

		public override void OnKill(int timeLeft) 
		{
			for (int i = 0; i < 5; i++)
			{
				Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Main.rand.NextVector2Circular(120, 120), 0, new Color(94, 229, 163), 1f);
				dust.noGravity = true;
				dust.velocity *= 1.5f;
				dust.scale *= 0.9f;
			} 
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/MagnetOrbBreak") with { PitchVariance = 0.5f, MaxInstances = 0 });
			for (int i = 0; i < 5; i++)
			{
				Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Main.rand.NextVector2Circular(120, 120), 0, new Color(94, 229, 163));
				dust.noGravity = true;
				dust.velocity *= 1.5f;
				dust.scale *= 0.9f;
			} 
			if (Projectile.velocity.X != oldVelocity.X) 
			{
				Projectile.velocity.X = -oldVelocity.X;
			}
			if (Projectile.velocity.Y != oldVelocity.Y) 
			{
				Projectile.velocity.Y = -oldVelocity.Y;
			}
			Projectile.ai[1]++;
			if (Projectile.ai[1] >= 10)
			{
				Projectile.Kill();
			}
            return false;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GalvanicCorrosion>(), 600);
        }

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            foreach (var trail in new[] { TrailPositions})
            {
                for (int i = 1; i < trail.Count; i++)
                {
                    Vector2 point1 = trail[i - 1];
                    Vector2 point2 = trail[i];
                    if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), point1, point2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}