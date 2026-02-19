
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
using System;

namespace FranciumCalamityWeapons.Content.Projectiles
{
	public class NecroTrail : ModProjectile
	{
        public override string Texture => DTUtils.NoTexture;

		private NPC HomingTarget {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }
        public ref float DelayTimer => ref Projectile.ai[1];
		
		public override void SetStaticDefaults() {
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
			ProjectileID.Sets.TrailingMode[Type] = 3;
			ProjectileID.Sets.TrailCacheLength[Type] = 12;
		}

		public override void SetDefaults()
		{
			Projectile.width = 16; // The width of projectile hitbox
			Projectile.height = 16; // The height of projectile hitbox

			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.timeLeft = 300; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.tileCollide = false;
            Projectile.penetrate = -1;
		}
		public override bool PreDraw(ref Color lightColor)
		{

			SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
			DTUtils Utility = new DTUtils();
			float opacity = Projectile.Opacity;

			Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);

            Opus.DrawTextureOnProj(DTAssetLib.PointGlow, Projectile, drawColor * opacity, true, Projectile.rotation, Scale1, Scale1);
			Opus.ReturnToDefaultDrawing(spriteBatch);

			return false;
		}

        public float Scale1 = 0f;
        public Color drawColor;
        public float rot = 0;
        
		public override void AI()
		{
            rot += 0.05f;
            drawColor = Opus.Sine(new Color(254, 80, 128), Color.White, 0.01f);
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			Projectile.rotation += (Projectile.velocity.Length() * 0.5f) * Projectile.direction;

            Projectile.ai[2]++;

            Scale1 = Opus.Sine(1f, 2f, 0.01f);

            Lighting.AddLight(Projectile.Center, drawColor.ToVector3() * Scale1);

			if (Projectile.ai[2] > 200)
			{
				Projectile.Opacity -= 0.01f;
			}

			Homing();
		}

		public void Homing()
		{
			if(DelayTimer < 20)
            {
                DelayTimer++;
                return;
            }

            if (HomingTarget == null) {
                HomingTarget = FindClosestNPC(1200);
            }

            if (HomingTarget != null && !IsValidTarget(HomingTarget)) {
                HomingTarget = null;
            }

            if (HomingTarget == null)
			{
				Projectile.velocity *= 0.94f;
                return;
			}

            float length = Projectile.velocity.Length();
            float targetAngle = Projectile.AngleTo(HomingTarget.Center);
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(30)).ToRotationVector2() * length;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			if (length < 10)
			{
				Projectile.velocity *= 1.1f;
			}
			Projectile.penetrate = 1;
		}

		public NPC FindClosestNPC(float maxDetectDistance) {
            NPC closestNPC = null;

            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            foreach (var target in Main.ActiveNPCs) {
                if (IsValidTarget(target)) {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);
                    if (sqrDistanceToTarget < sqrMaxDetectDistance) {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public bool IsValidTarget(NPC target) 
        {
            if (Projectile.tileCollide == true)
            {
                return target.CanBeChasedBy() && Collision.CanHit(Projectile.Center, 1, 1, target.position, target.width, target.height);
            }
            else
            {
                return target.CanBeChasedBy();
            }
        }

    }
	
}