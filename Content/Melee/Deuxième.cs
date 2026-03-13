
using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Resources;
using FranciumCalamityWeapons.Content.Projectiles;
using FranciumCalamityWeapons.Content.Resources;
using GlowmaskHelper.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Melee
{
	[AutoloadGlowmask]
	public class Deuxième : ModItem
	{
		public override void SetDefaults() 
		{
			Item.width = 164;
			Item.height = 164;
			Item.value = Item.sellPrice(gold: 2, silver: 50);
			Item.rare = ItemRarityID.White;

			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 70;
			Item.autoReuse = true;
			Item.damage = 6000;
			Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
			Item.noMelee = true; 
			Item.noUseGraphic = true; 
            Item.crit = 10;
			Item.channel = true;
			Item.shoot = ModContent.ProjectileType<DeuxièmeSwing>();
		}

		public override bool MeleePrefix() 
		{
			return true;
		}

       	public override void AddRecipes() {
			
			CreateRecipe()
			.AddIngredient<Colossus>(8)
			.AddIngredient<AuricBar>(4)
			.AddIngredient<AscendantSpiritEssence>(10)
			.AddTile<CosmicAnvil>()
			.Register();
		}
	}
}