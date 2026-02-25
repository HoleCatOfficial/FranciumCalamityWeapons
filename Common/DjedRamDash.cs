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
using DestroyerTest.Content.Dusts;
using Terraria.GameContent.ItemDropRules;
using System.Linq;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Content.Equips.ScepterAccessories;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Enums;
using DestroyerTest.Common;

namespace FranciumCalamityWeapons.Common
{
    /*
    public class DjedRamDash : PlayerDashEffect
    {
        public static new string ID => "Djed Pillar Charm";
        public override DashCollisionType CollisionType => DashCollisionType.ShieldSlam;
        public override bool IsOmnidirectional => false;

        public override float CalculateDashSpeed(Player player) => 16.9f;

        public override void OnDashEffects(Player player)
        {
            // Nothing
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustDirect(player.position, player.width, player.height, ModContent.DustType<ColorableNeonDust>(), player.velocity.X * 0.5f, 0f, 0, ColorLib.SpiritFireGradient(0.01f), 1.5f);
            }
        }
    }
    */
}