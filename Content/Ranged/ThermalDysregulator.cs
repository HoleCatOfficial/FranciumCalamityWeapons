using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using DestroyerTest.Content.Projectiles; // Add this line if CT3_Swing is in the Projectiles namespace
using DestroyerTest.Rarity;
using FranciumCalamityWeapons.Content.Projectiles;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;

namespace FranciumCalamityWeapons.Content.Ranged
{
    public class ThermalDysregulator : ModItem
    {

        //Weapon Properties
        public override void SetDefaults()
        {
            // Common Properties
            Item.width = 34;
            Item.height = 42;
            Item.value = Item.sellPrice(gold: 35, silver: 72, copper: 6);
            Item.rare = ItemRarityID.Red;

            // Use Properties
            // Note that useTime and useAnimation for this item don't actually affect the behavior because the held projectile handles that. 
            // Each attack takes a different amount of time to execute
            // Conforming to the item useTime and useAnimation makes it much harder to design
            // It does, however, affect the item tooltip, so don't leave it out.
            Item.useTime = 80;
            Item.useAnimation = 80;
            Item.useStyle = ItemUseStyleID.Shoot;

            // Weapon Properties
            Item.knockBack = 10;  // The knockback of your sword, this is dynamically adjusted in the projectile code.
            Item.autoReuse = true; // This determines whether the weapon has autoswing
            Item.damage = 65; // The damage of your sword, this is dynamically adjusted in the projectile code.
            Item.DamageType = DamageClass.Magic; // Deals melee damage\
            Item.channel = true;
            Item.mana = 15;
            Item.crit = 16; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.
            Item.noMelee = true;  // This makes sure the item does not deal damage from the swinging animation
            Item.noUseGraphic = true; // This makes sure the item does not get shown when the player swings his hand
            Item.useTurn = true;

            // Projectile Properties
            Item.shoot = ModContent.ProjectileType<ThermalDysregulatorHoldout>(); // The sword as a projectile
        }

        public override bool CanShoot(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] < 1;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ScoriaBar>(), 12)
                .AddIngredient(ModContent.ItemType<PyreMantle>(), 12)
                .AddIngredient(ItemID.Hellstone, 6)
                .AddIngredient(ModContent.ItemType<WaterCanister>(), 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }

    public class WaterCanister : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.value = Item.sellPrice(gold: 25, silver: 70);
            Item.rare = ItemRarityID.Red;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<ScoriaBar>(), 2)
                .AddIngredient(ItemID.IronBar, 6)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
} 