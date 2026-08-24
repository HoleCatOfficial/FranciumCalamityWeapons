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
using CalamityMod.Items.Armor.Silva;
using CalamityMod.Items.Placeables;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod;
using FranciumCalamityWeapons.Content.Buffs;
using CalamityMod.Items.Placeables.Abyss;
using FranciumCalamityWeapons.Content.Equips.NecroplasmSet;
using DestroyerTest.Content.Equips;
using CalamityMod.Items.Accessories;
using DestroyerTest.Rarity.Scepter;
using Terraria.GameContent;
using BreadLibrary.Core.Graphics.Particles;
using OpusLib.Content.Particles;

namespace FranciumCalamityWeapons.Content.Equips.NecroplasmSet
{

	[AutoloadEquip(EquipType.Head)]
	public class NecrokidHead : ModItem
	{
		public override void SetStaticDefaults()
		{
			ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
            CustomItemSets.DevItem[Type] = true;
		}

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 26;
			Item.value = Item.sellPrice(gold: 1);
			Item.rare = ModContent.RarityType<IncarnadineRarity>();
			Item.defense = 24;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs)
		{
			return body.type == ModContent.ItemType<NecrokidPlates>() && legs.type == ModContent.ItemType<NecrokidBoots>();
		}

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(ModContent.GetInstance<ScepterClass>()) += 24f;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = Language.GetTextValue("Mods.FranciumCalamityWeapons.Items.NecrokidHead.SetBonus");
            player.ScepterClass().ThrowSpeedModifier += 4f;
            player.Calamity().dodgeScarf = true;
            if (player.TryGetModPlayer<NecrokidPlayer>(out var Necro))
            {
                Necro.Active = true;
            }
        }

		public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

		public override void AddRecipes()
		{
            CreateRecipe()
				.AddIngredient<Necroplasm>(20)
                .AddIngredient<CounterScarf>()
                .AddIngredient<ReaperTooth>(1)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
	}

    public class NecrokidPlayer : ModPlayer
    {
        public bool Active = false;
        public int Cooldown = 0;

        public int CurrentCharge = 0;
        public const int MaxCharge = 100;
        public int ChargeDecayTimer = 120;
        public bool Sound1 = false;

        public override void ResetEffects()
        {
            Active = false;
        }


        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (Active)
            {
                DTUtils.DrawChargeBar(1.6f, (drawInfo.drawPlayer.Center + new Vector2(0, 40)) - Main.screenPosition, (float)CurrentCharge / (float)MaxCharge, new Color(254, 80, 128));
                Utils.DrawBorderString(Main.spriteBatch, $"{CurrentCharge.ToString()} / {MaxCharge}", (drawInfo.drawPlayer.Center + new Vector2(0, 55)) - Main.screenPosition, new Color(254, 80, 128), 0.75f, 0.5f, 0.5f);
            }
        }

        public override void PostUpdateEquips()
        {
            if (Active)
            {
                if(CurrentCharge >= MaxCharge)
                {
                    if (!Sound1)
                    {
                        SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/NecrokidCharge") with { PitchVariance = 0.5f }, Player.Center);
                        Opus.RingSpreadDust(DustID.FireworksRGB, 12, Player.Center, 20, 0, new Color(254, 80, 128), 1f, 3, offset: Main.rand.NextFloat(MathHelper.TwoPi));
                        Sound1 = true;
                    }
                }

                if (ChargeDecayTimer > 0)
                {
                    ChargeDecayTimer--;
                }

                if (ChargeDecayTimer <= 0 && CurrentCharge > 0)
                {
                    CurrentCharge--;
                    ChargeDecayTimer = 120;
                }


                if (Cooldown > 0)
                {
                    Cooldown--;
                }

                if (Cooldown == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/NecrokidCooldown") with { PitchVariance = 0.5f }, Player.Center);
                }

                if (DestroyerTestMod.ArmorSetBonusHotKey.JustPressed && Cooldown <= 0 && CurrentCharge >= MaxCharge)
                {
                    SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/NecroBoom") with { PitchVariance = 0.5f }, Player.Center);
                    Burst();
                    Player.AddBuff(ModContent.BuffType<NecroBoost>(), 600);
                    CurrentCharge = 0;
                    Cooldown = 1200;
                    Sound1 = false;
                }
            }
        }

        public void Burst()
        {
            for (int i = 0; i < 22; i++)
            {
                PointGlowPreMultiplied Part = new();
                Part.Initialize(Player.MountedCenter, Main.rand.NextVector2Circular(4, 4), new Color(254, 80, 128), 2f);
                ParticleEngine.ShaderParticles.Add(Part);
            }

            BloomRingSharp Ring = new();
            Ring.Prepare(Player.MountedCenter, Vector2.Zero, new Color(254, 80, 128), 0.3f, 0.02f, 3f, BlendState.Additive);
            ParticleEngine.ShaderParticles.Add(Ring);
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            CurrentCharge = 0;
            Cooldown = 1200;
            Sound1 = false;
        }
    }

    internal class NecrokidOwnedProjectiles : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[projectile.owner];
            if (player.TryGetModPlayer<NecrokidPlayer>(out var Necro))
            {
                if (projectile.DamageType == ModContent.GetInstance<ScepterClass>() && Necro.Active)
                {
                    if (Necro.CurrentCharge < NecrokidPlayer.MaxCharge && Necro.Cooldown <= 0)
                    {
                        Necro.CurrentCharge++;
                        Necro.ChargeDecayTimer = 180; 
                    }
                }
            }
        }
    }
}