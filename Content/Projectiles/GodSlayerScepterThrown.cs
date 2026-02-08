
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using System;
using Terraria.DataStructures;
using System.IO;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles;
using InnoVault.PRT;
using FranciumCalamityWeapons.Content.Particles;
using Microsoft.VisualBasic;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Melee;
using FranciumCalamityWeapons.Content.Projectiles;
using CalamityMod.Projectiles.Typeless;
using DestroyerTest.Content.Particles;
using OpusLib;
using CalamityMod.Dusts;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    public class GodSlayerScepterThrown : ModProjectile
    {
        public Color ThemeColor { get; set; }
        public int WidthDim { get; set; }
        public int HeightDim { get; set; }
        public int DustType { get; set; }
        public int soundCooldown = 0;
        public bool returning = false;
        public Color CosmicBlue = new Color(39, 151, 171);
        public Color CosmicPink = new Color(252, 109, 202);
        public override void SetDefaults()
        {
            ThemeColor = new Color(Opus.Sine(CosmicBlue.R, CosmicPink.R), Opus.Sine(CosmicBlue.G, CosmicPink.G), Opus.Sine(CosmicBlue.B, CosmicPink.B));
            WidthDim = 84;
            HeightDim = 84;
            DustType = ModContent.DustType<CosmiliteBarDust>();

            Projectile.width = WidthDim + ScepterClassStats.SizeModifier;
            Projectile.height = HeightDim + ScepterClassStats.SizeModifier;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 600;
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        
        SoundStyle Rot = new SoundStyle("DestroyerTest/Assets/Audio/SwordSounds/MagicSwing", 3) with { Volume = 0.75f, PitchVariance = 0.8f, MaxInstances = 0 };
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation += 0.8f * Projectile.direction;

            Vector2 dustPoint = Projectile.Center + new Vector2(0, 40);
            dustPoint = dustPoint.RotatedBy(Projectile.rotation - MathHelper.PiOver4);

            Rectangle DustRect = Utils.CenteredRectangle(dustPoint, new Vector2(10, 10));

            Dust.NewDust(DustRect.TopLeft(), DustRect.Width, DustRect.Height, DustID.FireworksRGB, 0f, 0f, 0, CosmicBlue, 0.5f);
            
            // Generate flying dust effect
            if (Main.rand.NextBool(3))
            {
                if (Main.rand.NextBool(2))
                {
                    PRTLoader.NewParticle(PRTLoader.GetParticleID<StarParticle>(), Projectile.Center, Projectile.velocity * 0.25f, CosmicBlue, 1f);
                }
                else
                {
                    PRTLoader.NewParticle(PRTLoader.GetParticleID<StarParticle>(), Projectile.Center, Projectile.velocity * 0.25f, CosmicPink, 1f);
                }
            }

            if (Main.GameUpdateCount % 10 == 0)
            {
                SoundEngine.PlaySound(Rot, Projectile.Center);
            }

            if (Projectile.ai[2] > 0)
            {
                Projectile.ai[2]--;
            }

            if (player.controlUseTile && !returning)
            {
                Projectile.timeLeft = 600;
                Vector2 toMouse = Main.MouseWorld - Projectile.Center;
                if (toMouse.Length() > 48f)
                {
                    toMouse.Normalize();
                    toMouse *= 1f; // acceleration speed

                    Projectile.velocity += toMouse;
                    if (Projectile.velocity.Length() > 24f) // cap the speed
                        Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 24f;
                }
                else
                {
                    Vector2 direction = Main.MouseWorld - Projectile.Center;
                    Projectile.velocity = direction * 0.8f; // or direction * some small factor
                }
            }

            if (!player.controlUseTile)
            {
                returning = true;
            }

            if (returning)
            {
                ArmCatchAnimate(player);
                // InPhase: Smooth return using Lerp
                Vector2 returnDirection = player.Center - Projectile.Center;
                float speed = MathHelper.Lerp(Projectile.velocity.Length(), 15f, 0.08f); // Smooth acceleration
                Projectile.velocity = returnDirection.SafeNormalize(Vector2.Zero) * speed;

                // If close enough, remove the projectile
                if (Projectile.Distance(player.Center) < 8) // 8 pixels radius
                {
                    Projectile.Kill();
                }
            }
        }

        public void ArmCatchAnimate(Player player)
        {
            // Calculate the direction vector from the player to the projectile
            Vector2 directionToProjectile = Projectile.Center - player.Center;

            // Normalize the direction vector to get a unit vector
            directionToProjectile.Normalize();

            // Calculate the angle between the player's direction and the direction to the projectile
            float angleDifference = MathHelper.WrapAngle(directionToProjectile.ToRotation() - player.direction * MathHelper.PiOver2);

            // Adjust arm rotation based on the player's facing direction
            if (player.direction == 1)
            {
                // Player is facing right, so we use the angle difference as is
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, angleDifference);
            }
            else if (player.direction == -1)
            {
                // Player is facing left, so flip the angle by pi (180 degrees) to reach the opposite direction
                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, angleDifference + MathHelper.Pi);
            }
        }


        SoundStyle Hit = new SoundStyle("FranciumCalamityWeapons/Audio/VoidScepterHit") with { Volume = 0.75f, PitchVariance = 0.8f };

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[2] <= 0)
            {
                SoundEngine.PlaySound(Hit, target.Center);
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SmallShine>(), target.Center, Vector2.Zero, Color.White, 1);
                int PinkOrBlue = Main.rand.NextBool(2) ? ModContent.ProjectileType<CosmicStarPink>() : ModContent.ProjectileType<CosmicStarBlue>();
                Opus.RadialSpreadProjectile(PinkOrBlue, 12, target.Center, Projectile.damage / 4, 0, 8, RandomOffset: false);
                Projectile.ai[2] = 20;
            }
        }
    }
}


