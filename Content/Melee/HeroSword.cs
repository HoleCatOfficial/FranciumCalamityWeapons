using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using FranciumCalamityWeapons.Content.Projectiles; // Add this line if CT3_Swing is in the Projectiles namespace

namespace FranciumCalamityWeapons.Content.Melee
{
	public class HeroSword : ModItem
	{
        
       
        
        //Weapon Properties
        public override void SetDefaults() {
			// Common Properties
			Item.width = 30;
			Item.height = 108;
			Item.value = Item.sellPrice(gold: 2, silver: 50);
			Item.rare = ItemRarityID.Pink;

			// Use Properties
			// Note that useTime and useAnimation for this item don't actually affect the behavior because the held projectile handles that. 
			// Each attack takes a different amount of time to execute
			// Conforming to the item useTime and useAnimation makes it much harder to design
			// It does, however, affect the item tooltip, so don't leave it out.
			Item.useTime = 80;
			Item.useAnimation = 80;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.UseSound = null; // The sound when the weapon is being used.

			// Weapon Properties
			Item.knockBack = 30;  // The knockback of your sword, this is dynamically adjusted in the projectile code.
			Item.autoReuse = true; // This determines whether the weapon has autoswing
			Item.damage = 350; // The damage of your sword, this is dynamically adjusted in the projectile code.
			Item.DamageType = DamageClass.Melee; // Deals melee damage
            Item.crit = 46; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.
			Item.noMelee = true;  // This makes sure the item does not deal damage from the swinging animation
			Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand

			// Projectile Properties
			Item.shoot = ModContent.ProjectileType<HeroSwordSlash>(); // The sword as a projectile
		}

		


		public override void UpdateInventory(Player player)
		{
			HeroSwordSlash Slash = ModContent.GetInstance<HeroSwordSlash>();

		}

	

		public override void AddRecipes()
		{
			if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod) && ModLoader.TryGetMod("DestroyerTest", out Mod DestroyerTest))
			{
				if (calamityMod.TryFind("WulfrumScrap", out ModItem WS) && calamityMod.TryFind("DubiousPlating", out ModItem DP) && DestroyerTest.TryFind("Steel", out ModItem S))
				{
					Recipe recipe = CreateRecipe(3);
					recipe.AddIngredient(DP.Type, 2);
					recipe.AddIngredient(WS.Type, 1);
					recipe.AddIngredient(S.Type, 1);
					recipe.AddTile(TileID.Anvils);
					recipe.Register();
				}
			}
		}
    }
} 