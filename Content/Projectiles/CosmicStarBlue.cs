using System;
using System.Collections.Generic;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Buffs.DamageOverTime;
using DestroyerTest.Common.Interfaces;
using System.Linq;
using BreadLibrary.Core.Utilities;
using CalamityMod.Buffs.StatDebuffs;

namespace FranciumCalamityWeapons.Content.Projectiles
{
	public class CosmicStarBlue : ModProjectile, IHomingProjectile
	{

		public float DelayTimer;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
			ProjectileID.Sets.TrailCacheLength[Type] = 50;
			ProjectileID.Sets.TrailingMode[Type] = 3;

        }

		public override void SetDefaults()
		{
			Projectile.width = 33;
			Projectile.height = 33;
			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.light = 0.4f;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = false;
		}

		public Color CosmicBlue = new Color(38, 148, 237);

		public float trailOffset = 0f;

		bool IHomingProjectile.TracksNPCs => true;

		bool IHomingProjectile.TracksPlayers => false;

		float IHomingProjectile.HomingTurnSpeed => 44;

		bool IHomingProjectile.UsesHomingAcceleration => true;

		float IHomingProjectile.HomingAccelAmount => 1.04f;

		float IHomingProjectile.HomingMaxAccel => 20f;

        float IHomingProjectile.DetectRadius => 1400;

        bool IHomingProjectile.CanHome => DelayTimer >= 25;

        public override bool PreDraw(ref Color lightColor)
		{
			trailOffset -= 0.04f;


			SpriteBatch spriteBatch = Main.spriteBatch;
			DTUtils Utility = new DTUtils();

            DTTrail.DrawTrail(spriteBatch, DTAssetLib.Streak(10).Value, Projectile.OldCenter().ToList(), Projectile.oldRot.ToList(), 40, CosmicBlue, trailOffset);

			spriteBatch.UseBlendState(BlendState.Additive);

            Opus.DrawGlowOnProj(Projectile, CosmicBlue, true);

			Opus.DrawTextureOnProj(DTAssetLib.Star(3), Projectile, DTColorUtils.Pastel(CosmicBlue, 0.5f), true, 0f, 0.9f, 0.9f);

			spriteBatch.ResetToDefault();

			return false;
		}

        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 25;
        }


		public override void AI()
		{
            Projectile.ResetExcessTrailPoints();
            Lighting.AddLight(Projectile.Center, CosmicBlue.ToVector3() * 0.2f);

			Projectile.rotation += (Projectile.velocity.Length() * 0.05f) * Projectile.direction;

			if (DelayTimer < 25)
			{
				DelayTimer += 1;
				return;
			}
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Voidfrost>(), 600);
        }
	}
}