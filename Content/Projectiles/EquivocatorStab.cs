
using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles
{
	public class EquivocatorStab : ModProjectile
	{
		// Define the range of the Spear Projectile. These are overridable properties, in case you'll want to make a class inheriting from this one.
		protected virtual float HoldoutRangeMin => 24f;
		protected virtual float HoldoutRangeMax => 300f;

		public override void SetDefaults() {
			Projectile.CloneDefaults(ProjectileID.Spear); // Clone the default values for a vanilla spear. Spear specific values set for width, height, aiStyle, friendly, penetrate, tileCollide, scale, hide, ownerHitCheck, and melee.
		}
        
        private bool hasSpawned = false;
        private int ProjShootTimer = 0;

		public override bool PreAI() {
            ProjShootTimer++;
            Player player = Main.player[Projectile.owner];
            player.heldProj = Projectile.whoAmI;

            // Get animation duration factoring in melee speed
            int duration = player.itemAnimationMax;

            if (Projectile.timeLeft > duration) {
                Projectile.timeLeft = duration;
            }

            Vector2 ToMouse = Main.MouseWorld - Projectile.Center;
            Projectile.velocity = Vector2.Normalize(Projectile.velocity);

            float halfDuration = duration * 0.5f;
            float progress;

            if (Projectile.timeLeft < halfDuration) {
                progress = Projectile.timeLeft / halfDuration;
            } else {
                progress = (duration - Projectile.timeLeft) / halfDuration;
            }

            // Adjust the threshold to account for float rounding or quicker attacks
            if (!hasSpawned && ProjShootTimer >= 4)
            {
                Vector2 direction = ToMouse.SafeNormalize(Vector2.UnitY);

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    direction * 6f,
                    ModContent.ProjectileType<EquivocatorAreaParticle>(),
                    60,
                    0f,
                    player.whoAmI
                );

                hasSpawned = true;
                ProjShootTimer = 0;
            }

            // Position spear between HoldoutRangeMin and Max depending on progress
            Projectile.Center = player.MountedCenter + Vector2.SmoothStep(
                Projectile.velocity * HoldoutRangeMin,
                Projectile.velocity * HoldoutRangeMax,
                progress
            );

            if (Projectile.spriteDirection == -1) {
                Projectile.rotation += MathHelper.ToRadians(45f);
            } else {
                Projectile.rotation += MathHelper.ToRadians(135f);
            }

            return false;
        }


	}
}