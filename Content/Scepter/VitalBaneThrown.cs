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

namespace FranciumCalamityWeapons.Content.Scepter
{
    public class VitalBaneThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = new Color(159, 202, 172);
            WidthDim = 80;
            HeightDim = 80;
            DustType = DustID.FireworksRGB;
            DustColor = ThemeColor;
            base.SetDefaults();
        }

        public bool Flag1 = false;
        public override void PostAI()
        {
            base.PostAI();
            if(returning)
            {
                if (!Flag1)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<LifeEnergyCrystal>(), 100, 2, Projectile.owner);
                    Flag1 = true;
                }
            }
        }
    }
}

