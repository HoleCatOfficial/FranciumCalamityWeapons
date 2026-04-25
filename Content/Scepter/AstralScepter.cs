using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.FurnitureMonolith;
using CalamityMod.Projectiles.Typeless;
using DestroyerTest.Content.Scepter;
using DestroyerTest.Rarity.Scepter;
using FranciumCalamityWeapons.Content.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Scepter
{
    public class AstralScepter : ScepterItem
    {
        public override int Width => 56;
        public override int Height => 56;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            // First let the base class handle core setup
            base.SetDefaults();

            // Override stats unique to this scepter
            ShootDMG = 40;
            ShootCrit = 4;
            ThrowCrit = 14;
            KB = 6;
            AdditiveValue = 1;
            Rarity = ModContent.RarityType<WineRarity>();

            // Assign projectile types
            ShootID = ModContent.ProjectileType<AstralStar>();
            ThrowID = ModContent.ProjectileType<AstralScepterThrown>();

            // Refresh defaults after overriding values
            base.SetDefaults();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AstralMonolith>(16)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
