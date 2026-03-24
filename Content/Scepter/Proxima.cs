using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using FranciumCalamityWeapons.Content.Scepter;
using System.Collections.Generic;
using FranciumCalamityWeapons.Content.Projectiles;
using DestroyerTest.Common;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Items.Placeables.Ores;
using OpusLib;

namespace FranciumCalamityWeapons.Content.Scepter
{
    public class Proxima : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            Opus.ItemChannel_LeftChannel_RightNot.Add(Type);
        }

        SoundStyle Throw = DTAssetLib.SwordSounds.Woosh;


        public override void SetDefaults()
        {
            Item.width = 148;
            Item.height = 132;
            Item.value = Item.sellPrice(gold: 2, silver: 50);
            Item.rare = ItemRarityID.Pink;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0;
            Item.autoReuse = true;

            Item.crit = 19;
            Item.shoot = ModContent.ProjectileType<GodSlayerScepterHoldout>();
            Item.shootSpeed = 12f;
            Item.noUseGraphic = true;
            Item.damage = 435 + (int)Math.Round(ScepterClassStats.DamageModifier);
            Item.DamageType = ModContent.GetInstance<ScepterClass>();
            Item.channel = true;
            Item.UseSound = null;
        }


        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.useTime = 60;
                Item.autoReuse = false;
                Item.useAnimation = 60;
                Item.shoot = ModContent.ProjectileType<ProximaThrown>();
                Item.shootSpeed = 25.0f;
                Item.noUseGraphic = true;
                Item.noMelee = false;
                Item.UseSound = Throw;
                Item.crit = 19;
                Item.damage = 700;
                Item.DamageType = ModContent.GetInstance<ScepterClass>();
                Item.channel = false;
            }
            else
            {
                Item.width = 148;
                Item.height = 132;
                Item.value = Item.sellPrice(gold: 2, silver: 50);
                Item.rare = ItemRarityID.Pink;
                Item.useTime = 40;
                Item.useAnimation = 40;
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.knockBack = 0;
                Item.autoReuse = true;
                Item.crit = 19;
                Item.shoot = ModContent.ProjectileType<ProximaHoldout>();
                Item.shootSpeed = 0.01f;
                Item.noUseGraphic = true;
                Item.damage = 700;
                Item.DamageType = ModContent.GetInstance<ScepterClass>();
                Item.channel = true;
                Item.UseSound = null;
            }


            return player.ownedProjectileCounts[Item.shoot] < 1;
        }


        public override void UseItemFrame(Player player)
        {
            if (player.altFunctionUse == 2) // Throwing mode
            {
                float animationSpeed = 8.0f; // You can modify this to change the animation speed.

                // Calculate the progress, but limit it to a max of 1.0
                float progress = ((player.itemAnimationMax - player.itemAnimation) / (float)player.itemAnimationMax);
                progress = Math.Min(progress * animationSpeed, 1.0f); // Clamps progress to a max of 1

                // Start angle at 180 degrees (upwards)
                float startAngle = MathHelper.ToRadians(180f);

                // Declare endAngle here to ensure it's accessible outside of the if blocks
                float endAngle;

                // Set the end angle based on player direction
                if (player.direction == 1)
                {
                    endAngle = MathHelper.ToRadians(270f); // Right side, end angle 270
                }
                else if (player.direction == -1)
                {
                    endAngle = MathHelper.ToRadians(90f); // Left side, end angle 90
                }
                else
                {
                    endAngle = startAngle; // Default case (shouldn't happen unless player.direction is unexpected)
                }

                float armRotation = MathHelper.Lerp(startAngle, endAngle, progress);

                if (progress == 1.0f)
                {
                    armRotation = endAngle;
                }

                player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armRotation);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[type] < 1)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            }
            return false; // Prevents vanilla from auto-firing
        }
        public override void AddRecipes()
        {

            CreateRecipe()
            .AddIngredient<RuinousSoul>(6)
            .AddIngredient<ExodiumCluster>(18)
            .AddIngredient<Lumenyl>(8)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}