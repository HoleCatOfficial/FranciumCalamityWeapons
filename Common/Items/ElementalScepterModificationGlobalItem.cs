using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.Melee.Spears;
using DestroyerTest.Content.Scepter;
using FranciumCalamityWeapons.Content.Scepter;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Common.Items
{
    public class ElementalScepterModificationGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ModContent.ItemType<ElementalScepter>())
            {
                ElementalScepter.RegisterScepterLine(Mod, "[i:FranciumCalamityWeapons/BrimstoneScepter] A Brimstone Flames shot,", tooltips);
            }
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
        }
    }
}