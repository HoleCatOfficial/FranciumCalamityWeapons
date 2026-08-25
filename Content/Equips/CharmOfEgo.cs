using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod.NPCs.DevourerofGods;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs.Imbues;
using DestroyerTest.Content.Equips.ScepterAccessories;
using DestroyerTest.Rarity.Scepter;
using FranciumCalamityWeapons.Content.Melee;
using FranciumCalamityWeapons.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Equips
{
    public class CharmOfEgo : ModItem
    {
        public override void SetStaticDefaults()
        {
           
        }
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 52;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ModContent.RarityType<IncarnadineRarity>();
            Item.accessory = true;
            Item.master = true;
            Item.masterOnly = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {

            player.GetDamage(ModContent.GetInstance<ScepterClass>()) *= 1.95f;
            player.GetDamage(DamageClass.Throwing) *= 2.1f;

            player.GetArmorPenetration<ScepterClass>() += 5f;

            player.GetModPlayer<CharmOfEgoPlayer>().Active = true;
        }
    }

    public class CharmOfEgoPlayer : ModPlayer
    {
        public bool Active = false;

        public override void ResetEffects()
        {
            Active = false;
        }

        public override void PostUpdateMiscEffects()
        {
            if (Active)
            {

            }
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Active)
            {
                if (proj.CountsAsClass(DamageClass.Throwing) || proj.CountsAsClass<ScepterClass>())
                {
                    if (hit.Crit && Main.rand.NextBool(3))
                    {
                        Projectile.NewProjectile(proj.GetSource_OnHit(target), target.Center, Vector2.Zero, ModContent.ProjectileType<CharmOfEgoExplosion>(), proj.damage / 2, 10, Player.whoAmI);
                    }
                }
            }
        }
    }


    public class CoE_DROP_NPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == ModContent.NPCType<DevourerofGodsHead>())
            {
                npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<CharmOfEgo>()));
            }
        }
    }
}
