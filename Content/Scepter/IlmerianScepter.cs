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
using CalamityMod.Items.Placeables.SunkenSea;
using CalamityMod.Tiles.FurnitureNavystone.FurnitureAncientNavystone;

namespace FranciumCalamityWeapons.Content.Scepter
{

	public class IlmerianScepter : ScepterItem
	{
		SoundStyle Spine = new SoundStyle("FranciumCalamityWeapons/Audio/IlmerisSpine");
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

            ShootID = ModContent.ProjectileType<SeaPrismChain_Shaft>();
            ThrowID = ModContent.ProjectileType<IlmerianScepterThrown>();

            ShootSound = Spine;
            ThrowSound = SoundID.Item169;
        }

		public override void ShootDefaults()
        {
            base.ShootDefaults();
            Item.shootSpeed = 0.001f;
        }

		public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient<Navystone>(8)
				.AddIngredient<SeaPrism>(6)
				.AddIngredient<PearlShard>(4)
				.AddIngredient(ItemID.GoldBar, 4)
				.AddTile<EutrophicShelf>()
				.Register();
        }
    }
} 