using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using InnoVault.PRT;
using Terraria.DataStructures;
using FranciumCalamityWeapons.Content.Projectiles;
using FranciumCalamityWeapons.Content.Scepter;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    public class GodSlayerScepterHoldout : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 84;
            Projectile.height = 84;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 16000; // persistent
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effects = SpriteEffects.None;
            Player player = Main.player[Projectile.owner];

            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);

            Main.spriteBatch.Draw(
                texture,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                effects,
                0f);

            // Return false to prevent default drawing, since we handled it
            return false;
        }

        bool flipRotation = false;
        public SoundStyle Shoot = new SoundStyle("FranciumCalamityWeapons/Audio/CosmicStarSpawn") with { MaxInstances = 0, PitchVariance = 0.3f };
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // Check if the player is holding the item and channeled
            if (player.HeldItem.type == ModContent.ItemType<GodSlayerScepter>() && player.channel)
            {
                // Lock the projectile's position relative to the player
                float holdDistance = 100f;
                Vector2 mountedCenter = player.MountedCenter;
                Vector2 toCursor = Main.MouseWorld - mountedCenter;
                toCursor.Normalize();
                Vector2 desiredPos = mountedCenter + toCursor * holdDistance;
                Projectile.Center = desiredPos;

                float angle = toCursor.ToRotation() + MathHelper.PiOver4;

                // Flip logic based on direction or cursor position
                flipRotation = toCursor.X < 0f;
                Projectile.rotation = angle;

                if (flipRotation)
                {
                    //Projectile.rotation -= MathHelper.Pi; // 180° flip
                }

                // Now calculate arm angle with a visually corrected offset
                float armAngle = angle;

                // Tweak for left/right symmetry
                if (player.direction == 1)
                {
                    // Facing right: back off by 1/8 circle
                    armAngle -= (MathHelper.Pi - MathHelper.PiOver4) ;  // ~22.5°
                }
                else
                {
                    // Facing left: back off by 1/16 circle
                    armAngle += MathHelper.Pi / 16f; // ~11.25°
                    armAngle += MathHelper.Pi + MathHelper.PiOver4; // because of the flip
                }

                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armAngle);
                player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armAngle);

                Vector2 shotPos = mountedCenter + toCursor * 120;
                Projectile.ai[0]++;
                if (Projectile.ai[0] % 10 == 0)
                {
                    SoundEngine.PlaySound(Shoot, Projectile.Center);
                    int PinkOrBlue = Main.rand.NextBool(2) ? ModContent.ProjectileType<CosmicStarPink>() : ModContent.ProjectileType<CosmicStarBlue>();
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), shotPos, toCursor, PinkOrBlue, Projectile.damage, 10, Projectile.owner);
                }
            }
            else
            {
                // Kill the projectile if the item is not being held
                Projectile.Kill();
            }
        }

		
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
            
		}

    }
}