using CalamityMod;
using CalamityMod.CustomRecipes;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.World;
using FranciumCalamityWeapons.Content.Projectiles;
using FranciumCalamityWeapons.Content.Projectiles.Arsenal;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Melee
{
	public class Breaker : ModItem
	{
        public int HitCount = 0;
        public int DecayTimer = 180;
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Item.width = 76;
            Item.height = 76;

            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.autoReuse = true;
            Item.useTurn = true;

            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.damage = 30;
            Item.knockBack = 6;
            Item.crit = 14;

            Item.value = Item.buyPrice(gold: 70);
            Item.rare = ItemRarityID.White;
            Item.shoot = ModContent.ProjectileType<BreakerSwing>();
            Item.noUseGraphic = true;
            Item.channel = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips) => InsertKnowledgeTooltip(tooltips, 1);

        public static void InsertKnowledgeTooltip(List<TooltipLine> tooltips, int tier, bool allowOldWorlds = false)
        {
            Mod m = ModContent.GetInstance<FranciumCalamityWeapons>();
            TooltipLine line = new TooltipLine(m, "SchematicKnowledge1", Language.GetTextValue("Mods.FranciumCalamityWeapons.Items.Breaker.SchematicKnowledgeTooltip1"));
            line.OverrideColor = Color.Cyan;

            bool allowedDueToOldWorld = allowOldWorlds && CalamityWorld.IsWorldAfterDraedonUpdate;
            tooltips.AddWithCondition(line, !ArsenalTierGatedRecipe.HasTierBeenLearned(tier) && !allowedDueToOldWorld);
        }
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void UpdateInventory(Player player)
        {
            if (DecayTimer > 0)
            {
                DecayTimer--;
            }

            if (DecayTimer <= 0)
            {
                HitCount = 0;
            }
        }


        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<WulfrumMetalScrap>(10)
            .AddIngredient<DubiousPlating>(4)
            .AddIngredient<MysteriousCircuitry>(4)
            .AddIngredient<EnergyCore>(2)
            .AddIngredient(ItemID.IronBar, 10)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
} 