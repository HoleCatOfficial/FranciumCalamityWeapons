using Terraria;
using Terraria.ID;
using System;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;
using FranciumCalamityWeapons.Common.Rarities;
using DestroyerTest.Content.RangedItems;
using CalamityMod;
using DestroyerTest.Content.Projectiles;
using FranciumCalamityWeapons.Content.Projectiles;
using Terraria.DataStructures;
using DestroyerTest.Content.Entity;
using DestroyerTest.Content.RogueItems;
using FranciumCalamityWeapons.Content.Projectiles.StealthStrike;
using DestroyerTest.Content.RiftArsenal;
using Terraria.Audio;

namespace FranciumCalamityWeapons.Common.Items
{
    public class RogueCompatibility : GlobalItem
    {
        public static bool CalamityLoaded => ModLoader.HasMod("CalamityMod");
        public static bool DestroyerTestLoaded => ModLoader.HasMod("DestroyerTest");

        public Vector2 PlayerToMouse = Main.MouseWorld - Main.LocalPlayer.Center;
        public Vector2 MouseToPlayer = Main.LocalPlayer.Center - Main.MouseWorld;

        public override bool InstancePerEntity => true;
        public override void SetDefaults(Item item)
        {
            if (CalamityLoaded)
            {
                if (item.type == ModContent.ItemType<TenebrousChakram>())
                {
                    item.DamageType = ModContent.GetInstance<RogueDamageClass>();
                }

                if (item.type == ModContent.ItemType<GigaCursedHammerWeapon>())
                {
                    item.DamageType = ModContent.GetInstance<RogueDamageClass>();
                }

                if (item.type == ModContent.ItemType<RiftMaker>())
                {
                    item.DamageType = ModContent.GetInstance<RogueDamageClass>();
                }

                if (item.type == ModContent.ItemType<RiftSpine>())
                {
                    item.DamageType = ModContent.GetInstance<RogueDamageClass>();
                }

                if (item.type == ModContent.ItemType<RiftTeardrop>())
                {
                    item.DamageType = ModContent.GetInstance<RogueDamageClass>();
                }

                if (item.type == ModContent.ItemType<Chroma>())
                {
                    item.DamageType = ModContent.GetInstance<RogueDamageClass>();
                }

                if (item.type == ModContent.ItemType<RiftChakram>())
                {
                    item.DamageType = ModContent.GetInstance<RogueDamageClass>();
                }
                
                if (item.type == ModContent.ItemType<P_Noctis>())
                {
                    item.DamageType = ModContent.GetInstance<RogueDamageClass>();
                }
            }
        }

        public override bool? UseItem(Item item, Player player)
        {
            if (CalamityLoaded && player.Calamity().StealthStrikeAvailable())
            {
                player.GetModPlayer<RogueCompatibilityStealthPlayer>().usedStealthStrike = true;
            }

            if (CalamityLoaded && player.Calamity().StealthStrikeAvailable() && item.type == ModContent.ItemType<RiftMaker>())
            {
                player.GetModPlayer<RogueCompatibilityStealthPlayer>().usedStealthStrike = true;
                for (int c = 0; c < 12; c++)
                {
                    float angle = Main.rand.NextFloat(0, MathHelper.TwoPi);
                    float speed = 8f; // Adjust speed as needed
                    Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed;
                    Projectile.NewProjectile(
                            player.GetSource_ItemUse(item),
                            player.Center,
                            velocity,
                            ModContent.ProjectileType<RiftStar>(),
                            (int)(item.damage * 0.5f),
                            1,
                            player.whoAmI
                        );
                }
            }

            if (CalamityLoaded && player.Calamity().StealthStrikeAvailable() && item.type == ModContent.ItemType<Chroma>())
            {
                Mod.Logger.Info("Stealth Strike triggered for Chroma!");

                player.GetModPlayer<RogueCompatibilityStealthPlayer>().usedStealthStrike = true;

                Vector2 sourceAbove = player.Center + new Vector2(0, -40);
                Vector2 sourceBelow = player.Center + new Vector2(0, 40);
                Vector2 directiondown = Main.MouseWorld - sourceAbove;
                Vector2 directionup = Main.MouseWorld - sourceBelow;
                directiondown.Normalize();
                directionup.Normalize();

                Projectile.NewProjectile(
                    player.GetSource_ItemUse(item),
                    sourceAbove,
                    directiondown * 20f,
                    ModContent.ProjectileType<ChromaStardust>(),
                    (int)(item.damage * 0.75f),
                    1f,
                    player.whoAmI
                );

                Projectile.NewProjectile(
                    player.GetSource_ItemUse(item),
                    sourceBelow,
                    directionup * 20f,
                    ModContent.ProjectileType<ChromaVortex>(),
                    (int)(item.damage * 0.75f),
                    1f,
                    player.whoAmI
                );
            }

            if (CalamityLoaded && player.Calamity().StealthStrikeAvailable() && item.type == ModContent.ItemType<RiftChakram>())
            {
                Projectile.NewProjectile(
                            player.GetSource_ItemUse(item),
                            player.Center,
                            new Vector2(0.0f, 8),
                            ModContent.ProjectileType<RiftChakramSawClone>(),
                            (int)(item.damage * 0.25f),
                            1,
                            player.whoAmI
                        );

                
            }
                return base.UseItem(item, player);
            }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (CalamityLoaded && player.Calamity().StealthStrikeAvailable() && item.type == ModContent.ItemType<P_Noctis>())
            {
				Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PNoctisAoE>(), damage, 0);
			}
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }


        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return true; // Applies to all items, but SetDefaults will filter
        }
    }
    
    public class RogueCompatibilityStealthPlayer : ModPlayer
    {
        public bool usedStealthStrike = false;

        public override void ResetEffects()
        {
            usedStealthStrike = false;
        }
    }


    public class RogueCompatibilityProjectile : GlobalProjectile
    {
        public bool IsStealth;
        public bool IsHoming = false;
        public int HomingDetectionRange = 0;
        public bool WillRotateToHomingTarget = false;
        public bool WillRotateToHomingTargetWith90DegreeRot = false;
        public bool WillUseTimerForHoming = false;
        public int TimerVal = 10;
        private int TCScytheTimer;
        public Vector2 PlayerToMouse = Main.MouseWorld - Main.LocalPlayer.Center;
        public Vector2 MouseToPlayer = Main.LocalPlayer.Center - Main.MouseWorld;

        public override bool InstancePerEntity => true;

        private NPC GetHomingTarget(Projectile projectile)
        {
            return projectile.ai[0] == 0 ? null : Main.npc[(int)projectile.ai[0] - 1];
        }

        private void SetHomingTarget(Projectile projectile, NPC value)
        {
            projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
        }

        public ref float GetDelayTimer(Projectile projectile) => ref projectile.ai[1];

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (!RogueCompatibility.CalamityLoaded || !RogueCompatibility.DestroyerTestLoaded)
                return;

            if (projectile.type == ModContent.ProjectileType<TenebrousChakramThrown>())
            {
                projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();

                Player owner = Main.player[projectile.owner];
                if (owner.GetModPlayer<RogueCompatibilityStealthPlayer>().usedStealthStrike)
                {
                    IsStealth = true;
                }
            }

            if (projectile.type == ModContent.ProjectileType<GigaCursedHammerThrown>())
            {
                projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();

                Player owner = Main.player[projectile.owner];
                if (owner.GetModPlayer<RogueCompatibilityStealthPlayer>().usedStealthStrike)
                {
                    IsStealth = true;
                }
            }

            if (projectile.type == ModContent.ProjectileType<RiftMaker_Thrown>())
            {
                projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
                Player owner = Main.player[projectile.owner];
                if (owner.GetModPlayer<RogueCompatibilityStealthPlayer>().usedStealthStrike)
                {
                    IsStealth = true;
                }
            }

            if (projectile.type == ModContent.ProjectileType<RiftTeardrop_Thrown>())
            {
                projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
                Player owner = Main.player[projectile.owner];
                if (owner.GetModPlayer<RogueCompatibilityStealthPlayer>().usedStealthStrike)
                {
                    IsStealth = true;
                }
            }

            if (projectile.type == ModContent.ProjectileType<RiftSpine_Thrown>())
            {
                projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
                Player owner = Main.player[projectile.owner];
                if (owner.GetModPlayer<RogueCompatibilityStealthPlayer>().usedStealthStrike)
                {
                    IsStealth = true;
                }
            }

            if (projectile.type == ModContent.ProjectileType<RiftChakramThrown>())
            {
                projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
                Player owner = Main.player[projectile.owner];
                if (owner.GetModPlayer<RogueCompatibilityStealthPlayer>().usedStealthStrike)
                {
                    IsStealth = true;
                }
            }
        }

        public override void AI(Projectile projectile)
        {

            if (projectile.type == ModContent.ProjectileType<TenebrousChakramThrown>() && IsStealth)
            {
                TCScytheTimer++;
                if (TCScytheTimer >= 30)
                {
                    Projectile.NewProjectile(
                        projectile.GetSource_FromThis(),
                        projectile.Center,
                        Vector2.Zero,
                        ModContent.ProjectileType<TenebrousChakramIdleScythe>(),
                        (int)(projectile.damage * 0.75f),
                        1,
                        projectile.owner
                    );
                    TCScytheTimer = 0;
                }
            }

            if (projectile.type == ModContent.ProjectileType<GigaCursedHammerThrown>() && IsStealth)
            {
                IsHoming = true;
                HomingDetectionRange = 1600;
                
                projectile.penetrate = 1;
            }

            if ((projectile.type == ModContent.ProjectileType<RiftTeardrop_Thrown>() || projectile.type == ModContent.ProjectileType<RiftSpine_Thrown>()) && IsStealth)
            {
                IsHoming = true;
                WillUseTimerForHoming = true;
                TimerVal = 20;
                HomingDetectionRange = 4600;
                WillRotateToHomingTarget = true;
                projectile.penetrate = 1;
            }
            



            if (IsHoming)
            {
                ManageHoming(projectile);
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            
        }

        public void ManageHoming(Projectile projectile)
        {
            if (WillUseTimerForHoming)
            {
                ref float DelayTimer = ref GetDelayTimer(projectile);
                if (DelayTimer < TimerVal)
                {
                    DelayTimer += 1;
                    return;
                }
            }

            float maxDetectRadius = HomingDetectionRange; // The maximum radius at which a projectile can detect a target

            // First, we find a homing target if we don't have one
            NPC HomingTarget = GetHomingTarget(projectile);
            if (HomingTarget == null)
            {
                HomingTarget = FindClosestNPC(maxDetectRadius, projectile);
                SetHomingTarget(projectile, HomingTarget);
            }

            // If we have a homing target, make sure it is still valid. If the NPC dies or moves away, we'll want to find a new target
            if (HomingTarget != null && !IsValidTarget(HomingTarget))
            {
                HomingTarget = null;
                SetHomingTarget(projectile, null);
            }

            // If we don't have a target, don't adjust trajectory
            if (HomingTarget == null)
                return;

            // If found, we rotate the projectile velocity in the direction of the target.
            // We only rotate by 3 degrees an update to give it a smooth trajectory. Increase the rotation speed here to make tighter turns
            float length = projectile.velocity.Length();
            float targetAngle = projectile.AngleTo(HomingTarget.Center);
            projectile.velocity = projectile.velocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(300)).ToRotationVector2() * length;
            if (WillRotateToHomingTargetWith90DegreeRot)
            {
                projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;

            }
            if (WillRotateToHomingTarget)
            {
                projectile.rotation = projectile.velocity.ToRotation();
            }
        }
        
        // Finding the closest NPC to attack within maxDetectDistance range
        // If not found then returns null
        public NPC FindClosestNPC(float maxDetectDistance, Projectile projectile)
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
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, projectile.Center);

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

}
