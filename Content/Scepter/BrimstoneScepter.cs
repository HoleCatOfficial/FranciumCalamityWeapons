using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using static Terraria.ModLoader.ModContent;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Common;
using DestroyerTest.Rarity;
using DestroyerTest.Rarity.Scepter;
using DestroyerTest.Content.Projectiles.Weapon.Scepter;
using DestroyerTest.Content.Scepter;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Items.Materials;
using FranciumCalamityWeapons.Content.Projectiles; // Add this line if CT3_Swing is in the Projectiles namespace

namespace FranciumCalamityWeapons.Content.Scepter
{
	public class BrimstoneScepter : ScepterItem
	{
        public override int Width => 58;
        public override int Height => 58;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 40;
            ShootCrit = 4;
            ThrowCrit = 14;
            KB = 6;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<WineRarity>();

            // Assign projectile types
            ShootID = ModContent.ProjectileType<BrimstoneShot>();
            ThrowID = ModContent.ProjectileType<BrimstoneScepterThrown>();

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient<UnholyCore>(6)
				.AddTile(TileID.MythrilAnvil)
				.Register();
        }
    }

    
} 