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
using FranciumCalamityWeapons.Content.Debuffs;

namespace FranciumCalamityWeapons.Content.Projectiles.Ammo
{
	public class ArsenalArrowT3Spark : ModProjectile
	{
        public override string Texture => DTUtils.NoTexture;
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
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 32;
			Projectile.height = 32;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.ignoreWater = true;
			Projectile.light = 0.4f;
			Projectile.timeLeft = 600;
			Projectile.tileCollide = false;
		}

		public Color PhaseSlayerOrange = new Color(255, 64, 31);
        public override bool? CanHitNPC(NPC target)
        {
            return DelayTimer >= 25;
        }

		public override void AI()
		{
            Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Vector2.Zero, 0, PhaseSlayerOrange, 1f);
			if (DelayTimer < 25)
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
			Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(20)).ToRotationVector2() * length;
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
            target.AddBuff(ModContent.BuffType<ArsenalArrowShock>(), 600);
        }


	}
}