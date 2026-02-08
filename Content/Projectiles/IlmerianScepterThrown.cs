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

namespace FranciumCalamityWeapons.Content.Projectiles
{
    public class IlmerianScepterThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = Color.SkyBlue;
            WidthDim = 34;
            HeightDim = 34;
            DustType = DustID.Electric;
            base.SetDefaults();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 300);
        }

    }
}

