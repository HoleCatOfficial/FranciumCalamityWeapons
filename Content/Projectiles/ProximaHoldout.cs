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
using OpusLib;
using DestroyerTest.Common;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    public class ProximaHoldout : ModProjectile
    {
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 16000;
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

            return false;
        }

        bool flipRotation = false;
        public SoundStyle Shoot = SoundID.Item9;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.HeldItem.type == ModContent.ItemType<Proxima>() && player.channel)
            {
                float holdDistance = 50f;
                Vector2 mountedCenter = player.MountedCenter;
                Vector2 toCursor = Main.MouseWorld - mountedCenter;
                toCursor.Normalize();
                Vector2 desiredPos = mountedCenter + toCursor * holdDistance;
                Projectile.Center = desiredPos;

                float angle = toCursor.ToRotation() + MathHelper.PiOver4;

                flipRotation = toCursor.X < 0f;
                Projectile.rotation = angle;

                if (flipRotation)
                {

                }

                float armAngle = angle;

                if (player.direction == 1)
                {
                    armAngle -= (MathHelper.Pi - MathHelper.PiOver4);
                }
                else
                {
                    armAngle += MathHelper.Pi / 16f;
                    armAngle += MathHelper.Pi + MathHelper.PiOver4;
                }

                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armAngle);
                player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armAngle);

                Vector2 shotPos = mountedCenter + toCursor * 120;
                Projectile.ai[0]++;
                if (Projectile.ai[0] % 100 == 0)
                {
                    SoundEngine.PlaySound(Shoot, Projectile.Center);

                    Opus.RadialSpreadProjectile(ModContent.ProjectileType<ProximaCrescent>(), 4, player.MountedCenter, Projectile.damage / 4, 8, 8, RandomOffset: true);
                }
            }
            else
            {
                Projectile.Kill();
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

    }
}