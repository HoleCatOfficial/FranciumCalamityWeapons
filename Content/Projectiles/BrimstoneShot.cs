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
using CalamityMod.NPCs;
using CalamityMod.Dusts;
using CalamityMod.Buffs.DamageOverTime;

namespace FranciumCalamityWeapons.Content.Projectiles
{
	public class BrimstoneShot : ScepterShot
	{
		public override void SetStaticDefaults()
		{
			TrailType = 9;
		}

		public override void SetDefaults()
		{
			Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.friendly = true;
            Projectile.hostile = false;

			TrailColor = Color.Red;
			DustColor = default;
			BounceDust = (int)CalamityDusts.Brimstone;
			KillDust = (int)CalamityDusts.Brimstone;
			TileBounce = false;
			TileKill = true;
			Homing = true;
			Projectile.Resize(16, 16);
			TrailAmplitude = 16f;
			Debuff = ModContent.BuffType<BrimstoneFlames>();
			DebuffTime = 600;
		}
	}
}