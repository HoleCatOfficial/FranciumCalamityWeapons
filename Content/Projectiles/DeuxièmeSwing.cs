
using BreadLibrary.Core.Graphics.Particles;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Melee;
using DestroyerTest.Common;
using DestroyerTest.Common.Systems;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Particles.Orchestrated;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.Weapon.Melee;
using FranciumCalamityWeapons.Common;
using FranciumCalamityWeapons.Content.Particles;
using FranciumCalamityWeapons.Content.Particles.Orchestrated;
using InnoVault.PRT;
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
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles
{
	public class DeuxièmeSwing : BaseBroadswordProjectileFullSwing
	{
		public override void SetDefaults() 
        {
            base.SetDefaults();
            Projectile.width = 164;
			Projectile.height = 164;
            UsesDefaultSweepFX = true;
            SweepColor = DTUtilsCalamity.DeuxiemeColor;
            SweepHighlightColor = DTUtilsCalamity.DeuxiemeColor;
            SweepScale = 1.9f;

            ScaleMult = 1.7f;


            Glowmask = ModContent.Request<Texture2D>($"{Texture}_Glow");
        }

        public override void ExtraEffects()
        {
            SweepColor = DTUtilsCalamity.DeuxiemeColor;
            SweepHighlightColor = DTUtilsCalamity.DeuxiemeColor;
        }

        public override SoundStyle Swing => new SoundStyle("CalamityMod/Sounds/Item/HeavySwing") with { MaxInstances = 0, PitchVariance = 0.3f, Volume = 0.7f };

        public override void HitNPCEffects(NPC npc, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(DTAssetLib.IdriGreatswordSlice(ChildSafety.Disabled) with { PitchVariance = 0.1f, MaxInstances = 10 }, npc.Center);
            Player player = Main.player[Projectile.owner];
            var ScreenShake = player.GetModPlayer<ScreenshakePlayer>();

            int splatterdir = npc.position.X > Owner.MountedCenter.X ? 1 : -1;
            for (int i = 0; i < 7; i++)
            {
                Spark Spark = new Spark();
                Spark.PrepareSpark(npc.Center, new Vector2(Main.rand.NextFloat(2f, 6f) * splatterdir, 0).RotatedByRandom(0.1f), 0f, DTUtilsCalamity.DeuxiemeColor, 1f, false, 30, SparkDrawMode.Additive);
                ParticleEngine.BehindProjectiles.Add(Spark);
            }

            DeuxiemeParticle FX = new DeuxiemeParticle();
            FX.Initiate(npc.Center);
            ParticleEngine.BehindProjectiles.Add(FX);

            //Opus.RadialSpreadParticle(DTUtils.Fire[Main.rand.Next(DTUtils.Fire.Length)], 10, npc.Center, 0.4f, DTUtilsCalamity.DeuxiemeColor, 2f, 3, RandomOffset: true);
            Opus.RadialSpreadProjectileRandom(ModContent.ProjectileType<DeuxiemeStar>(), 2, npc.Center, (int)(Projectile.damage * 0.2f), (int)(Projectile.knockBack * 0.5f), 14f);

            if (hit.Crit)
            {
                ScreenShake.screenshakeMagnitude = 8;
                ScreenShake.screenshakeTimer = 10;
                //SoundEngine.PlaySound(DTAssetLib.EnergyWoosh with { PitchVariance = 0.4f });
                SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Impacts/MagicHit", 3) { PitchVariance = 0.1f, MaxInstances = 10 }, npc.Center);
                for (int t = 0; t < 2; t++)
                {
                    Projectile.NewProjectile(Projectile.GetSource_OnHit(npc), npc.Center, new Vector2(20f * splatterdir, 0).RotatedByRandom(0.1f), ModContent.ProjectileType<ColossusPhantom>(), (int)(Projectile.damage * 0.2f), 4, Projectile.owner);
                }
            }
            else
            {
                ScreenShake.screenshakeMagnitude = 4;
                ScreenShake.screenshakeTimer = 10;
            }

            npc.AddBuff(ModContent.BuffType<Shred>(), 180);
        }
       
        
	}
}