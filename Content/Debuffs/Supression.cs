using DestroyerTest.Common;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Particles;
using FranciumCalamityWeapons.Content.Particles;
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
	public class Supression : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.debuff[Type] = true;  // Is it a debuff?
			Main.pvpBuff[Type] = true; // Players can give other players buffs, which are listed as pvpBuff
			Main.buffNoSave[Type] = true; // Causes this buff not to persist when exiting and rejoining the world
			BuffID.Sets.LongerExpertDebuff[Type] = true; // If this buff is a debuff, setting this to true will make this buff last twice as long on players in expert mode
		}

		// Allows you to make this buff give certain effects to the given player
		public override void Update(NPC target, ref int buffIndex) {
			if (target.TryGetGlobalNPC<SupressionTarget>(out var modNPC)) {
                modNPC.SupressionDebuff = true;
            }
		}
	}

	public class SupressionTarget : GlobalNPC
    {
        public override bool InstancePerEntity => true; // Ensures each NPC has its own instance

        public bool SupressionDebuff;

        public override void ResetEffects(NPC npc) {
            SupressionDebuff = false;
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage) {
			if (SupressionDebuff)
			{
				if (Main.rand.NextBool(3))
				{
					PRTLoader.NewParticle(PRTLoader.GetParticleID<SupressionParticle>(), Main.rand.NextVector2FromRectangle(npc.getRect()) + new Vector2(0, -(npc.height / 2)), new Vector2(0f, 0.04f), Color.White, 1.0f);
				}
				npc.damage = (int)(npc.damage * 0.75f);
				npc.defense = (int)(npc.defense * 0.65f);
				npc.velocity *= 0.85f;
            }
        }
    }
}