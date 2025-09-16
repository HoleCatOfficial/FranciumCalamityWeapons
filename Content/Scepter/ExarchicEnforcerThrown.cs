
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

namespace FranciumCalamityWeapons.Content.Scepter
{
    public class ExarchicEnforcerThrown : ModProjectile
    {
        public Color ThemeColor { get; set; }
        public int WidthDim { get; set; }
        public int HeightDim { get; set; }
        public int DustType { get; set; }
        public int soundCooldown = 0;
        public bool returning = false;
        public ModProjectile VoidStar1;
        public static readonly ColorPalette VoidBarColors = new ColorPalette("Void Bar", new Color(32, 13, 61), new Color(79, 30, 109), new Color(169, 79, 255), new Color(69, 215, 255), new Color(255, 69, 254));
        public override void SetDefaults()
        {
            ThemeColor = Color.White;
            WidthDim = 148;
            HeightDim = 132;
            DustType = DustID.Glass;

            Projectile.width = WidthDim + ScepterClassStats.SizeModifier;
            Projectile.height = HeightDim + ScepterClassStats.SizeModifier;
            Projectile.friendly = true;
            Projectile.penetrate = -1; // Infinite pierce
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180; // 10 seconds max lifespan
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.netImportant = true;
			Projectile.netUpdate = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation += 0.8f * Projectile.direction;
            // Generate flying dust effect
            if (Main.rand.NextBool(3)) // 33% chance per tick
            {
                if (Main.rand.NextBool(2)) // 50% chance to use the first color
                {
                    PRTLoader.NewParticle(PRTLoader.GetParticleID<StarParticle>(), Projectile.Center, Vector2.Zero, VoidBarColors.Colors[3], 1f);
                }
                else
                {
                    PRTLoader.NewParticle(PRTLoader.GetParticleID<StarParticle>(), Projectile.Center, Vector2.Zero, VoidBarColors.Colors[4], 1f);
                }
            }

            if (ModLoader.TryGetMod("CalamityEntropy", out Mod CLE))
            {
                if (CLE.TryFind("VoidStarF", out ModProjectile VS))
                {
                    VoidStar1 = VS;
                }
            }
            else
            {
                VoidStar1 = ModContent.GetModProjectile(ModContent.ProjectileType<VoidStar2>());
            }

            if (Main.rand.NextBool(40))
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 Direction = Main.rand.NextVector2CircularEdge(1f, 1f); // Random unit vector on circle edge
                    Vector2 velocity = Direction * 24f; // 6f = desired projectile speed

                    Projectile.NewProjectile(
                        Entity.GetSource_FromThis(),
                        Projectile.Center,
                        velocity,
                        VoidStar1.Type,
                        (int)(Projectile.damage * 0.5f),
                        (int)(Projectile.knockBack * 0.5f),
                        Projectile.owner
                    );
                }
            }

            if (!returning)
            {
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

            if (Projectile.timeLeft <= 60)
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
            if (ModLoader.TryGetMod("CalamityEntropy", out Mod CLE))
            {
                if (CLE.TryFind("VoidTouch", out ModBuff VT))
                {
                    target.AddBuff(VT.Type, 120);
                }

                if (CLE.TryFind("VoidStarF", out ModProjectile VS))
                {
                    VoidStar1 = VS;
                }
            }
            else
            {
                target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
                VoidStar1 = ModContent.GetModProjectile(ModContent.ProjectileType<VoidStar2>());
            }
            

            SoundEngine.PlaySound(Hit, target.Center);

            Vector2 launchVelocity = new Vector2(-8, 0); // Create a velocity moving the left.
            for (int i = 0; i < Main.rand.Next(3, 15); i++)
            {
                // Every iteration, rotate the newly spawned projectile by the equivalent 1/4th of a circle (MathHelper.PiOver4)
                // (Remember that all rotation in Terraria is based on Radians, NOT Degrees!)
                launchVelocity = launchVelocity.RotatedBy(MathHelper.PiOver4);

                // Spawn a new projectile with the newly rotated velocity, belonging to the original projectile owner. The new projectile will inherit the spawning source of this projectile.
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, launchVelocity * 2.75f, VoidStar1.Type, (int)(Projectile.damage * 0.25f), Projectile.knockBack, Projectile.owner);
            }
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}


