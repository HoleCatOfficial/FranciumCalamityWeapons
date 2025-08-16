using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using InnoVault.PRT;
using FranciumCalamityWeapons.Content.Particles;
using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Build.ObjectModelRemoting;
using FranciumCalamityWeapons.Content.Projectiles;

namespace FranciumCalamityWeapons.Content.Scepter
{
    public class SulfurScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = new Color(140, 234, 87);
            WidthDim = 40;
            HeightDim = 40;
            DustType = DustID.GreenBlood;
            base.SetDefaults();
        }

        public override void AI()
        {
            if (Main.rand.NextBool(16))
            {
                float RotOffset = Main.rand.NextFloat(-0.5f, 0.51f);
                // Base direction is the normalized velocity of the projectile
                Vector2 baseDirection = Projectile.velocity.SafeNormalize(Vector2.UnitY);
                Vector2 velocity = baseDirection.RotatedBy(RotOffset) * Main.rand.NextFloat(0.5f, 1.5f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity * 0.75f, ModContent.ProjectileType<SandBlindnessProjectile>(), 0, 0f, Projectile.owner);
                
            }
            base.AI();
        }


        public override void OnKill(int timeLeft)
        {
            for (int t = 0; t < 14; t++)
            {
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticle2>(), Projectile.Center, new Vector2(Main.rand.NextFloat(1, -1), -4), new Color(140, 234, 87), Main.rand.NextFloat(0.04f, 1.4f));
            }
            base.OnKill(timeLeft);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 6; i++)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(0, -20), new Vector2(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-9, -15)), ModContent.ProjectileType<CausticBlob>(), Projectile.damage / 4, Projectile.knockBack / 2, Projectile.owner);
            }
            target.AddBuff(ModContent.BuffType<Irradiated>(), 240);
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Inflate(40, 40);
        }
    }
}

