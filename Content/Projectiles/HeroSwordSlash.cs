using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using FranciumCalamityWeapons.Content.Melee;
using Terraria.GameContent;
using FranciumCalamityWeapons.Common;
using System.Collections.Generic;
using ReLogic.Content;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    internal class HeroSwordSlash : ModProjectile
    {
        //public override string Texture => "FranciumCalamityWeapons/Content/Projectiles/TestSword";
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 108;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 200; // persistent
        }
        
        public List<float> TrailIndex = new List<float>();
        public List<Vector2> TrailIndexPos = new List<Vector2>();
        
        
        
        public override bool PreDraw(ref Color lightColor)
        {
            //Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D texture = ModContent.Request<Texture2D>("FranciumCalamityWeapons/Content/Extras/HeroSwordTrail").Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            //Effect shader = ModContent.Request<Effect>("FranciumCalamityWeapons/Assets/HSHLShaders/SlashTrans", AssetRequestMode.ImmediateLoad).Value;
            SpriteBatch Spritebatch = Main.spriteBatch;

            // Interpolation for smooth trails
            for (int k = 1; k < TrailIndexPos.Count; k++)
            {
                Vector2 startPos = TrailIndexPos[k - 1];
                Vector2 endPos = TrailIndexPos[k];

                // Interpolate positions between the two points
                int interpolationSteps = 16; // Adjust for smoother trail
                for (int i = 0; i < interpolationSteps; i++)
                {
                    float t = i / (float)interpolationSteps;
                    Vector2 interpolatedPos = Vector2.Lerp(startPos, endPos, t);
                    float interpolatedRotation = MathHelper.Lerp(TrailIndex[k - 1], TrailIndex[k], t);

                    Vector2 drawPos = (interpolatedPos - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(lightColor) * ((TrailIndexPos.Count - k) / (float)TrailIndexPos.Count) * (1 - t);
                    Spritebatch.End();
                    Spritebatch.Begin(0, BlendState.Additive, Spritebatch.GraphicsDevice.SamplerStates[0], Spritebatch.GraphicsDevice.DepthStencilState, Spritebatch.GraphicsDevice.RasterizerState, null, Main.GameViewMatrix.TransformationMatrix);
                    Main.EntitySpriteDraw(texture, drawPos, null, Color.White * 0.02f, interpolatedRotation, drawOrigin, 0.1f * Projectile.scale, SpriteEffects.None, 0);
                    Spritebatch.End();
                    Spritebatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                }
            }

            return true;
        }
        

        /*
        public override bool PreDraw(ref Color lightColor)
        {
            //Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D texture = ModContent.Request<Texture2D>("FranciumCalamityWeapons/Content/Extras/HeroSwordTrail").Value;
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);

            // Interpolation for smooth trails
            for (int k = 1; k < TrailIndexPos.Count; k++)
            {
                Vector2 startPos = TrailIndexPos[k - 1];
                Vector2 endPos = TrailIndexPos[k];

                // Interpolate positions between the two points
                int interpolationSteps = 12; // Adjust for smoother trail
                for (int i = 0; i < interpolationSteps; i++)
                {
                    float t = i / (float)interpolationSteps;
                    Vector2 interpolatedPos = Vector2.Lerp(startPos, endPos, t);
                    float interpolatedRotation = MathHelper.Lerp(TrailIndex[k - 1], TrailIndex[k], t);

                    Vector2 drawPos = (interpolatedPos - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);

                    // Fast fading alpha curve, controlled by a power
                    float fadeAmount = ((TrailIndexPos.Count - k) / (float)TrailIndexPos.Count) * (1 - t);
                    fadeAmount = (float)Math.Pow(fadeAmount, 2.5f); // Adjust this power for faster/slower fade

                    Color color = Projectile.GetAlpha(lightColor) * fadeAmount;
                    Main.EntitySpriteDraw(texture, drawPos, null, color * 0.6f, interpolatedRotation, drawOrigin, 0.1f * Projectile.scale, SpriteEffects.None, 0);
                }
            }

            return true;
        }
        */







        public enum State
        {
            Down,
            Up,
            Idle
        }
        public override void AI()
        {
           
            TrailIndex.Add(Projectile.rotation);
            TrailIndexPos.Add(Projectile.position);
            

            
            if (TrailIndex.Count > 60)
            {
                TrailIndex.RemoveAt(0);
            }
            if (TrailIndexPos.Count > 60)
            {
                TrailIndexPos.RemoveAt(0);
            }
            
            Player player = Main.player[Projectile.owner];

            // Only continue if holding the correct item and swinging
            if (player.HeldItem.type == ModContent.ItemType<HeroSword>() && player.itemTime > 0)
            {
                Vector2 direction = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
                float MaxSwingTime = 8f;
                float IdleTime = 4f;

                State state = (State)Projectile.ai[1];

                switch (state)
                {
                    case State.Down:
                        Projectile.ai[0]++;
                        float downProgress = Projectile.ai[0] / MaxSwingTime;
                        float downStart = direction.ToRotation() - MathHelper.PiOver2;
                        float downEnd = direction.ToRotation() + MathHelper.PiOver2;

                        Projectile.rotation = MathHelper.Lerp(downStart, downEnd, downProgress);
                        SetProjectilePosition(player);

                        if (Projectile.ai[0] >= MaxSwingTime)
                        {
                            Projectile.ai[1] = (float)State.Idle;
                            Projectile.ai[0] = 0;
                            Projectile.localAI[0] = (float)State.Up;
                        }
                        break;

                    case State.Up:
                        Projectile.ai[0]++;
                        float upProgress = Projectile.ai[0] / MaxSwingTime;
                        float upStart = direction.ToRotation() - MathHelper.PiOver2;
                        float upEnd = direction.ToRotation() + MathHelper.PiOver2;

                        Projectile.rotation = MathHelper.Lerp(upStart, upEnd, upProgress);
                        SetProjectilePosition(player);

                        if (Projectile.ai[0] >= MaxSwingTime)
                        {
                            Projectile.ai[1] = (float)State.Idle;
                            Projectile.ai[0] = 0;
                            Projectile.localAI[0] = -1f;
                        }
                        break;

                    case State.Idle:
                        Projectile.ai[0]++;
                        if (Projectile.ai[0] >= IdleTime)
                        {
                            if (Projectile.localAI[0] == (float)State.Up)
                            {
                                Projectile.ai[1] = (float)State.Up;
                                Projectile.ai[0] = 0;
                            }
                            else
                            {
                                Projectile.Kill();
                            }
                        }
                        SetProjectilePosition(player);
                        break;
                }
            }
            else
            {
                Projectile.Kill();
            }
        }

        private void SetProjectilePosition(Player player)
        {
            Vector2 offset = new Vector2(-Projectile.width / 2, -Projectile.height / 2).RotatedBy(Projectile.rotation);
            Projectile.Center = player.Center + offset;
        }

        
    }
}