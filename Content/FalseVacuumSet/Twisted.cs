using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Tiles.Furniture.CraftingStations;
using DestroyerTest.Content.RogueItems;
using FranciumCalamityWeapons.Content.Debuffs;
using FranciumCalamityWeapons.Content.Projectiles;
using FranciumCalamityWeapons.Content.Resources;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.FalseVacuumSet
{
	public class Twisted : ModItem
	{
		public override void SetDefaults() {
			// Alter any of these values as you see fit, but you should probably keep useStyle on 1, as well as the noUseGraphic and noMelee bools

			// Common Properties
			Item.rare = ItemRarityID.Pink;
			Item.value = Item.sellPrice(silver: 5);
			Item.maxStack = 1;
            Item.width = 102;
            Item.height = 102;

			// Use Properties
            Item.useStyle = ItemUseStyleID.Swing;
			Item.useAnimation = 10;
			Item.useTime = 10;
            Item.UseSound = SoundID.Item103;
			
			Item.autoReuse = true;
			Item.consumable = false;

			// Weapon Properties			
			Item.damage = 1200;
			Item.knockBack = 5f;
			Item.noUseGraphic = true; // The item should not be visible when used
			Item.noMelee = true; // The projectile will do the damage and not the item
			Item.DamageType = DamageClass.Ranged;

			// Projectile Properties
			Item.shootSpeed = 2f;
			Item.shoot = ModContent.ProjectileType<FracturedPhantom>(); // The projectile that will be thrown
		}

        public override bool? UseItem(Player player)
        {
			if (player.altFunctionUse != 2)
			{
				player.AddBuff(ModContent.BuffType<FracturedPhantomBuff>(), 120);
			}
			else
			{
				player.ClearBuff(ModContent.BuffType<FracturedPhantomBuff>());
				FracturedPhantomBuff.Inf = false;
			}
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
			return true;
        }


		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient<HeatDeath>(13)
				.AddIngredient<EtherealSubjugator>(1)
				.AddIngredient<LoreAwakening>(1)
				.AddTile<DraedonsForge>()
				.Register();
		}
	}
}