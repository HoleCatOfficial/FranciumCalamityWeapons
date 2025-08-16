using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles
{
	public class HeroSwordLaser : ModProjectile {
		public float Distance {
			get {
				return Projectile.ai[0];
			}
			set {
				Projectile.ai[0] = value;
			}
		}

		public float Charge {
			get {
				return Projectile.localAI[0];
			}
			set {
				Projectile.localAI[0] = value;
			}
		}

		public bool IsAtMaxCharge {
			get {
				return Charge == 90f;
			}
		}

		public override void SetDefaults() {
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.hide = true;
		}

		public override bool PreDraw(ref Color lightColor) {
			if (IsAtMaxCharge) {
				DrawLaser(Main.spriteBatch, TextureAssets.Projectile[Projectile.type].Value, Main.player[Projectile.owner].Center, Projectile.velocity, 10f, Projectile.damage, -1.57f, 1f, 1000f, Color.White, 60);
			}
			return false;
		}

		public void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 unit, float step, int damage, float rotation = 0f, float scale = 1f, float maxDist = 1200f, Color color = default, int transDist = 50) {
			float num = unit.ToRotation() + rotation;
			for (float num2 = transDist; num2 <= Distance; num2 += step) {
				Color white = Color.White;
				Vector2 vector = start + num2 * unit;
				Main.EntitySpriteDraw(texture, vector - Main.screenPosition, new Rectangle?(new Rectangle(0, 26, 28, 26)), num2 < transDist ? Color.Transparent : white, num, new Vector2(14f, 13f), scale, 0, 0f);
			}
			Main.EntitySpriteDraw(texture, start + unit * (transDist - step) - Main.screenPosition, new Rectangle?(new Rectangle(0, 0, 28, 26)), Color.White, num, new Vector2(14f, 13f), scale, 0, 0f);
			Main.EntitySpriteDraw(texture, start + (Distance + step) * unit - Main.screenPosition, new Rectangle?(new Rectangle(0, 52, 28, 26)), Color.White, num, new Vector2(14f, 13f), scale, 0, 0f);
		}
		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			if (!IsAtMaxCharge) {
				return new bool?(false);
			}
			Player player = Main.player[Projectile.owner];
			Vector2 velocity = Projectile.velocity;
			float num = 0f;
			return new bool?(Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center, player.Center + velocity * Distance, 22f, ref num));
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			target.immune[Projectile.owner] = 6;
			if (Projectile.damage > 30) {
				Projectile.damage = (int)(Projectile.damage * 0.99f);
				return;
			}
			if (Projectile.damage <= 30) {
				Projectile.damage = 30;
			}
            target.AddBuff(BuffID.Electrified, 120);
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
			if (Projectile.damage <= 50) {
				int num = 50 - Projectile.damage + 10;
				modifiers.ArmorPenetration += num;
				return;
			}
			if (Projectile.damage <= 30) {
				modifiers.ArmorPenetration += 30f;
			}
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];
			Projectile.position = player.Center + Projectile.velocity * 60f;
			Projectile.timeLeft = 2;
			beamDuration--;
			Projectile.localAI[1] += 1f;
			UpdatePlayer(player);
			ChargeLaser(player);
			if (Charge < 90f) {
				return;
			}
			SetLaserPosition(player);
			CastLights();
			if (!beamBurst) {
				SoundEngine.PlaySound(SoundID.Item67, new Vector2?(player.position), null);
				Vector2 vector = Projectile.velocity;
				vector *= 40f;
				Vector2 vector2 = player.Center + vector - new Vector2(10f, 10f);
				Vector2 vector3 = Vector2.UnitX * 18f;
				vector3 = vector3.RotatedBy((double)(Projectile.rotation - 1.57f), default);
				Vector2 vector4 = Projectile.Center + vector3;
				for (int i = 0; i < 40; i++) {
					Vector2 vector5 = vector4 + ((float)Main.rand.NextDouble() * 6.28f).ToRotationVector2() * -4f;
					Dust dust = Main.dust[Dust.NewDust(vector2, 20, 20, DustID.HallowedWeapons, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f, 0, default, 1f)];
					dust.velocity = Vector2.Normalize(vector4 - vector5) * 1.5f * -6f / 10f;
					dust.velocity *= 5f;
					dust.noGravity = true;
					dust.scale = Main.rand.Next(15, 30) * 0.05f;
				}
				beamBurst = true;
			}
		}

		private void SetLaserPosition(Player player) {
			for (Distance = MOVE_DISTANCE; Distance <= 2200f; Distance += 5f) {
				var start = player.Center + Projectile.velocity * Distance;
				if (!Collision.CanHit(player.Center, 1, 1, start, 1, 1)) {
					Distance -= 5f;
					break;
				}
			}
		}

		private void ChargeLaser(Player player) {
			if (!player.channel) {
				Projectile.Kill();
				return;
			}
			if (Projectile.localAI[1] >= 10f) {
				Projectile.localAI[1] = 0f;
				if (!player.CheckMana(player.inventory[player.selectedItem].mana, true, false)) {
					Projectile.Kill();
				}
			}
			if (beamDuration <= 0) {
				Projectile.Kill();
			}
			Vector2 vector = Projectile.velocity;
			vector *= 40f;
			Vector2 vector2 = player.Center + vector - new Vector2(10f, 10f);
			if (Charge < 90f) {
				float charge = Charge;
				Charge = charge + 1f;
				int num = (int)(Charge / 15f);
				Vector2 vector3 = Vector2.UnitX * 18f;
				vector3 = vector3.RotatedBy((double)(Projectile.rotation - 1.57f), default);
				Vector2 vector4 = Projectile.Center + vector3;
				for (int i = 0; i < num + 1; i++) {
					Vector2 vector5 = vector4 + ((float)Main.rand.NextDouble() * 6.28f).ToRotationVector2() * (12f - num * 2);
					Dust dust = Main.dust[Dust.NewDust(vector2, 20, 20, DustID.HallowedWeapons, Projectile.velocity.X / 2f, Projectile.velocity.Y / 2f, 0, default, 1f)];
					dust.velocity = Vector2.Normalize(vector4 - vector5) * 1.5f * (10f - num * 2f) / 10f;
					dust.noGravity = true;
					dust.scale = Main.rand.Next(10, 20) * 0.05f;
				}
			}
		}

		private void UpdatePlayer(Player player) {
			if (Projectile.owner == Main.myPlayer) {
				Vector2 velocity = Main.MouseWorld - player.Center;
				velocity.Normalize();
				Projectile.velocity = velocity;
				Projectile.direction = Main.MouseWorld.X > player.position.X ? 1 : -1;
				Projectile.netUpdate = true;
			}
			int direction = Projectile.direction;
			player.ChangeDir(direction);
			player.heldProj = Projectile.whoAmI;
			player.itemTime = 2;
			player.itemAnimation = 2;
			player.itemRotation = (float)Math.Atan2((double)(Projectile.velocity.Y * direction), (double)(Projectile.velocity.X * direction));
		}
		private void CastLights() {
			// Cast a light along the line of the laser
			DelegateMethods.v3_1 = new Vector3(0.8f, 0.8f, 1f);
			Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * (Distance - MOVE_DISTANCE), 26, DelegateMethods.CastLight);
		}

		public override bool ShouldUpdatePosition() {
			return false;
		}

		private const float MAX_CHARGE = 90f;

		private const float MOVE_DISTANCE = 60f;

		private int beamDuration = 600;

		private bool beamBurst;
	}
}
