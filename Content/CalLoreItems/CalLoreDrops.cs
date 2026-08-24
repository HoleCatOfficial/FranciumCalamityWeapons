using CalamityMod;
using CalamityMod.Items.LoreItems;
using DestroyerTest.Content.Entities;
using DestroyerTest.Rarity;
using Mono.Cecil.Mdb;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.CalLoreItems
{
    public class CalLoreDrops : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == ModContent.NPCType<NightmareRoseBoss>())
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<NightmareRoseLore>(), 1, 1, 1));
            }

            if (npc.type == ModContent.NPCType<WyvernCorpseHead>())
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<NightmareRoseLore>(), 1, 1, 1));
            }
        }
    }
}
