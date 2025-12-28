using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Resources.Cloths;
using DestroyerTest.Content.SummonItems;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Rarity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System;
using InnoVault.PRT;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using Terraria.Audio;
using CalamityMod.Items.Armor.Tarragon;
using CalamityMod.Items.Materials;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using OpusLib;
using DestroyerTest;
using Microsoft.Build.Evaluation;

namespace FranciumCalamityWeapons.Content.Equips
{

	[AutoloadEquip(EquipType.Head)]
	public class TarragonMane : ModItem
	{

		public override void SetStaticDefaults()
		{
			// If your head equipment should draw hair while drawn, use one of the following:
			// ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false; // Don't draw the head at all. Used by Space Creature Mask
			//ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true; // Draw hair as if a hat was covering the top. Used by Wizards Hat
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true; // Draw all hair as normal. Used by Mime Mask, Sunglasses
            // ArmorIDs.Head.Sets.DrawsBackHairWithoutHeadgear[Item.headSlot] = true;

		}

		public override void SetDefaults()
		{
			Item.width = 30; // Width of the item
			Item.height = 26; // Height of the item
			Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
			Item.rare = ItemRarityID.Green; // The rarity of the item
			Item.defense = 30; // The amount of defense the item will give when equipped
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<TarragonBreastplate>() && legs.type == ModContent.ItemType<TarragonLeggings>();
		}

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.FranciumCalamityWeapons.Items.TarragonMane.SetBonus");
            if (player.TryGetModPlayer<TarragonManePlayer>(out var Mane))
            {
                Mane.Active = true;
            }
        }

		public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

		public override void AddRecipes()
		{
            CreateRecipe()
				.AddIngredient<UelibloomBar>(7)
                .AddIngredient<DivineGeode>(6)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}
	}

    public class TarragonManePlayer : ModPlayer
    {
        public bool Active = false;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateEquips()
        {
            if (Active)
            {
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.TryGetGlobalProjectile<TarragonThrownScepter>(out var Scptr))
                    {
                        Scptr.Active = true;
                    }
                }

                if (DestroyerTestMod.ArmorSetBonusHotKey.JustPressed)
                {
                    foreach(Projectile bomb in Main.projectile)
                    {
                        if (bomb.active && bomb.owner == Player.whoAmI && bomb.type == ModContent.ProjectileType<TarragonManeBomb>())
                        {
                            bomb.ai[0] = 1;
                        }
                    }
                }
            }
        }
    }

    public class TarragonThrownScepter : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool IsAThrownScepter = false;
        public bool Active = false;

        public override void SetDefaults(Projectile entity)
        {
            if (entity.DamageType == ModContent.GetInstance<ScepterClass>() && entity.Name.Contains("Thrown"))
            {
                IsAThrownScepter = true;
            }
        }
        public override void PostAI(Projectile projectile)
        {
            if (Active)
            {
                if (Main.rand.NextBool(24) && IsAThrownScepter)
                {
                    Projectile.NewProjectile(projectile.GetSource_FromAI(), projectile.Center, Main.rand.NextVector2Circular(10, 10), ModContent.ProjectileType<TarragonManeBomb>(), projectile.damage / 2, 0, projectile.owner);
                }
            }
        }
    }

    public class TarragonManeBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ai[0] == 1;
        }

        public override void AI()
        {
            bool Dormant = Projectile.ai[0] == 0;

            Projectile.velocity *= 0.99f;
            
            if (Dormant)
            {
                DormantAI();
            }
            else
            {
                Projectile.ai[0] = 1;
                Projectile.Kill();
            }
        }

        private void DormantAI()
        {
            Projectile.timeLeft = 5;
            if(Main.rand.NextBool(6))
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<AuricBarDust>(), Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1), 70, default, 1f);
                dust.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[1] == 0)
            {
                Projectile.alpha = 255;
                Projectile.Resize(100, 100);
                SoundEngine.PlaySound(SoundID.Item110, Projectile.Center);

                foreach (NPC tg in Main.npc)
                {
                    if (tg.Distance(Projectile.Center) < 50)
                    {
                        tg.AddBuff(ModContent.BuffType<ArmorCrunch>(), 600);
                    }
                }

                // Spawn a bunch of smoke dusts.
                for (int i = 0; i < 30; i++) {
                    Dust smokeDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                    smokeDust.velocity *= 1.4f;
                }

                // Spawn a bunch of fire dusts.
                for (int j = 0; j < 20; j++) {
                    Dust fireDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.FireworksRGB, 0f, 0f, 100, Color.Green, 3.5f);
                    fireDust.velocity *= 7f;
                }

                // Spawn a bunch of smoke gores.
                for (int k = 0; k < 2; k++) {
                    float speedMulti = 0.4f;
                    if (k == 1) {
                        speedMulti = 0.8f;
                    }

                    Gore smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                    smokeGore.velocity *= speedMulti;
                    smokeGore.velocity += Vector2.One;
                    smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                    smokeGore.velocity *= speedMulti;
                    smokeGore.velocity.X -= 1f;
                    smokeGore.velocity.Y += 1f;
                    smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                    smokeGore.velocity *= speedMulti;
                    smokeGore.velocity.X += 1f;
                    smokeGore.velocity.Y -= 1f;
                    smokeGore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.position, default, Main.rand.Next(GoreID.Smoke1, GoreID.Smoke3 + 1));
                    smokeGore.velocity *= speedMulti;
                    smokeGore.velocity -= Vector2.One;
                }
                Projectile.ai[1] = 1;
            }
        }
    }
}