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
using DestroyerTest.Content.Particles;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    public class SulfurScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = new Color(140, 234, 87);
            WidthDim = 40;
            HeightDim = 40;
            DustType = DustID.FireworksRGB;
            DustColor = new Color(140, 234, 87);
            base.SetDefaults();
        }


        public override void OnKill(int timeLeft)
        {
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

