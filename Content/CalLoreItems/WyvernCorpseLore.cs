using CalamityMod;
using CalamityMod.Items.LoreItems;
using DestroyerTest.Rarity;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.CalLoreItems
{
    public class WyvernCorpseLore : LoreItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override LocalizedText Tooltip => CalamityUtils.GetText($"{LocalizationCategory}.ShortTooltip");
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine fullLore = new(Mod, "CalamityMod:Lore", this.GetLocalizedValue("Lore"));
            if (LoreColor.HasValue)
                fullLore.OverrideColor = LoreColor.Value;
            CalamityUtils.HoldShiftTooltip(tooltips, new TooltipLine[] { fullLore }, true);
        }
        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ModContent.RarityType<CrimsonSpecialRarity>();
            Item.maxStack = 1;
        }
    }
}
