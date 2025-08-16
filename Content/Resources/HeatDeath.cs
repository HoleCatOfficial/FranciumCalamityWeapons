using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using System.Security.Cryptography.X509Certificates;
using System.Collections.Generic;
using Terraria.Social.Steam;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;

namespace FranciumCalamityWeapons.Content.Resources;

    
	public class HeatDeath : ModItem
	{
        public override string Texture => "FranciumCalamityWeapons/Content/Resources/HeatDeath";
		

		public override void SetDefaults() {
			Item.width = 46; // The item texture's width
			Item.height = 46; // The item texture's height

			Item.maxStack = Item.CommonMaxStack; // The item's max stack value
			Item.value = Item.buyPrice(silver: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
        }

        public override void SetStaticDefaults() {
			// The text shown below some item names is called a tooltip. Tooltips are defined in the localization files. See en-US.hjson.

			// How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.wiki.gg/wiki/Journey_Mode#Research for a list of commonly used research amounts depending on item type. This defaults to 1, which is what most items will use, so you can omit this for most ModItems.
			Item.ResearchUnlockCount = 5;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(8, 4));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            

			// This item is a custom currency (registered in ExampleMod), so you might want to make it give "coin luck" to the player when thrown into shimmer. See https://terraria.wiki.gg/wiki/Luck#Coins
			// However, since this item is also used in other shimmer related examples, it's commented out to avoid the item disappearing
			//ItemID.Sets.CoinLuckValue[Type] = Item.value;
		}

    public override void AddRecipes()
    {
        CreateRecipe(4)
		.AddIngredient<ShadowspecBar>(1)
		.AddIngredient<FalseVacuum>(1)
		.AddIngredient<MiracleMatter>(1)
		.AddTile<DraedonsForge>()
		.Register();
    }
}

