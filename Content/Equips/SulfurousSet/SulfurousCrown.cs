using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System;
using InnoVault.PRT;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using Terraria.Audio;
using DestroyerTest.Content.Projectiles.player.ArmorSet;
using GlowmaskHelper.Content;
using FranciumCalamityWeapons.Common;
using CalamityMod;
using DestroyerTest.Rarity.Scepter;
using FranciumCalamityWeapons.Content.Buffs;
using DestroyerTest.Content.Equips;
using DestroyerTest;

namespace FranciumCalamityWeapons.Content.Equips.SulfurousSet
{
	[AutoloadEquip(EquipType.Head)]
	public class SulfurousCrown : ModItem
	{

		public override void SetStaticDefaults()
		{
			ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 32;
			Item.height = 22;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<CerisePinkRarity>();
			Item.defense = 6;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<SulfurousGarb>();
		}

		public int cooldown = 0;
		public SoundStyle Bonus = new SoundStyle("FranciumCalamityWeapons/Audio/SulfurBoost") { MaxInstances = 0, PitchVariance = 0.3f};
		public SoundStyle Regen = new SoundStyle("FranciumCalamityWeapons/Audio/CalamityEntropy_AbyssBlade") { MaxInstances = 0, PitchVariance = 0.3f};
		
		public override void UpdateArmorSet(Player player)
		{	
			if (cooldown > 0)
			{
				cooldown--;
			}
			
			if (cooldown == 1)
			{
				SoundEngine.PlaySound(Regen, player.Center);
			}
			if (DestroyerTestMod.ArmorSetBonusHotKey.JustPressed)
			{
            	player.AddBuff(ModContent.BuffType<SulfurEmpowerment>(), 600);
				SoundEngine.PlaySound(Bonus, player.Center);
				cooldown = 1000;
			}
		}

        public override void UpdateEquip(Player player)
        {
            player.GetArmorPenetration(ModContent.GetInstance<ScepterClass>()) += 10;
        }

        public void HeadDust(Player player)
        {
            Vector2 Center1 = player.headFrame.Center.ToVector2();
            Rectangle spawn = Utils.CenteredRectangle(Center1, new Vector2(16, 8));

            if (Main.rand.NextBool(4))
            {
                Dust.NewDust(spawn.TopLeft(), spawn.Width, spawn.Height, DustID.FireworksRGB, Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-2, -1), 0, new Color(140, 234, 87), 0.5f);
            }
        }

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawOutlines = true;
		}
	}
}