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
	// This class serves as an example of a debuff that causes constant loss of life
	// See ExampleLifeRegenDebuffPlayer.UpdateBadLifeRegen at the end of the file for more information
	public class SandBlindness : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.debuff[Type] = true;  // Is it a debuff?
			Main.pvpBuff[Type] = true; // Players can give other players buffs, which are listed as pvpBuff
			Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world
			BuffID.Sets.LongerExpertDebuff[Type] = true; // If this buff is a debuff, setting this to true will make this buff last twice as long on players in expert mode
		}

		// Allows you to make this buff give certain effects to the given player
		public override void Update(NPC target, ref int buffIndex) {
			if (target.TryGetGlobalNPC<SandBlindnessNPC>(out var modNPC)) {
                modNPC.SandBlindness = true;
            }
		}
	}

	public class SandBlindnessNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true; // Ensures each NPC has its own instance

        public bool SandBlindness;

        public override void ResetEffects(NPC npc) {
            SandBlindness = false;
        }

        public override void AI(NPC npc)
        {
            Player player = Main.player[npc.target];
            if (SandBlindness)
            {
                player.aggro -= 1000;
            }
            base.AI(npc);
        }
    }
}