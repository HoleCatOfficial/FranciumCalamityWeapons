
using DestroyerTest.Content.Projectiles;
using FranciumCalamityWeapons.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.FalseVacuumSet
{

	public class GazeofNoxus : ModItem
	{

		public override void SetDefaults()
		{
			// Common Properties
			Item.width = 44;
			Item.height = 58;
			Item.value = Item.sellPrice(gold: 2, silver: 50);
			Item.rare = ItemRarityID.Purple; // Use the custom rarity defined in DestroyerTest.Rarity.RarityType


			// Use Properties
			// Note that useTime and useAnimation for this item don't actually affect the behavior because the held projectile handles that. 
			// Each attack takes a different amount of time to execute
			// Conforming to the item useTime and useAnimation makes it much harder to design
			// It does, however, affect the item tooltip, so don't leave it out.
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.channel = true;

			// Weapon Properties
			Item.autoReuse = true;
			Item.knockBack = 7;  // The knockback of your sword, this is dynamically adjusted in the projectile code.
			Item.damage = 500; // The damage of your sword, this is dynamically adjusted in the projectile code.
			Item.DamageType = DamageClass.Magic; // Deals melee damage
			Item.mana = 6;
			Item.noMelee = true;  // This makes sure the item does not deal damage from the swinging animation
			Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand

			// Projectile Properties
			Item.shoot = ModContent.ProjectileType<GazeofNoxusHoldout>(); // The sword as a projectile
		}

		public override bool CanUseItem(Player player)
		{
			return player.ownedProjectileCounts[ModContent.ProjectileType<GazeofNoxusHoldout>()] < 1;
		}
		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 spawnPos = player.MountedCenter;
			Projectile.NewProjectile(source, spawnPos, Vector2.Zero, type, damage, knockback, player.whoAmI);
			return false; // Don't shoot automatically
		}



		
	}
}