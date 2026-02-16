
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
			Item.DamageType = DamageClass.Melee;
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
	}
}