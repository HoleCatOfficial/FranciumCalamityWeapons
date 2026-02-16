
using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Resources;
using FranciumCalamityWeapons.Content.Projectiles;
using FranciumCalamityWeapons.Content.Resources;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Melee
{
	public class Deuxième : ModItem
	{
		public override void SetDefaults() 
		{
			// Common Properties
			Item.width = 164;
			Item.height = 164;
			Item.value = Item.sellPrice(gold: 2, silver: 50);
			Item.rare = ItemRarityID.Green;

			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 70;
			Item.autoReuse = true;
			Item.damage = 7600;
			Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
			Item.noMelee = true; 
			Item.noUseGraphic = true; 
            Item.crit = 46;
			Item.channel = true;
			Item.shoot = ModContent.ProjectileType<DeuxièmeSwing>();
		}

		public override bool MeleePrefix() 
		{
			return true;
		}

       	public override void AddRecipes() {
			
			CreateRecipe()
			.AddIngredient<Gargantua>(8)
			.AddIngredient<Tenebris>(8)
			.AddIngredient<LifeAlloy>(4)
			.AddIngredient<AscendantSpiritEssence>(10)
			.AddTile<CosmicAnvil>()
			.Register();
		}
	}
}