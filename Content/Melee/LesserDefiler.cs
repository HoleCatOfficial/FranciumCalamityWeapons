

using FranciumCalamityWeapons.Common.Rarities;
using FranciumCalamityWeapons.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Melee
{
	public class LesserDefiler : ModItem
	{
		public override void SetDefaults() {
			Item.width = 60; // The item texture's width.
			Item.height = 60; // The item texture's height.

			Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
			Item.useTime = 20; // The time span of using the weapon. Remember in terraria, 60 frames is a second.
			Item.useAnimation = 20; // The time span of the using animation of the weapon, suggest setting it the same as useTime.
			Item.autoReuse = true; // Whether the weapon can be used more than once automatically by holding the use button.

			Item.DamageType = DamageClass.MeleeNoSpeed; // Deals melee damage
			Item.damage = 130; // The damage your item deals.
			Item.knockBack = 20; // The force of knockback of the weapon. Maximum is 20
			Item.crit = 36; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.
			Item.shoot = ModContent.ProjectileType<LesserDefiler_Flail>(); // The flail projectile

			Item.value = Item.buyPrice(gold: 1); // The value of the weapon in copper coins.
			Item.rare = ModContent.RarityType<NewCosmicRarity>(); // Give this item our custom rarity.
			Item.UseSound = SoundID.Item1; // The sound when the weapon is being used.
			Item.channel = true;
			Item.noMelee = true; // This makes sure the item does not deal damage from the swinging animation
			Item.noUseGraphic = true;
		}

		//public override void MeleeEffects(Player player, Rectangle hitbox) {
			//if (Main.rand.NextBool(3)) {
				// Emit dusts when the sword is swung
				//Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<Dusts.Sparkle>());
			//}
		//}

		
		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod) && ModLoader.TryGetMod("DestroyerTest", out Mod DestroyerTest))
			{
				if (calamityMod.TryFind("MeldConstrust", out ModItem MC) && calamityMod.TryFind("TwistingNether", out ModItem TN) && DestroyerTest.TryFind("GildingMetal", out ModItem GM))
				{
					Recipe recipe = CreateRecipe();
					recipe.AddIngredient(MC.Type, 8);
					recipe.AddIngredient(TN.Type, 8);
					recipe.AddIngredient(GM.Type, 6);
					recipe.AddTile(TileID.LunarCraftingStation);
					recipe.Register();
				}
			}
		}
	}
}