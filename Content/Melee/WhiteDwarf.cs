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
	}
}