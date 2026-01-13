using CalamityMod.Buffs.StatDebuffs;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Equips.SulfurousSet
{
    [AutoloadEquip(EquipType.Body)]
    public class SulfurousGarb : ModItem
    {
        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
            {
                return;
            }

            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}", EquipType.Legs, this);
        }

        public override void SetStaticDefaults()
        {
            ArmorIDs.Body.Sets.HidesHands[Item.bodySlot] = false;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 30;
            Item.rare = ModContent.RarityType<CerisePinkRarity>();
            Item.defense = 18;
        }

        public override void SetMatch(bool male, ref int equipSlot, ref bool robes)
        {
            robes = true;
            equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
        }

        public override void UpdateEquip(Player player)
        {
            
        }
	}

    public class SulfurousGarbThorns : ModPlayer
    {
        public bool Active = false;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (Active)
            {
                npc.SimpleStrikeNPC(20, Player.direction, false, 4, null, false, 0, true);
                npc.AddBuff(ModContent.BuffType<Irradiated>(), 600);
            }
        }
    }
}