using CalamityMod.Buffs.StatDebuffs;
using DestroyerTest.Common;
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
	public class OverlordSwing : ModProjectile
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

		public List<float> TrailIndex = new List<float>();
		public List<Vector2> TrailIndexPos = new List<Vector2>();

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

		private float prepTime => 8f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
		private float execTime => 36f / Owner.GetTotalAttackSpeed(Projectile.DamageType);
		private float hideTime => 8f / Owner.GetTotalAttackSpeed(Projectile.DamageType);

		public override string Texture => "FranciumCalamityWeapons/Content/Projectiles/OverlordSwing";
		private Player Owner => Main.player[Projectile.owner];

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
			ProjectileID.Sets.TrailCacheLength[Type] = 15;
			ProjectileID.Sets.TrailingMode[Type] = 2;
		}

		public override void SetDefaults() {
			Projectile.width = 174;
			Projectile.height = 174;
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
			TrailIndex.Add(Projectile.rotation);
			TrailIndexPos.Add(Projectile.position);

			if (TrailIndex.Count > 260)
			{
				TrailIndex.RemoveAt(0);
			}
			if (TrailIndexPos.Count > 260)
			{
				TrailIndexPos.RemoveAt(0);
			}

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


			Vector2 basePoint = Projectile.Center;
			Vector2 tipPoint = swordTip; // ← your second point

			Vector2 direction = (tipPoint - basePoint).SafeNormalize(Vector2.UnitY);
			Vector2 normal = direction.RotatedBy(MathHelper.PiOver2); // 90° offset

			float width = 16f;

			Vector2 left = tipPoint + normal * width;
			Vector2 right = tipPoint - normal * width;

			// Load your trail texture
			Texture2D trailTex = ModContent.Request<Texture2D>("FranciumCalamityWeapons/Content/Extras/MotionTrail1").Value;

			VertexPositionColorTexture[] verts = new VertexPositionColorTexture[3];
			Color bladeColor = Color.Red; // leave it white if you want to keep the texture's original colors

			verts[0] = new VertexPositionColorTexture(new Vector3(basePoint, 0f), bladeColor, new Vector2(0.5f, 1f));
			verts[1] = new VertexPositionColorTexture(new Vector3(left, 0f), bladeColor, new Vector2(0f, 0f));
			verts[2] = new VertexPositionColorTexture(new Vector3(right, 0f), bladeColor, new Vector2(1f, 0f));

			// Draw
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			Main.graphics.GraphicsDevice.Textures[0] = trailTex;

			RasterizerState rasterizerState = new RasterizerState { CullMode = CullMode.None };
			Main.graphics.GraphicsDevice.RasterizerState = rasterizerState;

			BasicEffect effect = new BasicEffect(Main.graphics.GraphicsDevice)
			{
				VertexColorEnabled = true,
				TextureEnabled = true,
				Texture = trailTex,
				World = Matrix.Identity,
				View = Main.GameViewMatrix.TransformationMatrix,
				Projection = Matrix.CreateOrthographicOffCenter(
					0, Main.instance.GraphicsDevice.Viewport.Width,
					Main.instance.GraphicsDevice.Viewport.Height, 0,
					0, 1)
			};

			foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			{
				pass.Apply();
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, verts, 0, 1);
			}

			// Restore SpriteBatch
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, rasterizerState, null, Main.GameViewMatrix.TransformationMatrix);

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
					SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/OverlordSwing") with { PitchVariance = 1.0f });
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

		public static Vector2 CubicBezier(Vector2 start, Vector2 control1, Vector2 control2, Vector2 end, float t)
		{
			float u = 1 - t;
			return (u * u * u * start) + (3 * u * u * t * control1) + (3 * u * t * t * control2) + (t * t * t * end);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Color IceColor = new Color(39, 151, 171);
			Color FireColor = new Color(252, 109, 202);

			float lerpAmount = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)));
			Color entityhitcolor = Color.Lerp(IceColor, FireColor, lerpAmount);
			Player player = Main.LocalPlayer;
			player.GetModPlayer<ScreenshakePlayer>().screenshakeMagnitude = 9;
			player.GetModPlayer<ScreenshakePlayer>().screenshakeTimer = 12;
			Lighting.AddLight(target.Center, entityhitcolor.ToVector3() * 0.8f);
			SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/CosmicStarSpawn") with { PitchVariance = 1.0f, Volume = 1.5f });
			SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/CalamityBell") with { PitchVariance = 1.0f, Volume = 3.0f });
			Vector2 Flamedirection = new Vector2((float)Math.Cos(MathHelper.ToRadians(90)), (float)Math.Sin(MathHelper.ToRadians(90)));
			Vector2 Frostdirection = new Vector2((float)Math.Cos(MathHelper.ToRadians(270)), (float)Math.Sin(MathHelper.ToRadians(270)));
			Projectile.NewProjectile(Entity.GetSource_OnHit(target), Projectile.Center, Flamedirection, ModContent.ProjectileType<CosmicStarPink>(), 100, 8, Main.myPlayer);

			Projectile.NewProjectile(Entity.GetSource_OnHit(target), Projectile.Center, Frostdirection, ModContent.ProjectileType<CosmicStarBlue>(), 100, 8, Main.myPlayer);

			PRTLoader.NewParticle(PRTLoader.GetParticleID<Boom3>(), target.Center, Vector2.Zero, entityhitcolor, 1);
			if (hit.Crit)
			{
				target.AddBuff(ModContent.BuffType<WhisperingDeath>(), 360);
			}

			var modPlayer = player.GetModPlayer<OverlordCountPlayer>();

			modPlayer.HitCount += 1;
			modPlayer.DecayStartTimer = 0;

			
		}
	}
}
