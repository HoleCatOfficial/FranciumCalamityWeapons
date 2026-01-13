using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using OpusLib.Content.Helpers;
using CalamityMod.Tiles.Abyss;
using FranciumCalamityWeapons.Content.Equips.AbyssalNeptuneSet;


namespace FranciumCalamityWeapons
{
	public class FranciumCalamityWeapons : Mod
	{
		public override void Load()
		{
			ChestLootSystem.RegisterChestLoot(
				new ChestID(ModContent.TileType<AbyssTreasureChest>(), 0),
				ModContent.ItemType<AbyssalNeptuneMask>(),
				1,
				rarity: 0.3f
			);
			ChestLootSystem.RegisterChestLoot(
				new ChestID(ModContent.TileType<AbyssTreasureChest>(), 0),
				ModContent.ItemType<AbyssalNeptuneBodyArmor>(),
				1,
				rarity: 0.3f
			);
			ChestLootSystem.RegisterChestLoot(
				new ChestID(ModContent.TileType<AbyssTreasureChest>(), 0),
				ModContent.ItemType<AbyssalNeptuneCuisses>(),
				1,
				rarity: 0.3f
			);
		}

    }
}
