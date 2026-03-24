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
    public class ProximaThrown : ThrownScepter
    {
        public override void SetDefaults()
        {
            ThemeColor = new Color(123, 228, 234);
            WidthDim = 40;
            HeightDim = 40;
            DustType = DustID.FireworksRGB;
            DustColor = new Color(123, 228, 234);
            base.SetDefaults();
        }


        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            hitbox.Inflate(30, 30);
        }
    }
}

