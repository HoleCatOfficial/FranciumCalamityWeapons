using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using System;
using DestroyerTest.Common;
using DestroyerTest.Content.Projectiles;
using DestroyerTest.Content.Projectiles.ParentClasses;
using CalamityMod.Buffs.StatDebuffs;

namespace FranciumCalamityWeapons.Content.Scepter
{
    public class LunarVoltageScepterThrown : ThrownScepter
    {
        private NPC HomingTarget
        {
            get => Projectile.ai[0] == 0 ? null : Main.npc[(int)Projectile.ai[0] - 1];
            set
            {
                Projectile.ai[0] = value == null ? 0 : value.whoAmI + 1;
            }
        }
        public override void SetDefaults()
        {
            ThemeColor = new Color(94, 229, 163);
            WidthDim = 78;
            HeightDim = 78;
            DustType = DustID.Vortex;
            base.SetDefaults();
        }

        public override void PostAI()
        {
            base.PostAI();
            Projectile.ai[1]++;

            foreach(NPC hostile in Main.npc)
            {
                if (hostile.Distance(Projectile.Center) < 200)
                {
                    hostile.AddBuff(ModContent.BuffType<GalvanicCorrosion>(), 600);
                }
            }

            if (HomingTarget == null)
            {
                HomingTarget = FindClosestNPC(200);
            }

            if (HomingTarget != null && !IsValidTarget(HomingTarget))
            {
                HomingTarget = null;
            }

            if (HomingTarget != null && IsValidTarget(HomingTarget))
            {
                Vector2 dir = HomingTarget.Center - Projectile.Center;
                dir = dir.ToRotation().ToRotationVector2() * 30;

                if (Projectile.ai[1] % 10 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item176 with { MaxInstances = 0 }, Projectile.Center);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, dir, ProjectileID.LunarFlare, Projectile.damage / 4, 10, Projectile.owner);
                }
            }
        }

        public NPC FindClosestNPC(float maxDetectDistance)
        {
            NPC closestNPC = null;

            float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

            foreach (var target in Main.ActiveNPCs)
            {
                if (IsValidTarget(target))
                {
                    float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);
                    if (sqrDistanceToTarget < sqrMaxDetectDistance)
                    {
                        sqrMaxDetectDistance = sqrDistanceToTarget;
                        closestNPC = target;
                    }
                }
            }

            return closestNPC;
        }

        public bool IsValidTarget(NPC target) {
            return target.CanBeChasedBy();
        }
    }
}

