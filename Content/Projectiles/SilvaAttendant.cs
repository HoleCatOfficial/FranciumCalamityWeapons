using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using FranciumCalamityWeapons.Content.Buffs;
using DestroyerTest;
using FranciumCalamityWeapons.Content.Equips;
using OpusLib;
using CalamityMod.Projectiles.Healing;
using CalamityMod.Projectiles.Melee;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using Terraria.Audio;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    public class SilvaAttendant : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
        }
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 24;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.ignoreWater = true; 
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Generic;
        }

        private void AnimateProjectile() {
            // Loop through the frames, assuming each frame lasts 5 ticks
            if (++Projectile.frameCounter >= 5) {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type]) {
                    Projectile.frame = 0;
                }
            }
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
            AnimateProjectile();

            if (player.HasBuff<SilvaAttendantBuff>())
            {
                Projectile.timeLeft = 60;
            }

            if (player.TryGetModPlayer<SilvaCrownPlayer>(out var Crown))
            {
                if (Crown.Active && Crown.TrySpawnProjectilesFromAttendant)
                {
                    SoundEngine.PlaySound(SoundID.Item160 with { Pitch = 0.6f }, Projectile.Center);
                    Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), Projectile.Center, Vector2.Zero, new Color(67, 122, 102), 0.01f, 0.75f);
                    Opus.RingProjectileOutward(ModContent.ProjectileType<SilvaHeart>(), 3, Projectile.Center, 30, 100, 16, 3, RandomOffset: true);
                }
            }

            if (Main.rand.NextBool(120))
            {
                Projectile.velocity += new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
            }

            Vector2 playerPos = player.Center;
            Vector2 direction = playerPos - Projectile.Center;
            if (direction.Length() > 200f)
            {
                direction.Normalize();
                Projectile.velocity = direction * 6f;
            }

            
        }
    }
}