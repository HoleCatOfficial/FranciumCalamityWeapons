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
using DestroyerTest.Content.MeleeWeapons;
using FranciumCalamityWeapons.Content.Melee;
using CalamityMod.Events;
using DestroyerTest.Content.Entities;
using DestroyerTest.Content.Projectiles.Boss.TenebrousConstruct;


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

			BossRushEvent.Bosses.Insert(11, new BossRushEvent.Boss(ModContent.NPCType<ConstitutionBoss>(), BossRushEvent.TimeChangeContext.None));
            BossRushEvent.Bosses.Insert(33, new BossRushEvent.Boss(ModContent.NPCType<NightmareRoseBoss>(), BossRushEvent.TimeChangeContext.None));
            BossRushEvent.Bosses.Insert(33, new BossRushEvent.Boss(ModContent.NPCType<WyvernCorpseHead>(), BossRushEvent.TimeChangeContext.None, 
				permittedNPCs: 
				[
					ModContent.NPCType<WyvernCorpseBody1>(),
                    ModContent.NPCType<WyvernCorpseBody2>(),
                    ModContent.NPCType<WyvernCorpseBody3>(),
                    ModContent.NPCType<WyvernCorpseLegs>(),
                    ModContent.NPCType<WyvernCorpseTail>(),
                    ModContent.NPCType<SoulOrb>(),
                ]
				));
            BossRushEvent.Bosses.Insert(44, new BossRushEvent.Boss(ModContent.NPCType<TenebrousConstruct>(), BossRushEvent.TimeChangeContext.None, permittedNPCs: [ModContent.NPCType<KillableChargeSpirit>()]));
        }

        public override object Call(params object[] args)
        {
			if (args.Length > 0)
			{
				if ((string)args[0] == "BossRushActive")
				{
					return BossRushEvent.BossRushActive;
				}
			}
            return base.Call(args);
        }
    }

	public class Calls : ModSystem
	{
        public override void PostSetupContent()
        {
            ModContent.GetInstance<CalamityMod.CalamityMod>().Call("MakeItemExhumable", ModContent.ItemType<Colossus>(), ModContent.ItemType<Deuxième>());
        }
	}
}
