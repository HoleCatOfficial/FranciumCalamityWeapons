
using DestroyerTest.Content.Resources;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using GlowmaskHelper.Content;

namespace FranciumCalamityWeapons.Content.Equips.AbyssalNeptuneSet
{
    [AutoloadGlowmask]
	[AutoloadEquip(EquipType.Body)] 
    public class AbyssalNeptuneBodyArmor : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 20; 
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Red;
            Item.defense = 17;
        }

        public override void UpdateEquip(Player player)
        {
            Lighting.AddLight(player.Center, new Color(29, 230, 255).ToVector3() * 0.5f);
        }
	}
}