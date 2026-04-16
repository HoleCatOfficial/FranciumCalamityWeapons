using CalamityMod;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using FranciumCalamityWeapons.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles.StealthStrike
{

    public class DreamDiscMini : ModProjectile
    {
        private NPC HomingTarget
        {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set
            {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }

        public ref float DelayTimer => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 30;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32; 
            Projectile.height = 32;

            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D value = ModContent.Request<Texture2D>("FranciumCalamityWeapons/Content/Extras/DreamDiscMiniHighlight", ReLogic.Content.AssetRequestMode.AsyncLoad).Value;
            float num = Opus.Sine(0f, 0.6f, 1f);
            SpriteBatch spriteBatch = Main.spriteBatch;
            SpriteEffects effects = SpriteEffects.None;
            if ( Projectile.direction == -1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            Opus.StartSpriteBatchWithBlending(spriteBatch, BlendState.Additive, SpriteSortMode.Immediate);
            Vector2 vector = new Vector2((float)value.Width * 0.5f, (float) Projectile.height * 0.5f);
            for (int num2 =  Projectile.oldPos.Length - 1; num2 > 0; num2--)
            {
                Vector2 position =  Projectile.oldPos[num2] - Main.screenPosition + vector + new Vector2(0f,  Projectile.gfxOffY);
                Color color = Main.DiscoColor * ((float)( Projectile.oldPos.Length - num2) / (float) Projectile.oldPos.Length);
                Main.EntitySpriteDraw(value, position, null, color,  Projectile.rotation, vector,  Projectile.scale, effects);
            }

            Opus.ReturnToDefaultDrawing(spriteBatch);
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value,  Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, TextureAssets.Projectile[Type].Value.Size() / 2f,  Projectile.scale, effects);
            return false;
        }

        public override void AI()
        {
            Projectile.rotation += 0.55f * (float)Projectile.direction;
            Lighting.AddLight(Projectile.Center, Main.DiscoColor.ToVector3() * 1.0f);

            if (DelayTimer < 10)
            {
                DelayTimer += 1;
                return;
            }

            float maxDetectRadius = 1400f;

            if (HomingTarget == null)
            {
                HomingTarget = FindClosestNPC(maxDetectRadius);
            }

            if (HomingTarget != null && !IsValidTarget(HomingTarget))
            {
                HomingTarget = null;
            }

            if (HomingTarget == null)
                return;

            float length = Projectile.velocity.Length();
            float targetAngle = Projectile.AngleTo(HomingTarget.Center);
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(7)).ToRotationVector2() * length;
        }

        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            foreach (var target in Main.ActiveNPCs)
            { 
                if (IsValidTarget(target))
                {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public bool IsValidTarget(NPC target)
        {
            return target.CanBeChasedBy();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(DTAssetLib.Impacts.ShortShine, target.Center);
        }
    }
}