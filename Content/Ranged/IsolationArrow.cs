
using FranciumCalamityWeapons.Content.Projectiles;
using FranciumCalamityWeapons.Content.Resources;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Ranged
{
	// This example is similar to the Wooden Arrow item
	public class IsolationArrow : ModItem
	{
		public override void SetStaticDefaults() {
			Item.ResearchUnlockCount = 99;
		}

		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 62;

			Item.damage = 300; // Keep in mind that the arrow's final damage is combined with the bow weapon damage.
			Item.DamageType = DamageClass.Ranged;

			Item.maxStack = Item.CommonMaxStack;
			Item.consumable = true;
			Item.knockBack = 1.5f;
			Item.value = Item.sellPrice(copper: 16);
			Item.shoot = ModContent.ProjectileType<IsolationArrowProjectile>(); // The projectile that weapons fire when using this item as ammunition.
			Item.shootSpeed = 3f; // The speed of the projectile.
			Item.ammo = AmmoID.Arrow; // The ammo class this ammo belongs to.
		}

		// For a more detailed explanation of recipe creation, please go to Content/ExampleRecipes.cs.
		public override void AddRecipes() {
			if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod) && ModLoader.TryGetMod("DestroyerTest", out Mod DestroyerTest) && ModLoader.TryGetMod("CalamityEntropy", out Mod CalamityEntropy))
            {
                if (calamityMod.TryFind("VanguisherArrow", out ModItem VA) && calamityMod.TryFind("DraedonsForge", out ModTile DF))
                {
                    Recipe recipe = CreateRecipe(2);
                    recipe.AddIngredient<HeatDeath>(1);
                    recipe.AddIngredient(VA.Type, 1);
                    recipe.AddTile(DF.Type);
                    recipe.Register();
                }
            }
		}
	}
}