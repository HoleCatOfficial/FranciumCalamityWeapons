
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using DestroyerTest.Content.Projectiles.player.Accessory;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Common
{
    public class ScepterImbuesCalamity : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool Riptide = false;
        public bool CrushDepth = false;
       
        public bool Scepter = false;

        private void DustInEnchantVisuals(ThrownScepter t, int ID, int alpha, Color color, float scale, bool noGravity = true)
        {
            Dust dust = Dust.NewDustDirect(t.EnchantmentVisuals().TopLeft(), t.EnchantmentVisuals().Width, t.EnchantmentVisuals().Height, ID, 0, 0, alpha, color, scale);
            dust.noGravity = noGravity;
        }

        public override void AI(Projectile projectile)
        {
            if (projectile.ModProjectile is ThrownScepter thrown)
            {
                Scepter = true;
                

                if (Scepter)
                {
                    if (Riptide)
                    {
                        DustInEnchantVisuals(thrown, DustID.Water, 40, default, 1f, true);
                    }
                    if (CrushDepth)
                    {
                        DustInEnchantVisuals(thrown, DustID.Water, 40, Color.DarkCyan, 1f, true);
                    }
                   
                    

                }
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Scepter)
            {
                if (Riptide)
                {
                    target.AddBuff(ModContent.BuffType<RiptideDebuff>(), 60 * Main.rand.Next(10, 17));
                }
                if (CrushDepth)
                {
                    target.AddBuff(ModContent.BuffType<CrushDepth>(), 60 * Main.rand.Next(10, 17));
                }
            }
        }

        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            if (Scepter)
            {
                if (Riptide)
                {
                    target.AddBuff(ModContent.BuffType<RiptideDebuff>(), 60 * Main.rand.Next(10, 17));
                }
                if (CrushDepth)
                {
                    target.AddBuff(ModContent.BuffType<CrushDepth>(), 60 * Main.rand.Next(10, 17));
                }
            }
        }
    }
}