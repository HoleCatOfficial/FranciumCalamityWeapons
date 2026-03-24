using CalamityMod.Buffs.DamageOverTime;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.Weapon.Rogue;
using FranciumCalamityWeapons.Content.Melee;
using InnoVault.PRT;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles.Arsenal
{

    public class BreakerSwing : BaseBroadswordProjectile
    {
        public bool Power = false;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 76;
            Projectile.height = 76;
            SweepColor = Color.White;
            Glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow");
        }

        public override SoundStyle Swing => DTAssetLib.SwordSounds.Woosh;

        public Color Tesla = new Color(144, 246, 255);

        public override void ExtraEffects()
        {
            if (Owner.HeldItem.ModItem is Breaker B)
            {
                if (B.HitCount > 6)
                {
                    Power = true;
                }
            }

            if (Power)
            {
                if (TeslaBurstOpacity > 0)
                {
                    TeslaBurstOpacity -= 0.05f;
                }
                SweepColor = Tesla;
                SparkEdge(Main.player[Projectile.owner], 1f, Tesla);
            }
            else
            {
                if (TeslaBurstOpacity < 1)
                {
                    TeslaBurstOpacity = 1;
                }
            }
        }

        public float TeslaBurstOpacity = 1f;
        public override void DrawOverBlade()
        {
            Vector2 origin;
            float rotationOffset;
            SpriteEffects effects;

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D Tesla = ModContent.Request<Texture2D>($"{Texture}_Tesla").Value;
            Texture2D Tesla2 = ModContent.Request<Texture2D>($"{Texture}_Tesla2").Value;

            //i swear to FUCKING GOD.
            //dont touch this shit.
            //FUCK ROTATIONS DUDE.

            if (LastSwing == -1)
            {
                if (Projectile.spriteDirection > 0)
                {
                    origin = new Vector2(0, texture.Height);
                    effects = SpriteEffects.None;
                    rotationOffset = MathHelper.ToRadians(45f);
                }
                else
                {
                    origin = new Vector2(0, texture.Height);
                    effects = SpriteEffects.None;
                    rotationOffset = MathHelper.ToRadians(50f);
                }
            }
            else
            {
                if (Projectile.spriteDirection > 0)
                {
                    origin = new Vector2(texture.Width, texture.Height);
                    effects = SpriteEffects.FlipHorizontally;
                    rotationOffset = MathHelper.ToRadians(135);
                }
                else
                {
                    origin = new Vector2(texture.Width, texture.Height);
                    effects = SpriteEffects.FlipHorizontally;
                    rotationOffset = MathHelper.ToRadians(135f);
                }
            }


            if (Power)
            {
                Main.EntitySpriteDraw(Tesla, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation + rotationOffset + RotationManualOffset, origin, AdjustedScale, effects, 0);

                Main.EntitySpriteDraw(Tesla2, Projectile.Center - Main.screenPosition, null, (Color.White * Projectile.Opacity) * TeslaBurstOpacity, Projectile.rotation + rotationOffset + RotationManualOffset, origin, AdjustedScale, effects, 0);
            }
        }

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit)
        {
            if (Owner.HeldItem.ModItem is Breaker B)
            {
                B.HitCount++;
                B.DecayTimer = 180;
                TeslaBurstOpacity = 1f;
            }
            if (Power)
            {
                SoundEngine.PlaySound(DTAssetLib.Zap, npc.Center);
                npc.AddBuff(ModContent.BuffType<StaticDischarge>(), 500);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float MaxDiminishMultiplier = 0.4f;
            float CurrentDiminishMultiplier = 1f;
            float FirstPowerStrikeMultiplier = 2f;
            if (Power)
            {
                if (Owner.HeldItem.ModItem is Breaker B)
                {
                    if (B.HitCount == 7)
                    {
                        modifiers.SourceDamage *= FirstPowerStrikeMultiplier;
                    }
                    if (B.HitCount > 7)
                    {
                        int extraHits = B.HitCount - 7;

                        float diminishRate = 0.12f;

                        CurrentDiminishMultiplier = MathF.Max(MaxDiminishMultiplier, 1f - (extraHits * diminishRate));

                        modifiers.SourceDamage *= CurrentDiminishMultiplier;
                    }
                }
            }
            else
            {
                CurrentDiminishMultiplier = 1f; //Reset
            }
        }
    }
}