using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using InnoVault.PRT;
using Terraria.DataStructures;
using FranciumCalamityWeapons.Content.Projectiles;
using FranciumCalamityWeapons.Content.Particles;

namespace FranciumCalamityWeapons.Content.Scepter
{
    public class SulfurScepterHoldout : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 148;
            Projectile.height = 132;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 16000; // persistent
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.netImportant = true;
			Projectile.netUpdate = true;

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
        int ShootInterval = 0;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // Check if the player is holding the item and channeled
            if (player.HeldItem.type == ModContent.ItemType<SulfurScepter>() && player.channel)
            {
                Vector2 ShootDir = Projectile.DirectionTo(Main.MouseWorld);
                Vector2 KickbackDir = (-Projectile.DirectionTo(Main.MouseWorld)) * 4.5f;
                Rotations();
                if (ShootInterval <= 0)
                {
                    float PRTDist = 140f;
                    Vector2 mountedCenter = player.MountedCenter;
                    Vector2 toCursor = Main.MouseWorld - mountedCenter;
                    toCursor.Normalize();
                    Vector2 PRTPos = mountedCenter + toCursor * PRTDist;
                    PRTLoader.NewParticle(PRTLoader.GetParticleID<BloomRingSharp>(), PRTPos, Vector2.Zero, new Color(140, 234, 87), 2f);
                    for (int i = 0; i < 3; i++)
                    {
                        float angleOffset = Main.rand.NextFloat(-0.3f, 0.3f);
                        ShootDir = ShootDir.RotatedBy(angleOffset);
                        ShootDir *= Main.rand.NextFloat(0.8f, 1.2f);
                        ShootDir.Normalize();
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), PRTPos, ShootDir * 8f, ModContent.ProjectileType<CausticShot>(), 8, 2f, player.whoAmI);
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        float angleOffset = Main.rand.NextFloat(-0.3f, 0.3f);
                        ShootDir = ShootDir.RotatedBy(angleOffset);
                        ShootDir *= Main.rand.NextFloat(0.8f, 1.2f);
                        ShootDir.Normalize();
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), PRTPos, ShootDir * 8f, ModContent.ProjectileType<CausticBlob>(), 8, 2f, player.whoAmI);
                    }
                    for (int t = 0; t < 14; t++)
                    {
                        PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticle2>(), PRTPos, new Vector2(Main.rand.NextFloat(1, -1), -4), new Color(140, 234, 87), Main.rand.NextFloat(0.4f, 1.4f));
                    }
                    player.velocity += KickbackDir;
                    SoundEngine.PlaySound(SoundID.NPCDeath53, Projectile.position);
                    ShootInterval = 80;
                }
                if (ShootInterval > 0)
                {
                    ShootInterval--;
                }
            }
            else
            {
                // Kill the projectile if the item is not being held
                Projectile.Kill();
            }
        }

        public void Rotations()
        {
            Player player = Main.player[Projectile.owner];
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
        }

		
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

    }
}