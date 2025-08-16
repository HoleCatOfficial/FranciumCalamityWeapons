using System;
using CalamityMod;
using FranciumCalamityWeapons.Content.Particles;
using InnoVault.PRT;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    /*
    public class SupremacySwing : ModProjectile
    {


        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3600;
            Projectile.tileCollide = false;
            Projectile.hide = false; // Optional: hide if just visual
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.owner = Main.LocalPlayer.whoAmI;
            Projectile.extraUpdates = 3;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;

            Vector2 origin = new Vector2(0f, tex.Height); // bottom-left
            Vector2 anchor = Main.player[Projectile.owner].MountedCenter;
            float angle = Projectile.rotation;  // manually control this

            Main.EntitySpriteDraw(
                tex,
                anchor - Main.screenPosition,
                null,
                lightColor,
                angle,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0f
            );


            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Player Owner = Main.player[Projectile.owner];
            Vector2 start = Owner.MountedCenter;
            Vector2 end = start + Projectile.rotation.ToRotationVector2() * ((Projectile.Size.Length()) * Projectile.scale);
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 25f * Projectile.scale, ref collisionPoint);
        }
        
        public float rotSpeed = 0f;
        public bool Sound = false;
        public Vector2 swordTip;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = new Vector2(0f, tex.Height); // bottom-left
            Vector2 tipOffset = new Vector2(0f, -tex.Height) * Projectile.scale;
            Vector2 swordTip = Main.player[Projectile.owner].MountedCenter + tipOffset.RotatedBy(Projectile.rotation);

            int[] types = new int[]
            {
                PRTLoader.GetParticleID<BlackFire1>(),
                PRTLoader.GetParticleID<BlackFire2>(),
                PRTLoader.GetParticleID<BlackFire3>(),
                PRTLoader.GetParticleID<BlackFire4>(),
                PRTLoader.GetParticleID<BlackFire5>(),
                PRTLoader.GetParticleID<BlackFire6>(),
                PRTLoader.GetParticleID<BlackFire7>()
            };

            PRTLoader.NewParticle(types[Main.rand.Next(types.Length)], swordTip, Vector2.Zero, Color.Black, 1);

            int[] types2 = new int[]
            {
                PRTLoader.GetParticleID<ColoredFire1>(),
                PRTLoader.GetParticleID<ColoredFire2>(),
                PRTLoader.GetParticleID<ColoredFire3>(),
                PRTLoader.GetParticleID<ColoredFire4>(),
                PRTLoader.GetParticleID<ColoredFire5>(),
                PRTLoader.GetParticleID<ColoredFire6>(),
                PRTLoader.GetParticleID<ColoredFire7>()
            };

            PRTLoader.NewParticle(types2[Main.rand.Next(types2.Length)], swordTip, Vector2.Zero, new Color(255, 209, 0), 0.4f);

            if (Projectile.ai[0] == 0)
            {
                Projectile.rotation = -MathHelper.PiOver2 * player.direction; // Start vertical
                Projectile.direction = player.direction;
            }

            if (Sound == false)
            {
                SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/ThrowWoosh1") with { PitchVariance = 1.5f }, Projectile.Center);
                Sound = true;
            }

            Projectile.Center = player.MountedCenter;
            Projectile.rotation += rotSpeed;
            rotSpeed += 0.004f * player.direction; // tweak for speed
            Projectile.ai[0]++;


            if (Projectile.ai[0] > 72) // 360° = ~72 frames at 5° per frame
            {
                Sound = false;
                Projectile.Kill();
            }

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/WOTG_RiftOpen") with { PitchVariance = 0.5f }, Projectile.Center);
            target.velocity *= 0.25f;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SupremacyWyrmHead>()] < 1)
            {
                Projectile.NewProjectile(Entity.GetSource_FromThis(), Projectile.Center, (target.Center - Projectile.Center) * 0.1f, ModContent.ProjectileType<SupremacyWyrmHead>(), 200, 2, player.whoAmI);
            }
            PRTLoader.NewParticle(PRTLoader.GetParticleID<Boom2>(), target.Center, Vector2.Zero, new Color(255, 209, 0), 1);
        }
    }
    */

    public class SupremacySwing : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 36;
            Projectile.ArmorPenetration = 80;
            Projectile.timeLeft = 100000;
            Projectile.extraUpdates = 3;
        }

        public float scaleD = 0.64f;
        public float rotSpeed = 0f;
        public bool playsound1 = true;
        public bool playsound2 = true;

        public override void AI()
        {
            float updates = Projectile.MaxUpdates + 1;
            Player owner = Main.player[Projectile.owner];
            float meleeSpeed = owner.GetTotalAttackSpeed(Projectile.DamageType);

            if (Projectile.ai[0] == 0)
            {
                Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.rotation -= 2.42f * Projectile.direction;
            }

            if (Projectile.ai[0] >= 64 * updates && playsound1)
            {
                playsound1 = false;
                SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/ThrowWoosh1") with { PitchVariance = 1.5f }, Projectile.Center);
            }

            if (Projectile.ai[0] >= 74 * updates && playsound2)
            {
                playsound2 = false;
                SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/ThrowWoosh1") with { PitchVariance = 1.5f }, Projectile.Center);
            }

            int Radius = 200;
            Vector2 PRTPos = (Projectile.Center + new Vector2(Radius, 0)).RotatedBy(Projectile.rotation);

            int[] types = new int[]
            {
                PRTLoader.GetParticleID<BlackFire1>(),
                PRTLoader.GetParticleID<BlackFire2>(),
                PRTLoader.GetParticleID<BlackFire3>(),
                PRTLoader.GetParticleID<BlackFire4>(),
                PRTLoader.GetParticleID<BlackFire5>(),
                PRTLoader.GetParticleID<BlackFire6>(),
                PRTLoader.GetParticleID<BlackFire7>()
            };

            

            int[] types2 = new int[]
            {
                PRTLoader.GetParticleID<ColoredFire1>(),
                PRTLoader.GetParticleID<ColoredFire2>(),
                PRTLoader.GetParticleID<ColoredFire3>(),
                PRTLoader.GetParticleID<ColoredFire4>(),
                PRTLoader.GetParticleID<ColoredFire5>(),
                PRTLoader.GetParticleID<ColoredFire6>(),
                PRTLoader.GetParticleID<ColoredFire7>()
            };

            PRTLoader.NewParticle(types[Main.rand.Next(types.Length)], PRTPos, Vector2.Zero, Color.Black, 1);
            PRTLoader.NewParticle(types2[Main.rand.Next(types2.Length)], PRTPos, Vector2.Zero, new Color(255, 209, 0), 0.4f);


            Projectile.Center = owner.MountedCenter + owner.gfxOffY * Vector2.UnitY;
            Projectile.rotation += rotSpeed * meleeSpeed;

            if (Projectile.ai[0] < 60 * updates)
            {
                Projectile.ai[0] = 60 * updates;
                
            }
            else
            {
                if (Projectile.ai[0] < 86 * updates)
                {
                    rotSpeed += 0.00121f * Projectile.direction * meleeSpeed;
                }
                else
                {
                    rotSpeed *= (float)Math.Pow(0.94, 1.0 / meleeSpeed);
                    if (Projectile.ai[0] > 90 * updates && Projectile.owner == Main.myPlayer)
                    {
                        Projectile.direction = (Main.MouseWorld - owner.Center).X > 0 ? 1 : -1;
                        float targetrot = (Main.MouseWorld - owner.Center).ToRotation() - 2.42f * Projectile.direction;
                        Projectile.rotation = RotateTowardsAngle(Projectile.rotation, targetrot, 0.07f * meleeSpeed, false);
                    }
                    if (Projectile.ai[0] > 100 * updates)
                    {
                        owner.itemTime = 0;
                        owner.itemAnimation = 0;
                        Projectile.Kill();
                        return;
                    }
                }
            }

            Projectile.ai[0] += meleeSpeed;

            if (Projectile.velocity.X > 0)
            {
                owner.direction = 1;
                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)(Math.PI * 0.5f));
            }
            else
            {
                owner.direction = -1;
                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)(Math.PI * 0.5f));
            }

            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 2;
            owner.itemAnimation = 2;


        }

        public static float RotateTowardsAngle(float currentRadians, float targetRadians, float rotateSpeed, bool useFixedSpeed = true)
        {
            currentRadians = MathHelper.WrapAngle(currentRadians);
            targetRadians = MathHelper.WrapAngle(targetRadians);

            float difference = targetRadians - currentRadians;
            float turnAmount = MathHelper.WrapAngle(difference);

            if (useFixedSpeed)
            {
                turnAmount = MathHelper.Clamp(turnAmount, -rotateSpeed, rotateSpeed);
            }
            else
            {
                turnAmount *= MathHelper.Clamp(rotateSpeed, 0f, 1f);
            }

            return currentRadians + turnAmount;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D tex2 = ModContent.Request<Texture2D>("FranciumCalamityWeapons/Content/Extras/MotionTrail1").Value;
            Vector2 origin = new Vector2(0, tex.Height);
            Vector2 position = Main.player[Projectile.owner].MountedCenter - Main.screenPosition;
            float rotation = Projectile.rotation + MathF.PI * 0.25f;

            Main.EntitySpriteDraw(
                tex,
                position,
                null,
                Color.White,
                rotation,
                origin,
                Projectile.scale * 2.86f * scaleD,
                SpriteEffects.None,
                0);

            Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            Main.EntitySpriteDraw(
                tex2,
                position,
                null,
                new Color(255, 209, 0),
                rotation - MathHelper.PiOver4,
                origin,
                Projectile.scale * 3f * scaleD,
                SpriteEffects.None,
                0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 628 * Projectile.scale * scaleD, targetHitbox, 64);
        }

        public static bool LineThroughRect(Vector2 start, Vector2 end, Rectangle rect, int lineWidth = 4, int checkDistance = 8)
        {
            float point = 0f;
            return rect.Contains((int)start.X, (int)start.Y) || rect.Contains((int)end.X, (int)end.Y) || Collision.CheckAABBvLineCollision(rect.TopLeft(), rect.Size(), start, end, lineWidth, ref point);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/WOTG_RiftOpen") with { PitchVariance = 0.5f }, Projectile.Center);
            target.velocity *= 0.25f;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<SupremacyWyrmHead>()] < 1)
            {
                Projectile.NewProjectile(Entity.GetSource_FromThis(), Projectile.Center, (target.Center - Projectile.Center) * 0.1f, ModContent.ProjectileType<SupremacyWyrmHead>(), 200, 2, player.whoAmI);
            }
            PRTLoader.NewParticle(PRTLoader.GetParticleID<Boom2>(), target.Center, Vector2.Zero, new Color(255, 209, 0), 1);
        }
    }
}