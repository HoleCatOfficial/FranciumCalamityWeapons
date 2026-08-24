using BreadLibrary.Core.Graphics.Particles;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Typeless;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Stellar;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using FranciumCalamityWeapons.Common;
using FranciumCalamityWeapons.Content.Melee;
using FranciumCalamityWeapons.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Social.Base;

namespace FranciumCalamityWeapons.Content.Projectiles
{

	public class OverlordSwing : BaseBroadswordProjectileFullSwing
	{

		public override void SetDefaults() 
		{
            base.SetDefaults();
            Projectile.width = 188;
			Projectile.height = 188;
            UsesDefaultSweepFX = true;
            SweepColor = DTUtilsCalamity.GodSlayerInfernoGradient(SlashProgress);
            SweepHighlightColor = DTUtilsCalamity.GodSlayerInfernoGradient(SlashProgress);
            SwingSpeed = 0.16f;
            WaitTimeMultiplier = 1.7f;
            SweepScale = 2.1f;
            Projectile.extraUpdates = 3;

            ScaleMult = 1.6f;
            Glowmask = ModContent.Request<Texture2D>(Texture);

        }

        public Vector2 swordTip;
        public Line SwordLine;

        public override void ExtraEffects()
        {
            Color Blue = new Color(38, 148, 237);
            Color Pink = new Color(217, 46, 223);

            swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);

            SwordLine = new Line(Owner.MountedCenter, swordTip);

            Vector2[] pt = SwordLine.GetPointsAlongLine(50);
            Vector2[] ppt = pt[10..50];

            int threshold = CurrentState == State.Wait ? 0 : (int)MathHelper.Lerp(0, 5, SlashProgress);

            for (int i = 0; i < threshold; i++)
            {
                Color BP = Main.rand.NextBool() ? Blue : Pink;
                float SPD = Owner.direction == 1 ? Main.rand.NextFloat(2f, 10f) : Main.rand.NextFloat(-10f, -2f);
                Dust D = Dust.NewDustPerfect(ppt[Main.rand.Next(40)], DustID.FireworksRGB, (SwordLine.GetLineRotation - MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(0.2f) * SPD, 0, BP, 1f);
                D.noGravity = true;

                
            }


            int threshold2 = CurrentState == State.Wait ? 0 : (int)MathHelper.Lerp(0, 2, SlashProgress);

            for (int i = 0; i < threshold2; i++)
            {
                Color BP = Main.rand.NextBool() ? Blue : Pink;
                float SPD = Owner.direction == 1 ? Main.rand.NextFloat(1f, 4f) : Main.rand.NextFloat(-4f, -1f);
                StarParticle Star = new();

                Star.Initialize(ppt[Main.rand.Next(40)], (SwordLine.GetLineRotation - MathHelper.PiOver2).ToRotationVector2().RotatedByRandom(0.2f) * SPD, BP, 1f);
                ParticleEngine.BehindProjectiles.Add(Star);


            }

            SweepColor = DTUtilsCalamity.GodSlayerInfernoGradient(SlashProgress);

            var modPlayer = Owner.GetModPlayer<OverlordCountPlayer>();

            PitchAMT = MathHelper.Lerp(-0.7f, 0.4f, (float)modPlayer.HitCount / (float)modPlayer.HitThreshold2);


        }

        float PitchAMT = -0.7f;
        public override SoundStyle Swing => new SoundStyle("FranciumCalamityWeapons/Audio/OverlordSwing") with { PitchVariance = 0.6f };

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            Color Blue = new Color(38, 148, 237);
            Color Pink = new Color(217, 46, 223);

            SoundEngine.PlaySound(DTAssetLib.Impacts.MagicHit with { PitchVariance = 0.2f, Pitch = PitchAMT, Volume = 0.5f });
            SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/CosmicStarSpawn") with { Pitch = PitchAMT, Volume = 1.5f });
            SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/CalamityBell") with { Pitch = PitchAMT, Volume = 3.0f });

            

            if (hit.Crit)
            {
                npc.AddBuff(ModContent.BuffType<WhisperingDeath>(), 360);
            }

            var modPlayer = Owner.GetModPlayer<OverlordCountPlayer>();

            modPlayer.HitCount += 1;

            if (modPlayer.HitCount >= modPlayer.HitThreshold2)
            {
                modPlayer.HitCount = 0;
                modPlayer.DecayStartTimer = 0;

                SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/DevourerDeathImpact") with { PitchVariance = 0.3f });
                SoundEngine.PlaySound(DTAssetLib.ScholarShieldSounds.Break with { PitchVariance = 0.2f });

                DTUtils.InfectedScepter_RingSpreadProjectileAlternating(ModContent.ProjectileType<CosmicStarPink>(), ModContent.ProjectileType<CosmicStarBlue>(), 16, npc.Center, 20, Projectile.damage / 8, 10, 10, RandomOffset: true);

                for (int i = 0; i < 10; i++)
                {
                    Color BP = Main.rand.NextBool() ? Blue : Pink;
                    Dust.NewDustPerfect(npc.Center, DustID.FireworksRGB, Main.rand.NextVector2Circular(22f, 22f), 0, BP, 2f);
                }

                for (int j = 0; j < 5; j++)
                {
                    Dust.NewDustPerfect(npc.Center, DustID.FireworksRGB, Main.rand.NextVector2Circular(22f, 22f), 0, Color.White, 1f);
                }

                Projectile.NewProjectile(
                    Entity.GetSource_FromThis(),
                    npc.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<CosmicDashExplosion>(),
                    Projectile.damage,
                    1f,
                    Projectile.owner
                );

                Owner.GetModPlayer<ScreenshakePlayer>().screenshakeMagnitude = 10;
                Owner.GetModPlayer<ScreenshakePlayer>().screenshakeTimer = 120;
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    Color BP = Main.rand.NextBool() ? Blue : Pink;
                    Dust.NewDustPerfect(npc.Center, DustID.FireworksRGB, Main.rand.NextVector2Circular(22f, 22f), 0, BP, 2f);
                }

                DTUtils.InfectedScepter_RingSpreadProjectileAlternating(ModContent.ProjectileType<CosmicStarPink>(), ModContent.ProjectileType<CosmicStarBlue>(), 4, npc.Center, 20, Projectile.damage / 8, 10, 10, RandomOffset: true);
                Owner.GetModPlayer<ScreenshakePlayer>().screenshakeMagnitude = 4;
                Owner.GetModPlayer<ScreenshakePlayer>().screenshakeTimer = 30;
            }

        }
	}
}