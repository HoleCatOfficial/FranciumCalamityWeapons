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
using CalamityMod.Items.Materials; // Add this line if CT3_Swing is in the Projectiles namespace

namespace FranciumCalamityWeapons.Content.Scepter
{
	public class VitalBane : ScepterItem
	{
        public override int Width => 80;
        public override int Height => 80;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 310;
            ShootCrit = 4;
            ThrowCrit = 14;
            KB = 6;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<IncarnadineRarity>();

            // Assign projectile types
            ShootID = ModContent.ProjectileType<TerratomereSwordBeam>();
            ThrowID = ModContent.ProjectileType<VitalBaneThrown>();

            // Optional: change sounds
            ShootSound = new SoundStyle("FranciumCalamityWeapons/Audio/BrimstoneBigShoot") with { MaxInstances = 0, PitchVariance = 0.5f };
            ThrowSound = new SoundStyle("FranciumCalamityWeapons/Audio/MagicSwing", 3) with { MaxInstances = 0, PitchVariance = 0.5f };

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient<UelibloomBar>(6)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
        }
    }

    
} 