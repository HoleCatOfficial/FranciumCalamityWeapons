using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using Terraria.DataStructures;
using System.IO;
using tModPorter;
using System;
using Terraria.ModLoader.IO;
using System.Collections.Generic;
using InnoVault.PRT;
using FranciumCalamityWeapons.Content.Particles;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    public class SupremacyWyrmHead : ModProjectile
    {
        private bool spawned;

        private NPC HomingTarget
        {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set
            {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }

        public ref float DelayTimer => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hide = false;
            Projectile.timeLeft = 300;
        }

        public float BaseSpeed;
        public override void OnSpawn(IEntitySource source)
        {
            BaseSpeed = Projectile.velocity.Length();
        }
        public bool HasDashed = false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];


            if (!spawned && Main.netMode != NetmodeID.MultiplayerClient)
            {
                int previous = Projectile.whoAmI;
                int segmentCount = 36;

                for (int i = 0; i < segmentCount; ++i)
                {
                    int type = ModContent.ProjectileType<SupremacyWyrmBody>();
                    if (i == segmentCount - 1)
                        type = ModContent.ProjectileType<SupremacyWyrmTail>();

                    previous = Projectile.NewProjectile(Projectile.GetSource_FromAI(),
                        Projectile.Center, Vector2.Zero, type,
                        0, 0f, player.whoAmI,
                        Projectile.whoAmI, previous);
                }
                spawned = true;
            }

            Vector2 LeftOffset = new Vector2(-10, 0).RotatedBy(Projectile.rotation);
            Vector2 RightOffset = new Vector2(10, 0).RotatedBy(Projectile.rotation);

            Vector2 Left = Projectile.Center + LeftOffset;
            Vector2 Right = Projectile.Center + RightOffset;


            int[] types = new int[]
            {
                PRTLoader.GetParticleID<BlackFire1>(),
                PRTLoader.GetParticleID<BlackFire2>(),
                PRTLoader.GetParticleID<BlackFire3>(),
                PRTLoader.GetParticleID<BlackFire4>(),
                PRTLoader.GetParticleID<BlackFire5>(),
                PRTLoader.GetParticleID<BlackFire6>(),
                PRTLoader.GetParticleID<BlackFire7>()
            };

            PRTLoader.NewParticle(types[Main.rand.Next(types.Length)], Left, Vector2.Zero, Color.Black * 0.025f, 0.01f);
            PRTLoader.NewParticle(types[Main.rand.Next(types.Length)], Right, Vector2.Zero, Color.Black * 0.025f, 0.01f);

            float maxDetectRadius = 400f; // The maximum radius at which a projectile can detect a target

            float DashRadius = 50f;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // A short delay to homing behavior after being fired
            if (DelayTimer < 10)
            {
                DelayTimer += 1;
                return;
            }

            

            // First, we find a homing target if we don't have one
            if (HomingTarget == null)
            {
                HomingTarget = FindClosestNPC(maxDetectRadius);
            }

            // If we have a homing target, make sure it is still valid. If the NPC dies or moves away, we'll want to find a new target
            if (HomingTarget != null && !IsValidTarget(HomingTarget))
            {
                HomingTarget = null;
            }

            // If we don't have a target, don't adjust trajectory
            if (HomingTarget == null)
                return;


            if (Projectile.Center.Distance(HomingTarget.Center) <= DashRadius)
            {
                if (HasDashed == false)
                {
                    SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/CalamityEntropy_AbyssBlade") with { PitchVariance = 0.5f }, Projectile.Center);
                    for (int i = 0; i < 100; i++)
                    {
                        float angle = MathHelper.TwoPi * i / 100f;
                        float radius = 40f + (float)Math.Sin(6 * angle) * 20f; // 6 = number of petals

                        Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
                        Vector2 position = Projectile.Center + offset;

                        Dust.NewDustPerfect(position, DustID.YellowTorch).noGravity = true;
                    }

                    Projectile.velocity *= 2;
                    HasDashed = true;
                }
            }

            if (Projectile.Center.Distance(HomingTarget.Center) > DashRadius)
            {
                if (HasDashed == true)
                {
                    if (Projectile.velocity.Length() > BaseSpeed)
                    {
                        Projectile.velocity *= 0.99f;
                    }
                }
                if (Projectile.velocity.Length() <= BaseSpeed)
                {
                    HasDashed = false;
                }
            }

            // If found, we rotate the projectile velocity in the direction of the target.
            // We only rotate by 3 degrees an update to give it a smooth trajectory. Increase the rotation speed here to make tighter turns
            float length = Projectile.velocity.Length();
            float targetAngle = Projectile.AngleTo(HomingTarget.Center);
            Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(15)).ToRotationVector2() * length;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        
        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null
        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            // Using squared values in distance checks will let us skip square root calculations, drastically improving this method's speed.
            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            // Loop through all NPCs
            foreach (var target in Main.ActiveNPCs)
            {
                // Check if NPC able to be targeted. 
                if (IsValidTarget(target))
                {
                    // The DistanceSquared function returns a squared distance between 2 points, skipping relatively expensive square root calculations
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

                    // Check if it is within the radius
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public bool IsValidTarget(NPC target) {
            // This method checks that the NPC is:
            // 1. active (alive)
            // 2. chaseable (e.g. not a cultist archer)
            // 3. max life bigger than 5 (e.g. not a critter)
            // 4. can take damage (e.g. moonlord core after all it's parts are downed)
            // 5. hostile (!friendly)
            // 6. not immortal (e.g. not a target dummy)
            // 7. doesn't have solid tiles blocking a line of sight between the projectile and NPC
            return target.CanBeChasedBy();
        }
    }

    public class SupremacyWyrmBody : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mini Gigapora");
            Main.projPet[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 48;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hide = false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile proj = Main.projectile[(int)Projectile.ai[0]];
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!proj.active || proj.type != ModContent.ProjectileType<SupremacyWyrmHead>())
                    Projectile.active = false;
            }
            if (Projectile.ai[1] >= 0 && Projectile.ai[1] < Main.maxProjectiles)
            {
                Projectile follow = Main.projectile[(int)Projectile.ai[1]];
                if (!follow.active)
                    return;

                Vector2 toFollow = follow.Center - Projectile.Center;
                float distance = toFollow.Length();

                // Maintain spacing of 48 pixels between segment centers
                float desiredSpacing = 27.5f;
                if (distance > 0f)
                {
                    float moveFactor = (distance - desiredSpacing) / distance;
                    Projectile.position += toFollow * moveFactor;
                }

                // Face the segment we're following
                Projectile.rotation = toFollow.ToRotation() + MathHelper.PiOver2;

                Projectile.velocity = Vector2.Zero;

                // Flip sprite based on direction
                Projectile.spriteDirection = (toFollow.X < 0f) ? 1 : -1;
            }

            Vector2 LeftOffset = new Vector2(-10, 0).RotatedBy(Projectile.rotation);
            Vector2 RightOffset = new Vector2(10, 0).RotatedBy(Projectile.rotation);

            Vector2 Left = Projectile.Center + LeftOffset;
            Vector2 Right = Projectile.Center + RightOffset;

            
            int[] types = new int[]
            {
                PRTLoader.GetParticleID<BlackFire1>(),
                PRTLoader.GetParticleID<BlackFire2>(),
                PRTLoader.GetParticleID<BlackFire3>(),
                PRTLoader.GetParticleID<BlackFire4>(),
                PRTLoader.GetParticleID<BlackFire5>(),
                PRTLoader.GetParticleID<BlackFire6>(),
                PRTLoader.GetParticleID<BlackFire7>()
            };

            PRTLoader.NewParticle(types[Main.rand.Next(types.Length)], Left, Vector2.Zero, Color.Black * 0.025f, 0.01f);
            PRTLoader.NewParticle(types[Main.rand.Next(types.Length)], Right, Vector2.Zero, Color.Black * 0.025f, 0.01f);

        }
    }

    public class SupremacyWyrmTail : SupremacyWyrmBody
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mini Gigapora");
            Main.projPet[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 48;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.hide = false;
        }
    }

}