using BreadLibrary.Core.Verlet;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Projectiles.Weapon.Summon;
using DestroyerTest.Content.SummonItems;
using FranciumCalamityWeapons.Content.Buffs;
using InnoVault.GameContent;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using OpusLib.Content.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Net.Mime.MediaTypeNames;

namespace FranciumCalamityWeapons.Content.Projectiles.NightGazer
{
    public class NightGazerProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public Line ToPlayer;
        public Line ToMouse;


        public Line ToPlayerInit;


        public override void SetDefaults()
        {

            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 2;
            Projectile.minion = true;
            Projectile.minionSlots = 2f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;



        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D PT = TextureAssets.Projectile[Type].Value;
            Texture2D Glow = ModContent.Request<Texture2D>($"{Texture}_Glow").Value;
            SpriteEffects FX = SpriteEffects.None;

            float rot = Projectile.rotation;

            if (rot > MathHelper.PiOver2 || rot < -MathHelper.PiOver2)
            {
                FX = SpriteEffects.FlipVertically;
            }
            else
            {
                FX = SpriteEffects.None;
            }

            RenderRope(Main.screenPosition, Color.White);

            Main.EntitySpriteDraw(PT, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, PT.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);

            Main.EntitySpriteDraw(Glow, Projectile.Center - Main.screenPosition, null, Opus.Sine(Color.White, Color.LightCyan), Projectile.rotation, Glow.Size() / 2, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        private void RenderRope(Vector2 screenPos, Color drawColor)
        {
            Texture2D[] tentacleTexture = new Texture2D[2]
            {
                ModContent.Request<Texture2D>("FranciumCalamityWeapons/Content/Projectiles/NightGazer/NightGazerTentacleL").Value,
                ModContent.Request<Texture2D>("FranciumCalamityWeapons/Content/Projectiles/NightGazer/NightGazerTentacleR").Value,
            };

            for (int t = 0; t < 4; t++)
            {
                var start = TentacleBegin[t];
                var end = TentaclePoint[t];

                Vector2 vinePos = (start + end) / 2f;
                Vector2 drawPos = vinePos - screenPos;

                // left = 0, right = 1
                int texIndex = (t < 2) ? 0 : 1;

                var texture = tentacleTexture[texIndex];
                var frame = texture.Frame();

                float rotation = start.AngleTo(end);

                float segmentDistance = start.Distance(end);
                float lengthFactor = segmentDistance / Math.Max(1, frame.Height - 5) * 1.2f;

                Vector2 stretch = new Vector2(1f, lengthFactor);
                Vector2 origin = frame.Size() * 0.5f;

                Main.EntitySpriteDraw(texture, drawPos, frame, drawColor, rotation - MathHelper.PiOver2, origin, stretch, SpriteEffects.None);
            }
        }

        public int IdealDistancefromPlayerMin = 140;
        public int IdealDistanceFromPlayerExact = 150;
        public int IdealDistancefromPlayerMax = 160;
        public int DistancefromPlayerToTeleport = 1200;

        public float CurrentDistance = 0f;
        public int Buff = ModContent.BuffType<NightGazerBuff>();

        public enum Condition
        {
            TeleportToPlayer,
            TooFarFromPlayer,
            TooCloseToPlayer,
            SweetSpot,
            Limbo
        }
        public Condition CurrentCondition;

        public static int OuterOffsetL = -15;
        public static int InnerOffsetL = -5;
        public static int OuterOffsetR = 15;
        public static int InnerOffsetR = 5;



        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            ToPlayer = new Line(Projectile.Center, player.Center);

            TentacleBegin[0] = AdjBottom + (new Vector2(OuterOffsetL, -6).RotatedBy(Projectile.rotation));
            TentacleBegin[1] = AdjBottom + (new Vector2(InnerOffsetL, -6).RotatedBy(Projectile.rotation));
            TentacleBegin[2] = AdjBottom + (new Vector2(InnerOffsetR, -6).RotatedBy(Projectile.rotation));
            TentacleBegin[3] = AdjBottom + (new Vector2(OuterOffsetR, -6).RotatedBy(Projectile.rotation));

            TentaclePoint[0] = AdjBottom + new Vector2(OuterOffsetL, 120);
            TentaclePoint[1] = AdjBottom + new Vector2(InnerOffsetL, 120);
            TentaclePoint[2] = AdjBottom + new Vector2(InnerOffsetR, 120);
            TentaclePoint[3] = AdjBottom + new Vector2(OuterOffsetR, 120);
        }

        public Vector2[] TentaclePoint = new Vector2[4];

        public Vector2[] TentacleBegin = new Vector2[4];

        private void UpdateTentaclePoints()
        {
            Vector2[] Idealp = new Vector2[4]
            {
                AdjBottom  + new Vector2(OuterOffsetL, 120).RotatedBy(Projectile.rotation),
                AdjBottom  + new Vector2(InnerOffsetL, 120).RotatedBy(Projectile.rotation),
                AdjBottom  + new Vector2(InnerOffsetR, 120).RotatedBy(Projectile.rotation),
                AdjBottom  + new Vector2(OuterOffsetR, 120).RotatedBy(Projectile.rotation),
            };

            for (int j = 0; j < 4; j++)
            {
                if (TentaclePoint[j].Distance(Idealp[j]) > 15)
                {
                    TentaclePoint[j] += (Idealp[j] - TentaclePoint[j]) * 0.1f;
                }
            }
        }

        public Vector2 AdjBottom;
        public bool F1 = false;
        public override void AI()
        {
            AdjBottom = Projectile.Center + new Vector2(0, Projectile.height / 2).RotatedBy(Projectile.rotation);
            Player player = Main.player[Projectile.owner];
            ToPlayer = new Line(Projectile.Center, player.Center);
            ToMouse = new Line(Projectile.Center, Main.MouseWorld);



            TentacleBegin[0] = AdjBottom + (new Vector2(OuterOffsetL, -6).RotatedBy(Projectile.rotation));
            TentacleBegin[1] = AdjBottom + (new Vector2(InnerOffsetL, -6).RotatedBy(Projectile.rotation));
            TentacleBegin[2] = AdjBottom + (new Vector2(InnerOffsetR, -6).RotatedBy(Projectile.rotation));
            TentacleBegin[3] = AdjBottom + (new Vector2(OuterOffsetR, -6).RotatedBy(Projectile.rotation));


            Lighting.AddLight(Projectile.Center, Opus.Sine(Color.White, Color.LightCyan).ToVector3());

            UpdateTentaclePoints();

            Projectile.ai[1]++;

            //CycleLine(ToPlayer);

            if (!CheckActive(player))
            {
                return;
            }

            CurrentDistance = Projectile.Center.Distance(player.Center);

            if (CurrentDistance > DistancefromPlayerToTeleport)
            {
                CurrentCondition = Condition.TeleportToPlayer;
            }
            else if (CurrentDistance < DistancefromPlayerToTeleport && CurrentDistance > IdealDistancefromPlayerMax)
            {
                CurrentCondition = Condition.TooFarFromPlayer;
            }
            else if (CurrentDistance < IdealDistancefromPlayerMax && CurrentDistance > IdealDistancefromPlayerMin)
            {
                CurrentCondition = Condition.SweetSpot;
            }
            else if (CurrentDistance < IdealDistancefromPlayerMin)
            {
                CurrentCondition = Condition.TooCloseToPlayer;
            }
            else
            {
                CurrentCondition = Condition.Limbo;
            }

            switch (CurrentCondition)
            {
                case Condition.TeleportToPlayer:
                    {
                        Projectile.Center = player.Center;
                        break;
                    }
                case Condition.TooFarFromPlayer:
                    {
                        if (!F1)
                        {
                            if (CurrentDistance > 165)
                            {
                                SoundEngine.PlaySound(SoundID.DD2_LightningBugHurt with { Pitch = -1, PitchVariance = 0.2f, MaxInstances = 0 }, Projectile.Center);
                            }
                            F1 = true;
                        }
                        Vector2 targ1 = player.Center + new Vector2(IdealDistanceFromPlayerExact, 0).RotatedBy(ToPlayer.GetLineRotation + MathHelper.Pi);
                        Vector2 toTarget = targ1 - Projectile.Center;
                        float dist = toTarget.Length();

                        float maxSpeed = 20f;
                        float slowRadius = 200f;

                        float desiredSpeed = maxSpeed * MathHelper.Clamp(dist / slowRadius, 0f, 1f);

                        Vector2 desiredVelocity = Vector2.Normalize(toTarget) * desiredSpeed;

                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.1f);
                        //Projectile.Center += Projectile.velocity;
                        break;
                    }
                case Condition.SweetSpot:
                    {
                        Projectile.velocity *= 0.995f;
                        Vector2 RandP = player.Center + new Vector2(IdealDistanceFromPlayerExact, 0);
                        if (Main.GameUpdateCount % 240 == 0)
                        {
                            RandP = RandP.RotatedByRandom(MathHelper.TwoPi);
                        }

                        Vector2 D = RandP - Projectile.Center;
                        D.Normalize();
                        Projectile.velocity += D;
                        F1 = false;

                        break;
                    }
                case Condition.TooCloseToPlayer:
                    {
                       
                        Vector2 targ1 = player.Center + new Vector2(IdealDistanceFromPlayerExact, 0).RotatedBy(ToPlayer.GetLineRotation + MathHelper.Pi);
                        Vector2 toTarget = targ1 - Projectile.Center;
                        float dist = toTarget.Length();

                        float maxSpeed = 20f;
                        float slowRadius = 200f;

                        float desiredSpeed = maxSpeed * MathHelper.Clamp(dist / slowRadius, 0f, 1f);

                        Vector2 desiredVelocity = Vector2.Normalize(toTarget) * desiredSpeed;

                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.1f);

                        break;
                    }
                case Condition.Limbo:
                    {
                        CurrentCondition = Condition.TeleportToPlayer;
                        break;
                    }
            }


            SearchForTargets(player, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);

            Projectile.rotation = 0.1f * Projectile.velocity.X;
            Spread();

            if (foundTarget)
            {


                Vector2 dir = targetCenter - Projectile.Center;
                //Vector2 dir = Main.MouseWorld - Projectile.Center;
                dir.Normalize();
                Vector2 Vel = dir * 10;

                if (Projectile.ai[1] % 85 == 0)
                {
                    SoundEngine.PlaySound(SoundID.DD2_LightningBugDeath with { Pitch = -1, PitchVariance = 0.2f, MaxInstances = 0 }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.Item77, Projectile.Center);
                    
                    //PRTLoader.NewParticle(PRTLoader.GetParticleID<SmallShine>(), Projectile.Center, Vel.RotatedByRandom(0.1f), Color.White, 0.25f);
                    
                    Projectile.velocity += dir * -4f;
                    for (int i = 0; i < 3; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vel.RotatedByRandom(0.5f), ModContent.ProjectileType<NightGazerBolt>(), Projectile.damage / 2, 8, player.whoAmI);
                    }
                }
            }
        }


        private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter)
        {
            // Starting search distance
            distanceFromTarget = 1200f;
            targetCenter = Projectile.position;
            foundTarget = false;

            // This code is required if your minion weapon has the targeting feature
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[owner.MinionAttackTargetNPC];
                float between = Vector2.Distance(npc.Center, Projectile.Center);

                // Reasonable distance away so it doesn't target across multiple screens
                if (between < 2000f)
                {
                    distanceFromTarget = between;
                    targetCenter = npc.Center;
                    foundTarget = true;
                }
            }

            if (!foundTarget)
            {
                // This code is required either way, used for finding a target
                foreach (var npc in Main.ActiveNPCs)
                {
                    if (npc.CanBeChasedBy())
                    {
                        float between = Vector2.Distance(npc.Center, Projectile.Center);
                        bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
                        bool inRange = between < distanceFromTarget;
                        bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
                        // Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
                        // The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
                        bool closeThroughWall = between < 100f;

                        if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall))
                        {
                            distanceFromTarget = between;
                            targetCenter = npc.Center;
                            foundTarget = true;
                        }
                    }
                }
            }

            Projectile.friendly = foundTarget;
        }

        private void Spread()
        {

            foreach (Projectile proj in Main.projectile)
            {
                if (proj == Projectile)
                    continue;

                if (proj.type == Type && proj.active)
                {
                    Vector2 Dir = proj.Center - Projectile.Center;
                    Dir.Normalize();
                    float TooClose = 20f * 20f;
                    if (Projectile.Center.DistanceSQ(proj.Center) < TooClose)
                    {
                        Projectile.velocity += Dir * -1f;
                    }
                    if (Projectile.Center == proj.Center)
                    {
                        Projectile.velocity += Main.rand.NextVector2Circular(5, 5);
                    }
                }
            }

        }


        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(Buff);

                return false;
            }

            if (owner.HasBuff(Buff))
            {
                Projectile.timeLeft = 2;
            }

            return true;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = true;
            return true;
        }

        private float scroll;

        private void CycleLine(Line line)
        {
            Player player = Main.player[Projectile.owner];

            int C = 3;

            if (player.ownedProjectileCounts[Type] > 4)
            {
                C = 2;
            }
            Vector2[] basePoints = line.GetPointsAlongLine(C);
            int len = basePoints.Length;

            scroll += 0.05f;

            int baseIndex = (int)scroll % len;
            float t = scroll % 1f;

            for (int i = 0; i < len; i++)
            {
                int a = (baseIndex + i) % len;
                int b = (a + 1) % len;

                Vector2 pos = Vector2.Lerp(basePoints[a], basePoints[b], t);

                Dust T = Dust.NewDustPerfect(pos, DustID.CursedTorch, Vector2.Zero, 0, default, 0.8f);
                T.noGravity = true;
            }
        }
    }
}
