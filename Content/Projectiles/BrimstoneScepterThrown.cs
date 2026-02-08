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
using CalamityMod.Dusts;
using CalamityMod.Projectiles.Typeless;
using GlowmaskHelper.Content;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    [AutoloadGlowmask]
    public class BrimstoneScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.Red;
            WidthDim = 34;
            HeightDim = 34;
            DustType = (int)CalamityDusts.Brimstone;
            base.SetDefaults();
        }

        public override void AI()
        {
            base.AI();

            if (Main.rand.NextBool(4))
            {
                for (int t = 0; t < 2; t++)
                {
                    Dust dust = Dust.NewDustDirect(EnchantmentVisuals().TopLeft(), EnchantmentVisuals().Width, EnchantmentVisuals().Height, DustType, 0f, 0f, 0, default, 0.5f);
                    dust.velocity = Vector2.Zero;
                }
            }
            if (Main.GameUpdateCount % 6 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BrimstoneScepterBomb>(), Projectile.damage / 2, 4, Projectile.owner);
            }
        }

    }
}

