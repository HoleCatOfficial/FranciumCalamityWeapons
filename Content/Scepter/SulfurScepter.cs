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
using DestroyerTest.Rarity.Scepter;

namespace FranciumCalamityWeapons.Content.Scepter
{
	public class SulfurScepter : ScepterItem
	{
        public override int Width => 90;
        public override int Height => 96;

		public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
			base.SetDefaults();

            ShootDMG = 28;
            ShootCrit = 16;
            ThrowCrit = 16;
            KB = 5;
            AdditiveValue = Item.sellPrice(silver: 80);
            Rarity = ModContent.RarityType<PaleFuchsiaRarity>();

            //ShootID = ModContent.ProjectileType<SeaPrismChain_Shaft>();
			ShootID = ModContent.ProjectileType<CausticBlob>();
            ThrowID = ModContent.ProjectileType<SulfurScepterThrown>();

            ShootSound = SoundID.DD2_SkyDragonsFuryShot;
            ThrowSound = SoundID.Item169;

        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (player.altFunctionUse != 2)
			{
				for(int u = 0; u < 5; u++)
				{
					Projectile.NewProjectile(source, position, velocity.RotatedByRandom(1), ModContent.ProjectileType<CausticBlob>(), damage, 5, player.whoAmI);
				}
				for(int t = 0; t < 3; t++)
				{
					Projectile.NewProjectile(source, position, velocity.RotatedByRandom(1), ModContent.ProjectileType<CausticShot>(), damage / 3, 5, player.whoAmI);
				}
			}
			else
			{
				Projectile.NewProjectile(source, position, velocity, ThrowID, damage, knockback, player.whoAmI);
			}
            return false;
        }

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<SulphuricScale>(16)
            .AddIngredient<SulphurousSand>(22)
            .AddTile(TileID.Anvils)
            .Register();
        }
    }
} 