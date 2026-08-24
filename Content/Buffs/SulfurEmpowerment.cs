using CalamityMod.Buffs.StatDebuffs;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using OpusLib;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Buffs
{

	public class SulfurEmpowerment : ModBuff
	{
		public override void SetStaticDefaults() 
        {
			Main.debuff[Type] = false; 
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true; 
			BuffID.Sets.LongerExpertDebuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
        {
            if (player.TryGetModPlayer<SEPlayer>(out var Sulfur))
            {
                Sulfur.Active = true;
            }
        }
	}

	public class SEPlayer : ModPlayer
	{
		public bool Active = false;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateBuffs()
        {
            if (Active)
            {
                if (Player.miscCounter % 20 == 0)
                {
                    Opus.RadialSpreadDust(DustID.TintableDustLighted, 16, Player.Center, 0, new Color(140, 234, 87), 1f, 3, Main.rand.NextFloat(MathHelper.TwoPi));
                }

                Player.ScepterClass().Range += 160;
                Player.ScepterClass().ThrowSpeedModifier += 3f;
                Player.GetDamage(ModContent.GetInstance<ScepterClass>()) *= 1.16f;
            }
        }
	}

    public class SEOwnedProjectile : GlobalProjectile
    {
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[projectile.owner];
            DTConfig cfg = ModContent.GetInstance<DTConfig>();
            if (player.TryGetModPlayer<SEPlayer>(out var Sulfur) && projectile.owner == player.whoAmI && projectile.DamageType == ModContent.GetInstance<ScepterClass>())
            {
                if (Sulfur.Active)
                {
                    Opus.RadialSpreadDust(DustID.FireworksRGB, 10, target.Center, 0, new Color(140, 234, 87), 1f, 2, Main.rand.NextFloat(MathHelper.TwoPi));
                    target.AddBuff(ModContent.BuffType<Irradiated>(), 300);
                }
            }
        }
    }
}