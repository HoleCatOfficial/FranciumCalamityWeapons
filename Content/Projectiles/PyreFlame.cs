using CalamityMod.Buffs.DamageOverTime;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using FranciumCalamityWeapons.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace DestroyerTest.Content.Projectiles
{
	public class PyreFlame : ModProjectile
	{
		public override string Texture => "FranciumCalamityWeapons/Content/Particles/ParticleDrawEntity";
		public override void SetDefaults()
		{
			Projectile.width = 80; // The width of projectile hitbox
			Projectile.height = 80; // The height of projectile hitbox

			Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.light = 1f; // How much light emit around the projectile
			Projectile.timeLeft = 600; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.tileCollide = false;
			Projectile.alpha = 160;
            Projectile.penetrate = -1;
			Projectile.netImportant = true;
			Projectile.netUpdate = true;
		}

		public override void AI()
		{
            Lighting.AddLight(Projectile.Center, PyreEmber().R / 255, PyreEmber().G / 255, PyreEmber().B / 255);
			int[] types = new int[]
            {
                PRTLoader.GetParticleID<ColoredFire1>(),
                PRTLoader.GetParticleID<ColoredFire2>(),
                PRTLoader.GetParticleID<ColoredFire3>(),
                PRTLoader.GetParticleID<ColoredFire4>(),
                PRTLoader.GetParticleID<ColoredFire5>(),
                PRTLoader.GetParticleID<ColoredFire6>(),
                PRTLoader.GetParticleID<ColoredFire7>()
            };

            PRTLoader.NewParticle(types[Main.rand.Next(types.Length)], Projectile.Center, Vector2.Zero, PyreEmber(), 1);
		}
        
        public Color PyreEmber()
        {
            return LerpThree(new Color(224, 108, 29), new Color(125, 58, 58), new Color(99, 7, 36), new Color());
        }

        public Color LerpThree(Color Val1, Color Val2, Color Val3, Color result)
        {
            float sine = (float)System.Math.Sin(Main.GlobalTimeWrappedHourly * 6f); // -1 to 1
            float t = (sine + 1f) / 2f; // 0 to 1
            result = Color.Lerp(Val1, Val2, t);
            result = Color.Lerp(Val2, Val3, t);
            return result;
        }

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire3, 120);
        }
	}

	
}