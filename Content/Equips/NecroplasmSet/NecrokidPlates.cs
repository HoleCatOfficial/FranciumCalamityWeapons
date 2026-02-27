using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Abyss;
using CalamityMod.Items.Placeables.Ores;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Common;

namespace FranciumCalamityWeapons.Content.Equips.NecroplasmSet
{
	[AutoloadEquip(EquipType.Body)]
    public class NecrokidPlates : ModItem
	{
        public override void SetStaticDefaults()
		{
            CustomItemSets.DevItem[Type] = true;
		}
		public override void SetDefaults() 
		{
			Item.width = 42;
			Item.height = 24; 
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<IncarnadineRarity>();
			Item.defense = 30;
		}

        public override void UpdateEquip(Player player) 
        {

		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<Necroplasm>(12)
                .AddIngredient<Voidstone>(6)
                .AddIngredient<ExodiumCluster>(10)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}