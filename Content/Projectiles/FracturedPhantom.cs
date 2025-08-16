using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using InnoVault.PRT;
using FranciumCalamityWeapons.Content.Particles;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.UI;
using CalamityMod;
using DestroyerTest.Content.Projectiles;
using Terraria.Audio;
using FranciumCalamityWeapons.Content.Debuffs;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    public class FracturedPhantom : ModProjectile
    {
        private NPC HomingTarget
        {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set
            {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public float VortRot = 0f;

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch SB = Main.spriteBatch;
            lightColor = new Color(Main.DiscoR / 2, (byte)(Main.DiscoG / 1.25f), (byte)(Main.DiscoB / 1.5f));
            Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

            DrawTelegraph(Projectile.Center, Main.LocalPlayer.Center, ModContent.Request<Texture2D>("FranciumCalamityWeapons/Content/Particles/BasicLine").Value);

            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D glowTexture = ModContent.Request<Texture2D>("FranciumCalamityWeapons/Content/Projectiles/VoidAura").Value;
            SB.Draw(
                glowTexture,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                VortRot,
                glowTexture.Size() / 2,
                Projectile.scale,
                SpriteEffects.None,
                0
            );

            SB.End();
            SB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            SB.Draw(
                projectileTexture,
                Projectile.Center - Main.screenPosition,
                null,
                Color.White,
                Projectile.rotation,
                projectileTexture.Size() / 2,
                Projectile.scale,
                SpriteEffects.None,
                0
            );


            return false;
        }

        public int totalPhantoms = 0;
        public int ProjTimer = 0;

        public override void AI()
        {
            
            totalPhantoms = 0;

            if (totalPhantoms >= 0 && FracturedPhantomBuff.Inf == true)
            {
                Projectile.timeLeft = 60;
            }

            if (FracturedPhantomBuff.Inf == false)
            {
                Projectile.Kill();
            }
            

            //if (Main.rand.NextBool(4)) // 33% chance per tick
                //{
                //PRTLoader.NewParticle(PRTLoader.GetParticleID<Ember>(), Projectile.Center, Vector2.Zero, new Color(Main.DiscoR / 2, (byte)(Main.DiscoG / 1.25f), (byte)(Main.DiscoB / 1.5f)), 1);
                //}
                VortRot = Main.GameUpdateCount * -0.2f;

            Player owner = Main.player[Projectile.owner];
            owner.AddBuff(ModContent.BuffType<FracturedPhantomBuff>(), 120);

            // Count all active FracturedPhantom projectiles owned by this player
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == owner.whoAmI && proj.type == Projectile.type)
                    totalPhantoms++;
            }

            // Find this projectile's index among the player's FracturedPhantoms
            int myIndex = 0;
            for (int i = 0, found = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == owner.whoAmI && proj.type == Projectile.type)
                {
                    if (proj.whoAmI == Projectile.whoAmI)
                    {
                        myIndex = found;
                        break;
                    }
                    found++;
                }
            }

            if (HomingTarget == null || !HomingTarget.active || !IsValidTarget(HomingTarget))
            {




                // Define arc
                float arc = MathF.PI; // 180 degrees

                // Angle spacing between each phantom
                float spacing = totalPhantoms > 1 ? arc / (totalPhantoms - 1) : 0f;

                // Angle starts at -π/2 (pointing straight up) and fans out right and left
                float spacingAngle = (-arc / 2f + spacing * myIndex) - MathF.PI / 2;


                // Convert polar to Cartesian; Y is down in Terraria, so no need to invert Y
                Vector2 offset = new Vector2(MathF.Cos(spacingAngle), MathF.Sin(spacingAngle)) * 120f;

                // Set rotation so the projectile points radially outward from the owner
                Projectile.rotation = spacingAngle;

                Vector2 idleCenter = owner.Center + offset;

                if (Projectile.Center != idleCenter)
                {
                    Projectile.velocity = (idleCenter - Projectile.Center) * 0.25f;
                }

            }

            float maxDetectRadius = 800f;

            if (HomingTarget == null)
            {
                HomingTarget = FindClosestNPC(maxDetectRadius);
            }

            // If we have a homing target, make sure it is still valid. If the NPC dies or moves away, we'll want to find a new target
            if (HomingTarget != null && !IsValidTarget(HomingTarget))
            {
                HomingTarget = null;
            }

            // If we don't have a target, don't adjust trajectory
            if (HomingTarget == null)
                return;

            if (HomingTarget != null && HomingTarget.active && IsValidTarget(HomingTarget))
            {

                // Calculate this projectile's index among all owned FracturedPhantoms
                myIndex = 0;
                int found = 0, total = 0;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile proj = Main.projectile[i];
                    if (proj.active && proj.owner == owner.whoAmI && proj.type == Projectile.type)
                    {
                        if (proj.whoAmI == Projectile.whoAmI)
                            myIndex = found;
                        found++;
                    }
                }
                total = found;

                // Orbit around the target, spaced evenly
                float radius = HomingTarget.width + 80f;
                float speed = 0.05f;
                float baseAngle = Main.GameUpdateCount * speed;
                float spacing = MathHelper.TwoPi / Math.Max(1, total);
                float myAngle = baseAngle + myIndex * spacing;
                float targetAngle = Projectile.AngleTo(HomingTarget.Center);

                Vector2 orbitOffset = new Vector2(MathF.Cos(myAngle), MathF.Sin(myAngle)) * radius;
                Vector2 orbitingCenter = HomingTarget.Center + orbitOffset;
                if (Projectile.Center != orbitingCenter)
                {
                    Projectile.velocity = (orbitingCenter - Projectile.Center) * 0.25f;
                }
                Projectile.rotation = targetAngle;
                ProjTimer++;
                if (ProjTimer >= 100)
                {
                    SoundEngine.PlaySound(SoundID.Item104);
                    Projectile SoulFire = Projectile.NewProjectileDirect(
                        Entity.GetSource_FromThis(),
                        Projectile.Center,
                        /*CubicBezier(Projectile.Center, Projectile.Center + new Vector2(0, 6), Projectile.Center + new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, 5)), HomingTarget.Center, 2.0f).RotatedBy(Projectile.rotation)*/ 
                        (HomingTarget.Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * 20f, // Example velocity, adjust as needed
                        ModContent.ProjectileType<SoulFireball>(),
                        80,
                        1
                    );
                    ProjTimer = 0;
                }
            }
        }

        public static Vector2 CubicBezier(Vector2 start, Vector2 control1, Vector2 control2, Vector2 end, float t)
		{
			float u = 1 - t;
			return (u * u * u * start) + (3 * u * u * t * control1) + (3 * u * t * t * control2) + (t * t * t * end);
		}

        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs
            foreach (var target in Main.ActiveNPCs)
            {
                // Check if NPC able to be targeted. 
                if (IsValidTarget(target))
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
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
            // This method checks that the NPC is:
            // 1. active (alive)
            // 2. chaseable (e.g. not a cultist archer)
            // 3. max life bigger than 5 (e.g. not a critter)
            // 4. can take damage (e.g. moonlord core after all it's parts are downed)
            // 5. hostile (!friendly)
            // 6. not immortal (e.g. not a target dummy)
            // 7. doesn't have solid tiles blocking a line of sight between the projectile and NPC
            return target.CanBeChasedBy();
        }

        public override void PostDraw(Color lightColor)
        {
            
            base.PostDraw(lightColor);
            
        }
        
        public void DrawTelegraph(Vector2 start, Vector2 end, Texture2D texture)
        {
            Vector2 direction = end - start;
            float length = direction.Length();
            direction.Normalize();
            texture ??= ModContent.Request<Texture2D>("FranciumCalamityWeapons/Content/Particles/SpiritTrail").Value;
            SpriteBatch spriteBatch = Main.spriteBatch;

            float rotation = direction.ToRotation();

            // Assuming your texture is a chain segment, like 16px long
            float segmentLength = texture.Height * 0.75f; // or Width, depending on the texture orientation
            //Effect Wavy = Mod.Assets.Request<Effect>("Assets/Shaders/Custom/WavyDeform", AssetRequestMode.ImmediateLoad).Value;
            //Wavy.CurrentTechnique.Passes["WavyPass"].Apply();

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (float i = 0; i < length; i += segmentLength)
            {
                Vector2 position = start + direction * i;

                Main.spriteBatch.Draw(
                    texture,
                    position - Main.screenPosition,
                    null,
                    new Color(Main.DiscoR / 2, (byte)(Main.DiscoG / 1.25f), (byte)(Main.DiscoB / 1.5f)),
                    rotation + MathHelper.PiOver2, // Adjust if your texture points upward
                    new Vector2(texture.Width / 2f, texture.Height / 2f), // Origin at center
                    1f, // Scale
                    SpriteEffects.None,
                    0f
                );
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }       
    }
}