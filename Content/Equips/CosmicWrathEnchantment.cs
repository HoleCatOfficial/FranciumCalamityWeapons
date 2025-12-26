
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.RiftArsenal;
using DestroyerTest.Content.Equips;
using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using DestroyerTest.Rarity;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.RangedItems;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Buffs;
using Terraria.Audio;
using InnoVault.PRT;
using CalamityMod.Tiles.Furniture.CraftingStations;
using FranciumCalamityWeapons.Content.Resources;
using FranciumCalamityWeapons.Content.Melee;
using FranciumCalamityWeapons.Content.Particles;
using CalamityMod.Items.Accessories;
using CalamityMod.Buffs.DamageOverTime;
using System.Collections.Generic;
using System.Linq;
using FranciumCalamityWeapons.Common.Rarities;
using Terraria.DataStructures;
using FranciumCalamityWeapons.Common;
using System;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Items.Materials;
using FranciumMultiCrossMod.Content.Equips;

namespace FranciumCalamityWeapons.Content.Equips
{
    [JITWhenModsEnabled("FranciumMultiCrossMod")]
    public class CosmicWrathEnchantment : ModItem
    {
        public static readonly int MultiplicativeDamageBonus = 12;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(MultiplicativeDamageBonus);

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 24));
        }
        public override void SetDefaults()
        {
            Item.width = 78;
            Item.height = 78;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<NewCosmicRarity>();
            Item.value = Item.sellPrice(platinum: 6);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CosmicWrathEnchantmentPlayer CWPlayer = player.GetModPlayer<CosmicWrathEnchantmentPlayer>();


            float lifePercent = (float)player.statLife / player.statLifeMax2;
            float dynamicBonus = MultiplicativeDamageBonus + ((1f - lifePercent) * MultiplicativeDamageBonus * 2);
            player.GetDamage(DamageClass.Generic) *= 1f + (dynamicBonus / 100f);

            CWPlayer.CosmicWrathEnchantment = true;
        }

        public override void AddRecipes()
        {
            if (CrossMod.DTCrossModLoaded)
            {
                if (CrossMod.DTCrossMod.TryFind("HeliciteEnchantment", out ModItem HE))
                {
                    CreateRecipe()
                    .AddIngredient<FalseVacuum>(5)
                    .AddIngredient<DarksunFragment>(5)
                    .AddIngredient<Overlord>()
                    .AddIngredient<NebulousCore>()
                    .AddIngredient(HE.Type)
                    .AddTile<CosmicAnvil>()
                    .Register();
                }
            }
        }
	}

    [JITWhenModsEnabled("FranciumMultiCrossMod")]
    public class CosmicWrathEnchantmentPlayer : ModPlayer
    {
        public bool CosmicWrathEnchantment = false;

        public int Cooldown = 7200;
        public int currentCooldown = 0;

        public override void ResetEffects()
            {
                base.ResetEffects();
                CosmicWrathEnchantment = false;
            }

        public override void OnRespawn()
        {
            currentCooldown = 0;
        }

        public override void PostUpdate()
        {
            int Rad = 130;
            Vector2 center = Player.Center;
            Vector2 Spawn = Main.rand.NextVector2CircularEdge(Rad, Rad) + center;
            Vector2 ToPlayer = center - Spawn;
            if (currentCooldown > 0)
            {
                if (Main.rand.NextBool(2))
                {
                    for (int i = 0; i < 3; i++)
                    {
                        PRTLoader.NewParticle(PRTLoader.GetParticleID<SmallShine>(), Spawn, ToPlayer * 0.03f, CalCTAUtils.GodSlayerInfernoGradient((float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)))), Main.rand.NextFloat(0.5f, 1.5f));
                    }
                   
                }
                currentCooldown--;
            }

            if (currentCooldown == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/CosmicWrathEnchantmentRegen"), Player.position);
                    PRTLoader.NewParticle(PRTLoader.GetParticleID<BloomRingSharp>(), Player.Center, Vector2.Zero, CalCTAUtils.GodSlayerInfernoGradient((float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)))), 3f);
                }
        }

        private void TrySurviveFatalHit(Player.HurtInfo hurtInfo)
            {
                if (!CosmicWrathEnchantment || currentCooldown > 0)
                return;

                if (hurtInfo.Damage > Player.statLife)
                {
                Player.GetModPlayer<ScreenshakePlayer>().screenshakeTimer = 5;
			    Player.GetModPlayer<ScreenshakePlayer>().screenshakeMagnitude = 16;
                PRTLoader.NewParticle(PRTLoader.GetParticleID<BloomRingSharp>(), Player.Center, Vector2.Zero, CalCTAUtils.GodSlayerInfernoGradient((float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)))), 1f);
                SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/CosmicWrathEnchantmentDeath"), Player.position);
                Player.statLife = Player.statLifeMax2;
                CombatText.NewText(Player.getRect(), CalCTAUtils.GodSlayerInfernoGradient((float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)))), "Death Evaded!", true);
                Player.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 1200); // 10 seconds of Bleeding debuff
                currentCooldown = Cooldown;
                hurtInfo.Damage = 0;
                //Player.NinjaDodge(); // Optional: visual effect
                }
            }



        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {

            TrySurviveFatalHit(hurtInfo);
            for (int i = 0; i < 7; i++)
            {
            Vector2 HighInTheSky = new Vector2(Main.rand.NextFloat(-100f, 100f), -400f);
            Vector2 ToPlayer = Player.Center - HighInTheSky;
            Projectile.NewProjectile(npc.GetSource_OnHurt(npc), HighInTheSky, ToPlayer, ProjectileID.ManaCloakStar, 200, 1f);
            }
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            TrySurviveFatalHit(hurtInfo);
                for (int i = 0; i < 7; i++)
            {
            Vector2 HighInTheSky = new Vector2(Main.rand.NextFloat(-100f, 100f), -400f);
            Vector2 ToPlayer = Player.Center - HighInTheSky;
            Projectile.NewProjectile(proj.GetSource_OnHurt(proj), HighInTheSky, ToPlayer, ProjectileID.ManaCloakStar, 200, 1f);
            }
        }
    }
}
