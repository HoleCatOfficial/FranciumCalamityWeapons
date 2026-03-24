using CalamityMod;
using DestroyerTest.Content.MeleeWeapons;
using DestroyerTest.Content.MeleeWeapons.Flails;
using DestroyerTest.Content.OrionCrossover;
using DestroyerTest.Content.RiftArsenal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Common.Items
{
    public class TrueMeleeGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;

        public static List<int> TalidTrueMelee = new List<int>
        {
            ModContent.ItemType<Quixotism>(),
            ModContent.ItemType<Icemourne>(),
            ModContent.ItemType<ScarletDragon>(),
            ModContent.ItemType<FrigidHalberd>(),
            ModContent.ItemType<SpiritOfJustice>(),
            ModContent.ItemType<Memoriam>(),
            ModContent.ItemType<CoronaBreaker>(),
            ModContent.ItemType<Gargantua>(),
            ModContent.ItemType<Colossus>(),
            ModContent.ItemType<SparkFrostCleaver>(),
            ModContent.ItemType<Malevolence>(),
            ModContent.ItemType<Scorn>(),
            ModContent.ItemType<HeliciteShank>(),
            ModContent.ItemType<RiftHypersabre>(),
            ModContent.ItemType<BlackDiamond>(),
            ModContent.ItemType<SunSaber>(),
            ModContent.ItemType<Sabhati>(),
            ModContent.ItemType<RiftYoyoT1>(),
            ModContent.ItemType<RiftYoyoT2>(),
            ModContent.ItemType<RiftYoyoT3>(),
            ModContent.ItemType<RiftClaymore>(),
        };

        public override void SetDefaults(Item entity)
        {
            if (TalidTrueMelee.Contains(entity.type))
            {
                entity.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            }
        }
    }
}
