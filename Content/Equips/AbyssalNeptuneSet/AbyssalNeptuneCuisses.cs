using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Content.Resources;
using DestroyerTest.Rarity;
using System.Numerics;
using DestroyerTest.Content.Dusts;
using DestroyerTest.Content.Resources.Cloths;
using System.Drawing;
using CalamityMod;
using GlowmaskHelper.Content;
using DestroyerTest.Rarity.Scepter;

namespace FranciumCalamityWeapons.Content.Equips.AbyssalNeptuneSet
{
    [AutoloadGlowmask]
	[AutoloadEquip(EquipType.Legs)]
	public class AbyssalNeptuneCuisses : ModItem
	{
		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<CerisePinkRarity>();
			Item.defense = 4;
		}

		public override void UpdateEquip(Player player) 
        {
            player.Calamity().ironBoots = true;
        }

	}
}