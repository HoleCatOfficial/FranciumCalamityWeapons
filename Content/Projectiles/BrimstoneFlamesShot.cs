using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using DestroyerTest.Content.Projectiles.ParentClasses;
using CalamityMod.Dusts;
using CalamityMod.Buffs.DamageOverTime;
using DestroyerTest.Content.Scepter;

namespace FranciumCalamityWeapons.Content.Projectiles
{
	public class CursedShot : ElementalScepterShot
	{
        public override string Texture => DTUtils.NoTexture;
        public override void SetStaticDefaults()
        {
            TrailType = 10;
            ElementalScepter.ElementalScepterOptions.Add(Type);
        }

        public override void SetDefaults()
        {
			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;
			TrailColor = Color.Red;
			DustColor = default;
			TravelDust = (int)CalamityDusts.Brimstone;
			KillDust = (int)CalamityDusts.Brimstone;
			Projectile.Resize(16, 16);
			TrailAmplitude = 10f;

            Debuff = ModContent.BuffType<BrimstoneFlames>();
            DebuffTime = 300;
            DetectionRad = 1200;
        }
    }
}