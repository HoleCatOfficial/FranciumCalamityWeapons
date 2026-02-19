using DestroyerTest.Common;
using FranciumCalamityWeapons.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Buffs
{

	public class NecroBoost : ModBuff
	{
		public override void SetStaticDefaults() 
        {
			Main.debuff[Type] = false; 
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true; 
			BuffID.Sets.LongerExpertDebuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
        {
            if (player.TryGetModPlayer<NecroBoostPlayer>(out var Boost))
            {
                Boost.Active = true;
            }
        }
	}

	public class NecroBoostPlayer : ModPlayer
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
                if (Main.rand.NextBool(6))
                {
                    Dust RegenDust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.FireworksRGB, Player.velocity.X * 0.5f, -2f, 0, new Color(254, 80, 128), 0.5f);
                    RegenDust.noGravity = true;
                }

                if (Player.miscCounter % 20 == 0)
                {
                    Projectile.NewProjectile(Player.GetSource_None(), Main.rand.NextVector2FromRectangle(Player.Hitbox), new Vector2(0, 0.01f), ModContent.ProjectileType<NecroTrail>(), 90, 16);
                }

                Player.GetAttackSpeed<ScepterClass>() += 0.6f;
            }
        }


	}
}