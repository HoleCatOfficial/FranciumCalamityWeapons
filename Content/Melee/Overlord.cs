
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
	public class Overlord : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 174;
			Item.height = 174;
			Item.value = Item.sellPrice(gold: 25, silver: 60);
			Item.rare = ModContent.RarityType<NewCosmicRarity>();
			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 17; 
			Item.autoReuse = true;
			Item.damage = 1100;
			Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.channel = true;
			Item.shoot = ModContent.ProjectileType<OverlordSwing>();
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