using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using FranciumCalamityWeapons.Content.Projectiles;

namespace FranciumCalamityWeapons.Content.Melee
{
	public class WhiteDwarf : ModItem
	{
		public override void SetDefaults() {
			// Alter any of these values as you see fit, but you should probably keep useStyle on 1, as well as the noUseGraphic and noMelee bools

			// Common Properties
			Item.rare = ItemRarityID.Pink;
			Item.value = Item.sellPrice(silver: 5);
			Item.maxStack = 999;

			// Use Properties
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useAnimation = 8;
			Item.useTime = 8;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.consumable = false;

			// Weapon Properties			
			Item.damage = 120;
			Item.knockBack = 5f;
			Item.noUseGraphic = true; // The item should not be visible when used
			Item.noMelee = true; // The projectile will do the damage and not the item
			Item.DamageType = DamageClass.Melee;

			// Projectile Properties
			Item.shootSpeed = 17f;
			Item.shoot = ModContent.ProjectileType<WhiteDwarfProjectile>(); // The projectile that will be thrown
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod) && ModLoader.TryGetMod("DestroyerTest", out Mod DestroyerTest))
			{
				if (calamityMod.TryFind("CosmiliteBar", out ModItem CB)
                    && calamityMod.TryFind("DarkPlasma", out ModItem DP)
                    && DestroyerTest.TryFind("GildingMetal", out ModItem GM)
                    && calamityMod.TryFind("CosmicAnvil", out ModTile CA))
				{
					Recipe recipe = CreateRecipe();
					recipe.AddIngredient(CB.Type, 20);
                    recipe.AddIngredient(DP.Type, 18);
					recipe.AddIngredient(GM.Type, 6);
					recipe.AddTile(CA.Type);
					recipe.Register();
				}
			}
		}
	}
}