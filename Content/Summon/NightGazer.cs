
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using DestroyerTest.Rarity;
using DestroyerTest.Common;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.MeleeWeapons;

using Terraria.Localization;
using DestroyerTest.Content.Entities;
using System.Linq;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Projectiles.Weapon.Summon;
using FranciumCalamityWeapons.Content.Buffs;
using FranciumCalamityWeapons.Content.Projectiles.NightGazer;
using CalamityMod.Items.Placeables.Abyss;
using CalamityMod.Items.Materials;

namespace FranciumCalamityWeapons.Content.Summon
{
    public class NightGazer : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;

            ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f;
        }

        public override void SetDefaults()
        {
            Item.damage = 230;
            Item.knockBack = 9f;
            Item.mana = 30;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = 18000;
            Item.rare = ModContent.RarityType<VesperRarity>();
            Item.UseSound = new SoundStyle("FranciumCalamityWeapons/Audio/NightGazerSummon");

            Item.noMelee = true;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<NightGazerBuff>();
            Item.shoot = ModContent.ProjectileType<NightGazerProjectile>();


        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            position = Main.MouseWorld;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.AddBuff(Item.buffType, 2);
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Voidstone>(10)
                .AddIngredient<RuinousSoul>(6)
                .AddIngredient(ItemID.LunarBar, 10)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
