using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Common;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using DestroyerTest.Content.Scepter;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.Ores;

namespace FranciumCalamityWeapons.Content.Scepter
{
	public class LunarVoltageScepter : ScepterItem
	{
        public override int Width => 78;
        public override int Height => 78;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 250;
            ShootCrit = 4;
            ThrowCrit = 14;
            KB = 16;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<IncarnadineRarity>();

            // Assign projectile types
            ShootID = ModContent.ProjectileType<LunarElectricityArc>();
            ThrowID = ModContent.ProjectileType<LunarVoltageScepterThrown>();

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void ShootDefaults()
        {
            base.ShootDefaults();
            Item.shootSpeed = 40;
        }

        public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient<TheTransformer>()
                .AddIngredient(ItemID.LunarBar, 16)
                .AddIngredient<ExodiumCluster>(10)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
        }
    }

    
} 