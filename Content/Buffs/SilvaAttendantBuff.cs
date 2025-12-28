using FranciumCalamityWeapons.Content.Equips;
using FranciumCalamityWeapons.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Buffs
{
	public class SilvaAttendantBuff : ModBuff
	{
		public override void SetStaticDefaults() 
        {
			Main.debuff[Type] = false; 
			Main.buffNoSave[Type] = true; 
		}

        public bool flag1 = false;
		public override void Update(Player player, ref int buffIndex)
        {
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref flag1, ModContent.ProjectileType<SilvaAttendant>());
			if (player.TryGetModPlayer<SilvaCrownPlayer>(out var Crown))
			{
				if (!Crown.Active)
				{
					player.DelBuff(buffIndex);
				}
			}
		}
	}
}