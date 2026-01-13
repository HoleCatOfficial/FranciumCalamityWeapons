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
using DestroyerTest.Content.Scepter;
using CalamityMod.Projectiles.Enemy;
using DestroyerTest.Rarity.Scepter;

namespace FranciumCalamityWeapons.Content.Scepter
{

	public class VictideScepter : ScepterItem
	{
        public override int Width => 54;
        public override int Height => 54;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            ShootDMG = 17;
            ShootCrit = 36;
            ThrowCrit = 22;
            KB = 5;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<PearlRarity>();

            ShootID = ModContent.ProjectileType<TealCoral>();
            ThrowID = ModContent.ProjectileType<VictideScepterThrown>();
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
			int[] types = new int[3]
			{
				ModContent.ProjectileType<TealCoral>(),
				ModContent.ProjectileType<PinkCoral>(),
				ModContent.ProjectileType<PearlChunk>()
			};

			if (player.altFunctionUse != 2)
			{
				type = types[Main.rand.Next(types.Length)];
			}
		}

		public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient<SeaRemains>(4)
				.AddIngredient(ItemID.GoldBar, 4)
				.AddTile<EutrophicShelf>()
				.Register();
        }
    }
} 