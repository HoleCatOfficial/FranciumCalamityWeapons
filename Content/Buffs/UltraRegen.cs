using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Buffs
{

	public class UltraRegen : ModBuff
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
            if (player.TryGetModPlayer<UltraRegenPlayer>(out var Regen))
            {
                Regen.Active = true;
            }
        }
	}

	public class UltraRegenPlayer : ModPlayer
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
                    Dust RegenDust = Dust.NewDustDirect(Player.position, Player.width, Player.height, DustID.FireworksRGB, Player.velocity.X * 0.5f, -2f, 0, new Color(221, 189, 62), 0.5f);
                    RegenDust.noGravity = true;
                }
            }
        }

        public override void UpdateLifeRegen()
        {
            if (Active)
            {
                Player.lifeRegen += 45;
            }
        }
	}
}