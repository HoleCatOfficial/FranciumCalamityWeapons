
using CalamityMod;
using CalamityMod.NPCs.DevourerofGods;
using FranciumCalamityWeapons.Common.Rarities;
using FranciumCalamityWeapons.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Melee
{
	// ExampleCustomSwingSword is an example of a sword with a custom swing using a held projectile
	// This is great if you want to make melee weapons with complex swing behavior
	public class Overlord : ModItem
	{
		public override void SetDefaults()
		{
			// Common Properties
			Item.width = 174;
			Item.height = 174;
			Item.value = Item.sellPrice(gold: 25, silver: 60);
			Item.rare = ModContent.RarityType<NewCosmicRarity>();


			// Use Properties
			// Note that useTime and useAnimation for this item don't actually affect the behavior because the held projectile handles that. 
			// Each attack takes a different amount of time to execute
			// Conforming to the item useTime and useAnimation makes it much harder to design
			// It does, however, affect the item tooltip, so don't leave it out.
			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = ItemUseStyleID.Shoot;

			// Weapon Properties
			Item.knockBack = 17;  // The knockback of your sword, this is dynamically adjusted in the projectile code.
			Item.autoReuse = true; // This determines whether the weapon has autoswing
			Item.damage = 200; // The damage of your sword, this is dynamically adjusted in the projectile code.
			Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>(); // Deals melee damage
			Item.noMelee = true;  // This makes sure the item does not deal damage from the swinging animation
			Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand

			// Projectile Properties
			Item.shoot = ModContent.ProjectileType<OverlordSwing>(); // The sword as a projectile
		}

		public override bool MeleePrefix()
		{
			return true; // return true to allow weapon to have melee prefixes (e.g. Legendary)
		}

		public static bool OverlordIsHeld = false;

        public override void HoldItem(Player player)
		{
			OverlordIsHeld = true;
			base.HoldItem(player);
		}
	}
	

	public class OL_DROP_NPC : GlobalNPC
	{
		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
		{
			if (npc.type == ModContent.NPCType<DevourerofGodsHead>())
			{
				npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Overlord>(), 1, 1, 1));
			}
		}
	}
}