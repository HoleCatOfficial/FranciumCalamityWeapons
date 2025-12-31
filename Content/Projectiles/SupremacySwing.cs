using CalamityMod.Buffs.StatDebuffs;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using FranciumCalamityWeapons.Common;
using FranciumCalamityWeapons.Content.Melee;
using FranciumCalamityWeapons.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
	public class SupremacySwing : ModProjectile
	{
		private const float SWINGRANGE = 1.67f * (float)Math.PI;
		private const float SPINRANGE = 4.5f * (float)Math.PI;
		private const float WINDUP = 0.15f;
		private const float UNWIND = 0.4f;
		private const float SPINTIME = 2.0f;

		private enum AttackType
		{
			Spin, //This has no purpose in the code anymore beyond its usage in CurrentAttack.
		}

		private enum AttackStage
		{
			Prepare,
			Execute,
			Unwind
		}

		private AttackType CurrentAttack {
			get => (AttackType)Projectile.ai[0];
			set => Projectile.ai[0] = (float)value;
		}

		private AttackStage CurrentStage {
			get => (AttackStage)Projectile.localAI[0];
			set {
				Projectile.localAI[0] = (float)value;
				Timer = 0;
			}
		}

		private ref float InitialAngle => ref Projectile.ai[1];
		private ref float Timer => ref Projectile.ai[2];
		private ref float Progress => ref Projectile.localAI[1];
		private ref float Size => ref Projectile.localAI[2];

		private float prepTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
		private float execTime => 48f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
		private float hideTime => 12f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
		private Player Owner => Main.player[Projectile.owner];

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
			ProjectileID.Sets.TrailCacheLength[Type] = 15;
			ProjectileID.Sets.TrailingMode[Type] = 2;
		}

		public override void SetDefaults() {
			Projectile.width = 100;
			Projectile.height = 100;
			Projectile.friendly = true;
			Projectile.timeLeft = 10000;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
			Projectile.ownerHitCheck = true;
			Projectile.DamageType = DamageClass.Melee;
		}

		public override void OnSpawn(IEntitySource source) {
			Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
			float targetAngle = (Main.MouseWorld - Owner.MountedCenter).ToRotation();

			
			InitialAngle = (float)(-Math.PI / 2 - Math.PI * 1 / 3 * Projectile.spriteDirection);
			
		}

		public override void SendExtraAI(BinaryWriter writer) {
			writer.Write((sbyte)Projectile.spriteDirection);
		}

		public override void ReceiveExtraAI(BinaryReader reader) {
			Projectile.spriteDirection = reader.ReadSByte();
		}

		public override void AI() {

			Owner.itemAnimation = 2;
			Owner.itemTime = 2;

			if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed) {
				Projectile.Kill();
				return;
			}

			switch (CurrentStage) {
				case AttackStage.Prepare:
					PrepareStrike();
					break;
				case AttackStage.Execute:
					ExecuteStrike();
					break;
				default:
					UnwindStrike();
					break;
			}

			SetSwordPosition();
			Timer++;
		}

		public Vector2 swordTip;

		public override bool PreDraw(ref Color lightColor)
		{

			Vector2 origin;
			float rotationOffset;
			SpriteEffects effects;

			if (Projectile.spriteDirection > 0)
			{
				origin = new Vector2(0, Projectile.height);
				rotationOffset = MathHelper.ToRadians(45f);
				effects = SpriteEffects.None;
			}
			else
			{
				origin = new Vector2(Projectile.width, Projectile.height);
				rotationOffset = MathHelper.ToRadians(135f);
				effects = SpriteEffects.FlipHorizontally;
			}

			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);

			return false;
		}

        public override bool PreDrawExtras()
        {
			
			return false;
        }


		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			Vector2 start = Owner.MountedCenter;
			Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * Projectile.scale);
			float collisionPoint = 0f;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 25f * Projectile.scale, ref collisionPoint);
		}

		public override void CutTiles() {
			Vector2 start = Owner.MountedCenter;
			Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
			Utils.PlotTileLine(start, end, 15 * Projectile.scale, DelegateMethods.CutTiles);
		}

		public override bool? CanDamage() {
			if (CurrentStage == AttackStage.Prepare)
				return false;
			return base.CanDamage();
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
			modifiers.HitDirectionOverride = target.position.X > Owner.MountedCenter.X ? 1 : -1;
			modifiers.Knockback += 1;
		}

		public void SetSwordPosition()
		{
			Projectile.rotation = InitialAngle + Projectile.spriteDirection * Progress;

			Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f));
			Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2);


			armPosition.Y += Owner.gfxOffY;
			Projectile.Center = armPosition;
			Projectile.scale = Size * 1.2f * Owner.GetAdjustedItemScale(Owner.HeldItem);

			Owner.heldProj = Projectile.whoAmI;
			swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
		}

		public bool TakeoutSoundPlayed = false;

		private void PrepareStrike()
		{
			swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
			Progress = WINDUP * SWINGRANGE * (1f - Timer / prepTime);
			Size = MathHelper.SmoothStep(0, 1, Timer / prepTime);
			if (TakeoutSoundPlayed == false)
			{
				SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/OverlordTakeout") with { PitchVariance = 1.0f });
				TakeoutSoundPlayed = true;
			}

			if (Timer >= prepTime)
				{
					SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/MagicSwing", 3) with { PitchVariance = 1.0f });
					CurrentStage = AttackStage.Execute;
				}
		}

		private void ExecuteStrike() {
			Player player = Main.player[Projectile.owner];

			swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);

			Progress = MathHelper.SmoothStep(0, SPINRANGE, (1f - UNWIND / 2) * Timer / (execTime * SPINTIME));

			if (Timer == (int)(execTime * SPINTIME * 3 / 4)) {
				SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/OverlordSwing") with { PitchVariance = 1.0f });
				Projectile.ResetLocalNPCHitImmunity();
			}

			if (Timer >= execTime * SPINTIME) {
				CurrentStage = AttackStage.Unwind;
			}
			
		}

		private void UnwindStrike() {
			swordTip = Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
			Progress = MathHelper.SmoothStep(0, SPINRANGE, (1f - UNWIND / 2) + UNWIND / 2 * Timer / (hideTime * SPINTIME / 2));
			Size = 1f - MathHelper.SmoothStep(0, 1, Timer / (hideTime * SPINTIME / 2));

			if (Timer >= hideTime * SPINTIME / 2) {
				Projectile.Kill();
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Player player = Main.LocalPlayer;
			SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/CosmicWrathEnchantmentDeath") with { PitchVariance = 1.0f, Volume = 3.0f });
            PRTLoader.NewParticle(PRTLoader.GetParticleID<SmallShine>(), target.Center, Vector2.Zero, Color.White, 2);
		}
	}
}
