
using FranciumCalamityWeapons.Common.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Melee
{
    public class CosmicBeam : ModProjectile
    {
        public override string Texture => "FranciumCalamityWeapons/Content/Projectiles/CosmicBeam";
		
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.TerraBeam);
            Projectile.width = 30;
            Projectile.height = 100;
            Projectile.aiStyle = 27; // Terra Beam AI style
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
			{
				if (calamityMod.TryFind("GodSlayerInferno", out ModBuff GSI))
				{
					target.AddBuff(GSI.Type, 240);
				}
			}
		}
    }
	public class CosmicSaber : ModItem
	{
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(1, 4));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        }
        public override void SetDefaults() {
            Item.CloneDefaults(ItemID.TerraBlade);
			Item.width = 68; // The item texture's width.
			Item.height = 68; // The item texture's height.

			Item.useStyle = ItemUseStyleID.Swing; // The useStyle of the Item.
			Item.useTime = 20; // The time span of using the weapon. Remember in terraria, 60 frames is a second.
			Item.useAnimation = 20; // The time span of the using animation of the weapon, suggest setting it the same as useTime.
			Item.autoReuse = true; // Whether the weapon can be used more than once automatically by holding the use button.

			Item.DamageType = DamageClass.Melee; // Whether your item is part of the melee class.
			Item.damage = 300; // The damage your item deals.
			Item.knockBack = 0; // The force of knockback of the weapon. Maximum is 20
			Item.crit = 54; // The critical strike chance the weapon has. The player, by default, has a 4% critical strike chance.

			Item.value = Item.buyPrice(gold: 1); // The value of the weapon in copper coins.
			Item.rare = ModContent.RarityType<NewCosmicRarity>(); // Give this item our custom rarity.
			Item.shoot = ModContent.ProjectileType<CosmicBeam>();
		}

        

		public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Spawn the custom Terra Beam projectile
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<CosmicBeam>(), damage, knockback, player.whoAmI);
            SoundEngine.PlaySound(SoundID.Item60, player.position);
            return false; // Return false to prevent the default projectile from being spawned
        }


		//public override void MeleeEffects(Player player, Rectangle hitbox) {
			//if (Main.rand.NextBool(3)) {
				// Emit dusts when the sword is swung
				//Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, ModContent.DustType<Dusts.Sparkle>());
			//}
		//}

		public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) {
			if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
			{
				if (calamityMod.TryFind("GodSlayerInferno", out ModBuff GSI))
				{
					target.AddBuff(GSI.Type, 240);
				}
			}
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes() {
			if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod) && ModLoader.TryGetMod("DestroyerTest", out Mod DestroyerTest))
			{
				if (calamityMod.TryFind("CosmiliteBar", out ModItem CB)
                    && calamityMod.TryFind("DarkPlasma", out ModItem DP)
                    && DestroyerTest.TryFind("GildingMetal", out ModItem GM)
                    && calamityMod.TryFind("CosmicAnvil", out ModTile CA))
				{
					Recipe recipe = CreateRecipe();
					recipe.AddIngredient(CB.Type, 20);
                    recipe.AddIngredient(DP.Type, 18);
					recipe.AddTile(CA.Type);
					recipe.Register();
				}
			}
		}
	}
}