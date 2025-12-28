using Terraria;
using Terraria.ID;
using System;
using Terraria.ModLoader;
using DestroyerTest.Content.Resources.Cloths;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Rarity;
using System.Collections.Generic;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.RiftArsenal;
using Terraria.GameContent.ItemDropRules;
using System.Linq;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Content.Equips.ScepterAccessories;

namespace FranciumCalamityWeapons.Common
{
    public static class CrossMod
    {
        public static string DTCrossModName = "FranciumMultiCrossMod";
        public static bool DTCrossModLoaded;
        public static Mod DTCrossMod;

        public static void LoadMods()
        {
            DTCrossModLoaded = ModLoader.TryGetMod(DTCrossModName, out Mod CrossMod);
            DTCrossMod = CrossMod;
        }

        public static void UnloadMods()
        {
            DTCrossModLoaded = false;
            DTCrossMod = null;
        }
    }
}
