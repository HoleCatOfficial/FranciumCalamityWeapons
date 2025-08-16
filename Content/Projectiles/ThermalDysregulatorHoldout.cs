using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using Microsoft.Xna.Framework.Graphics;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Buffs;
using FranciumCalamityWeapons.Content.Ranged;
using System;
using DestroyerTest.Content.Projectiles;
using InnoVault;
using Microsoft.Xna.Framework.Input;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    public class ThermalDysregulatorHoldout : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 19; // This projectile has 4 frames.
        }
        public override void SetDefaults()
        {
            Projectile.width = 110;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1800; // persistent
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.netImportant = true;
			Projectile.netUpdate = true;
        }

        private void AnimateProjectile() {
            // Loop through the frames, assuming each frame lasts 5 ticks
            if (++Projectile.frameCounter >= 1) {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type]) {
                    Projectile.frame = 0;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            return true;
        }

        int ChargeAmount = 0;
        int MagmaLaserCount = 6;
        int HotWaterTimer = 60;
        int PyreFlameTimer = 60;
        int HotWaterSoundTimer = 5;
        int PyreFlameSoundTimer = 5;
        public bool IsCharging;
        public int ChargeTimer;
        public bool HasConsumedWater;
        public int _upHoldTicks = 0;
        public int _downHoldTicks = 0;
        public int _smartHoldTicks = 0;
        public int DownTime = 20;

        public enum FireMode { Normal, JetWater, MagmaMine, FlameMagma }
        public FireMode SelectedMode;



        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // Check if the player is holding the item and channeled
            if (player.HeldItem.type == ModContent.ItemType<ThermalDysregulator>() && player.channel)
            {
                AnimateProjectile();

                HoldAnim(player);

                // Track ticks held for each control
                if (player.controlUp)
                    _upHoldTicks++;
                else
                    _upHoldTicks = 0;

                if (player.controlDown)
                    _downHoldTicks++;
                else
                    _downHoldTicks = 0;

                if (player.controlSmart)
                    _smartHoldTicks++;
                else
                    _smartHoldTicks = 0;

                // Only change mode if key is held for 10+ ticks
                if (_upHoldTicks >= 10)
                    SelectedMode = FireMode.JetWater;
                else if (_downHoldTicks >= 10)
                    SelectedMode = FireMode.MagmaMine;
                else if (_smartHoldTicks >= 10)
                    SelectedMode = FireMode.FlameMagma;
                else
                    SelectedMode = FireMode.Normal;


                // Start or continue charging
                IsCharging = player.channel;
                if (IsCharging)
                {
                    ChargeTimer++;

                    // Maybe show a glow or sound at 60, 120, 180 ticks?
                    if (ChargeTimer == 60) // Full charge!
                    {
                        Fire(player); // Modular fire method

                        ChargeTimer = 0;
                        IsCharging = false;
                        DownTime--;
                        if (DownTime <= 0)
                        {
                            Projectile.Kill();
                        }
                    }
                }
                else
                {
                    ChargeTimer = 0;
                    IsCharging = false;
                }

            }
            else
            {
                // Kill the projectile if the item is not being held
                Projectile.Kill();
            }
        }
        
        bool ConsumeWaterBottles(Player player)
        {
            int count = 0;
            for (int i = 0; i < player.inventory.Length; i++)
            {
                if (player.inventory[i].type == ItemID.BottledWater)
                {
                    int take = Math.Min(2 - count, player.inventory[i].stack);
                    player.inventory[i].stack -= take;
                    count += take;
                    if (player.inventory[i].stack <= 0) player.inventory[i].TurnToAir();
                }
                if (count >= 2) return true;
            }
            return false;
        }

        public void HoldAnim(Player player)
        {
            // Lock the projectile's position relative to the player
            float holdDistance = 40f;
            Vector2 mountedCenter = player.MountedCenter;
            Vector2 toCursor = Main.MouseWorld - mountedCenter;
            toCursor.Normalize();
            Vector2 desiredPos = mountedCenter + toCursor * holdDistance;

            Projectile.Center = desiredPos;

            // Rotate to face the cursor
            Projectile.rotation = toCursor.ToRotation();

            // Constantly face the direction it's pointing
            Projectile.spriteDirection = toCursor.X > 0 ? 1 : -1;
            Projectile.direction = toCursor.X > 0 ? 1 : -1;

            if (Projectile.spriteDirection == -1)
                Projectile.rotation += MathHelper.Pi;
        }

        public void Fire(Player player)
        {
            Vector2 direction = Vector2.Normalize(Main.MouseWorld - Projectile.Center);
            int damage = player.GetWeaponDamage(player.HeldItem);
            float knockback = player.GetWeaponKnockback(player.HeldItem);

                    switch (SelectedMode)
                    {
                        case FireMode.Normal:
                            SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/MagmaLaserShoot"), Projectile.Center);
                            for (int i = 0; i < 6; i++)
                            {
                                Vector2 perturbedDirection = direction.RotatedByRandom(MathHelper.ToRadians(10));
                                Projectile.NewProjectile(
                                    Projectile.GetSource_FromThis(),
                                    Projectile.Center + new Vector2((Projectile.width / 2) * Projectile.spriteDirection, 0).RotatedBy(Projectile.rotation),
                                    perturbedDirection * 14f,
                                    ModContent.ProjectileType<MagmaLaser>(), // You'll need to create this
                                    damage / 4,
                                    knockback,
                                    player.whoAmI
                                );
                            }
                            break;

                        case FireMode.JetWater:
                            if (!HasConsumedWater)
                            {
                                if (!ConsumeWaterBottles(player))
                                    return;
                                HasConsumedWater = true;
                            }
                            if (HotWaterTimer > 0)
                            {
                                Projectile.frame = 1;

                                // Play sound every 5 ticks
                                if (HotWaterSoundTimer == 5)
                                {
                                    SoundEngine.PlaySound(SoundID.Item13, Projectile.Center);
                                }
                                HotWaterSoundTimer--;
                                if (HotWaterSoundTimer <= 0)
                                {
                                    HotWaterSoundTimer = 5;
                                }
                                
                                for (int i = 0; i < 36; i++)
                                {
                                    Vector2 perturbedDirection = direction.RotatedByRandom(MathHelper.ToRadians(10));
                                    // Spawn projectile every tick while timer is active
                                    Projectile.NewProjectile(
                                        Projectile.GetSource_FromThis(),
                                        Projectile.Center + new Vector2((Projectile.width / 2) * Projectile.spriteDirection, 0).RotatedBy(Projectile.rotation),
                                        perturbedDirection * 14f,
                                        ModContent.ProjectileType<HotWater>(),
                                        damage / 8,
                                        knockback,
                                        player.whoAmI
                                    );
                                }
                                HotWaterTimer--;
                            }
                            break;
                        case FireMode.MagmaMine:
                            SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/PyreMineShoot"), Projectile.Center);
                                        Vector2 fireDirection = Projectile.rotation.ToRotationVector2();

                                        // Use the adjusted fire direction
                                            Projectile.NewProjectile(
                                            Projectile.GetSource_FromThis(),
                                            Projectile.Center + new Vector2((Projectile.width / 2) * Projectile.spriteDirection, 0).RotatedBy(Projectile.rotation),
                                            direction * 4f,
                                            ModContent.ProjectileType<PyreMine>(),
                                            damage,
                                            knockback,
                                            player.whoAmI
                                        );

                            break;
                        case FireMode.FlameMagma:
                            if (PyreFlameTimer > 0)
                            {
                                Projectile.frame = 1;

                                if (PyreFlameSoundTimer == 5)  // you can rename this var to differentiate
                                {
                                    SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/PyreFlameShoot"), Projectile.Center);
                                }
                                PyreFlameSoundTimer--;
                                if (PyreFlameSoundTimer <= 0)
                                {
                                    PyreFlameSoundTimer = 5;
                                }
                                for (int i = 0; i < 36; i++)
                                {
                                    Vector2 perturbedDirection = direction.RotatedByRandom(MathHelper.ToRadians(10));
                                    Projectile.NewProjectile(
                                        Projectile.GetSource_FromThis(),
                                        Projectile.Center + new Vector2((Projectile.width / 2) * Projectile.spriteDirection, 0).RotatedBy(Projectile.rotation),
                                        perturbedDirection * 14f,
                                        ModContent.ProjectileType<PyreFlame>(),
                                        damage / 8,
                                        knockback,
                                        player.whoAmI
                                    );
                                }

                                PyreFlameTimer--;  // Assuming you want the timer to count down each tick
                            }
                            break;
            }

            // Add effects here if you want (dust, sound)
            
        }


		
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.LocalPlayer;

        }

    }
}