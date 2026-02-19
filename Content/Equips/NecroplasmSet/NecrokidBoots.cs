using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Abyss;
using CalamityMod.Items.Placeables.Ores;
using DestroyerTest.Common;

namespace FranciumCalamityWeapons.Content.Equips.NecroplasmSet
{
	[AutoloadEquip(EquipType.Legs)]
	public class NecrokidBoots : ModItem
	{

        public override void SetStaticDefaults()
		{
            CustomItemSets.DevItem[Type] = true;
		}
		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<IncarnadineRarity>();
			Item.defense = 20;
		}

		public override void UpdateEquip(Player player) 
        {
			player.GetCritChance(DamageClass.Generic) += 5f;
		}
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<Necroplasm>(4)
                .AddIngredient<ExodiumCluster>(10)
                .AddIngredient<Voidstone>(6)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}
}