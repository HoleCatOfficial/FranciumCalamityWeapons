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
using CalamityMod.Items.Armor.Tarragon;
using CalamityMod.Items.Materials;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using OpusLib;
using DestroyerTest;
using Microsoft.Build.Evaluation;
using CalamityMod.Items.Armor.Silva;
using CalamityMod.Items.Placeables;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod;
using FranciumCalamityWeapons.Content.Buffs;
using CalamityMod.Items.Placeables.Abyss;

namespace FranciumCalamityWeapons.Content.Equips
{

	[AutoloadEquip(EquipType.Head)]
	public class SilvaCrown : ModItem
	{

		public override void SetStaticDefaults()
		{
			// If your head equipment should draw hair while drawn, use one of the following:
			// ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
			//ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
            // ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true;

		}

		public override void SetDefaults()
		{
			Item.width = 30; // Width of the item
			Item.height = 26; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 18; // The amount of defense the item will give when equipped
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<SilvaArmor>() && legs.type == ModContent.ItemType<SilvaLeggings>();
		}

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(ModContent.GetInstance<ScepterClass>()) *= 1.1f;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.FranciumCalamityWeapons.Items.SilvaCrown.SetBonus");
            ScepterClassStats.ThrowSpeedModifier += 2.25f;
            ScepterClassStats.ShotBounceModifier += 4;
            ScepterClassStats.SizeMultiplier = 1.75f;
            player.Calamity().silvaSet = true;
            player.AddBuff(ModContent.BuffType<SilvaAttendantBuff>(), 600);
            if (player.TryGetModPlayer<SilvaCrownPlayer>(out var Crown))
            {
                Crown.Active = true;
            }
        }

		public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

		public override void AddRecipes()
		{
            CreateRecipe()
				.AddIngredient<PlantyMush>(6)
                .AddIngredient<EffulgentFeather>(5)
                .AddIngredient<AscendantSpiritEssence>(2)
				.AddTile<CosmicAnvil>()
				.Register();
		}
	}

    public class SilvaCrownPlayer : ModPlayer
    {
        public bool Active = false;
        public bool TrySpawnProjectilesFromAttendant = false;
        public int Cooldown = 0;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateEquips()
        {
            if (Active)
            {
                if (Cooldown > 0)
                {
                    Cooldown--;
                }

                if (Cooldown == 1)
                {
                    
                    SoundEngine.PlaySound(new SoundStyle("DestroyerTest/Assets/Audio/Corpse/TeleportSetPosition") with { PitchVariance = 0.5f }, Player.Center);
                }

                if (Cooldown == 1199)
                {
                    TrySpawnProjectilesFromAttendant = false;
                }

                if (DestroyerTestMod.ArmorSetBonusHotKey.JustPressed && Cooldown <= 0 && !TrySpawnProjectilesFromAttendant)
                {
                    TrySpawnProjectilesFromAttendant = true;
                    Cooldown = 1200;
                }
            }
        }
    }
}