using CalamityMod;
using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using FranciumCalamityWeapons.Content.Particles;
using InnoVault;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Debuffs
{
	public class ArsenalArrowShock : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = false;
			Main.buffNoSave[Type] = true; 
			BuffID.Sets.LongerExpertDebuff[Type] = true;
		}
		public override void Update(NPC target, ref int buffIndex) {
			if (target.TryGetGlobalNPC<ArsenalArrowShockNPC>(out var modNPC)) {
                modNPC.Shock = true;
            }
		}
	}

	public class ArsenalArrowShockNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true; // Ensures each NPC has its own instance

        public bool Shock;

        public override void ResetEffects(NPC npc) {
            Shock = false;
        }

        public Color PhaseSlayerOrange = new Color(255, 64, 31);
        public override void AI(NPC npc)
        {
            if (Shock)
            {
                for(int i = 0; i < 5; i++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.FireworksRGB, Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-3, -4), 0, PhaseSlayerOrange, 0.5f);
                }
                if (!npc.boss)
                {
                    if (npc.velocity.Length() > 40)
                    {
                        npc.velocity *= 0.9f;
                    }
                }
                else
                {
                    if (npc.velocity.Length() > 60)
                    {
                        npc.velocity *= 0.9f;
                    }
                }
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (Shock)
            {
                if (npc.lifeRegen > 0) 
                {
                    npc.lifeRegen = 0;
                }

                npc.lifeRegen -= 70;
            }
        }
    }
}