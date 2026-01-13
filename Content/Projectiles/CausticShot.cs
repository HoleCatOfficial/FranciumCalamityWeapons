using System.IO;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using OpusLib;
using System.Collections.Generic;
using CalamityMod.Buffs.StatDebuffs;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;

namespace FranciumCalamityWeapons.Content.Projectiles
{
	public class CausticShot : ScepterShot
	{
		public override void SetStaticDefaults()
		{
			TrailType = 7;
		}

		public override void SetDefaults()
		{
			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;

			TrailColor = new Color(140, 234, 87);
			DustColor = new Color(140, 234, 87);
			BounceDust = KillDust;
			KillDust = DustID.FireworksRGB;
			TileBounce = false;
			TileKill = true;
			Homing = true;
			Projectile.Resize(16, 16);
			TrailAmplitude = 20f;
			Debuff = ModContent.BuffType<Irradiated>();
			DebuffTime = 600;
		}
	}
}