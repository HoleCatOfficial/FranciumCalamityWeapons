
using System;
using System.Collections.Generic;
using System.Linq;
using BreadLibrary.Core.Graphics.Particles;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Tiles.Furniture.CraftingStations;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Equips;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.RangedItems;
using DestroyerTest.Content.Resources;
using DestroyerTest.Content.RiftArsenal;
using DestroyerTest.Content.Tiles;
using DestroyerTest.Content.Tiles.RiftConfigurator;
using DestroyerTest.Rarity;
using FranciumCalamityWeapons.Common;
using FranciumCalamityWeapons.Common.Rarities;
using FranciumCalamityWeapons.Content.Melee;
using FranciumCalamityWeapons.Content.Particles;
using FranciumCalamityWeapons.Content.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Equips
{
    public class UnfathomableConglomeration : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemNoGravity[Item.type] = true;
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 7));
        }
        public override void SetDefaults()
        {
            Item.width = 72;
            Item.height = 104;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<NewCosmicRarity>();
            Item.value = Item.sellPrice(platinum: 6);
            Item.expert = true;
            Item.expertOnly = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.rBrain = true;
            modPlayer.amalgam = true;
            player.brainOfConfusionItem = Item;
            modPlayer.HeatDebuffMultiplier += 2.25f;
            modPlayer.ColdDebuffMultiplier += 2.25f;
            modPlayer.SicknessDebuffMultiplier += 2.25f;
            modPlayer.WaterDebuffMultiplier += 2.25f;
            modPlayer.ElectricDebuffMultiplier += 2.25f;


            player.buffImmune[ModContent.BuffType<ShimmeringFlames>()] = true;
            player.buffImmune[ModContent.BuffType<GodSlayerInferno>()] = true;
            player.buffImmune[ModContent.BuffType<Defilement>()] = true;
            player.GetDamage(DamageClass.Generic) += 0.26f;
            player.GetArmorPenetration(DamageClass.Generic) += 37;
            player.endurance += 0.21f;

            float prog = Opus.Sine(0f, 1f, 0.05f);
            Lighting.AddLight(player.Center, OpusColorUtils.MultiLerp(prog, DTUtilsCalamity.UC_Colormap).ToVector3() * 0.1f);

            player.GetModPlayer<UnfathomableConglomerationPlayer>().Active = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient<LivingShard>(2)
            .AddIngredient<RuinousSoul>(2)
            .AddIngredient<TheAmalgam>()
            .AddIngredient<ShadeHeart>()
            .AddTile<CosmicAnvil>()
            .Register();
        }
    }

    public class UnfathomableConglomerationPlayer : ModPlayer
    {
        public bool Active = false;
        public float TexRot = 0f;
        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateEquips()
        {
            if (Active)
            {
                TexRot += 0.05f * Player.direction;

                LerpingSpark Spark = new LerpingSpark();

                Spark.PrepareSpark(Main.rand.NextVector2FromRectangle(Player.Hitbox), new Vector2(0f, -5f).RotatedByRandom(0.05f), 0f, DTUtilsCalamity.UC_Colormap, 0.6f, false, 30, SparkDrawMode.Additive, 2f);
                ParticleEngine.ShaderParticles.Add(Spark);
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (Active)
            {
                LerpingSimpleExplosionParticle ExplosionFX = new LerpingSimpleExplosionParticle();
                ExplosionFX.Prepare(Player.Center, Vector2.Zero, DTUtilsCalamity.UC_Colormap, 0.1f, 0.02f, 2f);
                ParticleEngine.ShaderParticles.Add(ExplosionFX);

                LerpingBloomRingSharp Ring = new LerpingBloomRingSharp();
                Ring.Prepare(Player.Center, Vector2.Zero, DTUtilsCalamity.UC_Colormap, 0.03f, 0.007f, 0.6f);
                ParticleEngine.ShaderParticles.Add(Ring);

                Opus.RadialSpreadProjectile(ModContent.ProjectileType<RottenStar>(), 5, Player.Center, (int)Player.GetTotalDamage(DamageClass.Generic).ApplyTo(25), 4, 6, offset: Main.rand.NextFloat(MathHelper.TwoPi));
            }
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (Active)
            {
                LerpingSimpleExplosionParticle ExplosionFX = new LerpingSimpleExplosionParticle();
                ExplosionFX.Prepare(Player.Center, Vector2.Zero, DTUtilsCalamity.UC_Colormap, 0.1f, 0.02f, 2f);
                ParticleEngine.ShaderParticles.Add(ExplosionFX);

                LerpingBloomRingSharp Ring = new LerpingBloomRingSharp();
                Ring.Prepare(Player.Center, Vector2.Zero, DTUtilsCalamity.UC_Colormap, 0.03f, 0.007f, 0.6f);
                ParticleEngine.ShaderParticles.Add(Ring);

                Opus.RadialSpreadProjectile(ModContent.ProjectileType<RottenStar>(), 5, Player.Center, (int)Player.GetTotalDamage(DamageClass.Generic).ApplyTo(25), 4, 6, offset: Main.rand.NextFloat(MathHelper.TwoPi));
            }
        }


        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.friendly)
            {
                return;
            }

            if (item.DamageType == DamageClass.Summon && Main.rand.NextBool(10) && Active)
            {
                target.AddBuff(ModContent.BuffType<DemonicFlames>(), 120);
            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.friendly)
            {
                return;
            }

            if (proj.DamageType == DamageClass.Summon && Main.rand.NextBool(10) && Active)
            {
                target.AddBuff(ModContent.BuffType<DemonicFlames>(), 120);
            }

            if (proj.DamageType == DamageClass.Summon && Main.rand.NextBool((int)(20 * (1 + (0.1f * Player.numMinions)))) && proj.type != ProjectileID.StardustGuardian && Active)
            {
                Opus.RadialSpreadProjectile(ModContent.ProjectileType<RottenStar>(), 6, target.Center, (int)Player.GetTotalDamage(DamageClass.Summon).ApplyTo(50), 12, 6);
            }
        }

        public override void NaturalLifeRegen(ref float regen)
        {
            if (Active)
            {
                regen *= 1.4f;
            }
        }
    }
}
