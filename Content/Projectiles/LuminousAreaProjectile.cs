using System;
using FranciumCalamityWeapons.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles
{

    public class LuminousAreaParticle : ModProjectile
    {
        public override string Texture => "FranciumCalamityWeapons/Content/Particles/ParticleDrawEntity";

        private int auraTimer = 600;
        private float radius = 0f;

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3600;
            Projectile.tileCollide = false;
            Projectile.scale = 0.1f;
            Projectile.hide = true; // Optional: hide if just visual
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.owner = Main.LocalPlayer.whoAmI;
        }

        public override void AI()
        {
            auraTimer--;

            // Interpolate radius from 0 to 300 over the first 30 ticks (auraTimer 600 -> 570)
            if (auraTimer > 570)
            {
                float t = 1f - (auraTimer - 570) / 30f; // t goes from 0 to 1 as auraTimer goes from 600 to 570
                radius = MathHelper.SmoothStep(0f, 200f, MathHelper.Clamp(t, 0f, 1f));
            }
            else
            {
                radius = 200f;
            }

            // Create the dust ring
            int dustAmount = 5; // Number of dust particles in the ring
            for (int i = 0; i < dustAmount; i++)
            {
                float angle = MathHelper.TwoPi * i / dustAmount;
                // Offset the angle each tick so the dust rotates around the circle over time
                float timeOffset = Main.GameUpdateCount * 0.03f; // Adjust speed as needed
                float dynamicAngle = angle + timeOffset;
                Vector2 dustPos = Projectile.Center + radius * new Vector2((float)Math.Cos(dynamicAngle), (float)Math.Sin(dynamicAngle));
                PRTLoader.NewParticle(PRTLoader.GetParticleID<LuminousStar>(), dustPos, Vector2.Zero, default, 1);
            }
            if (auraTimer <= 30)
            {
                float t = (float)auraTimer / 30f; // t goes from 1 to 0 as auraTimer goes from 30 to 0
                radius = MathHelper.SmoothStep(0f, 200f, t); // Smoothly shrink from 300 to 0
            }



            // Apply buffs to players inside the radius
            foreach (NPC target in Main.npc)
            {
                if (target.active && !target.friendly && Vector2.Distance(target.Center, Projectile.Center) <= radius)
                {
                    int hit = Main.rand.Next(20, 40); // Set the damage amount as needed
                    NPC.HitInfo hitInfo = new NPC.HitInfo()
                    {
                        Damage = hit,
                        Knockback = 0f,
                        HitDirection = 0,
                        Crit = false
                    };
                    target.StrikeNPC(hitInfo, false, false);
                }
            }

            

            PRTLoader.NewParticle(PRTLoader.GetParticleID<BubbleParticle>(), Projectile.Center, new Vector2(0, 0.07f), default, 1);

            Vector2 RandPos = Main.rand.NextVector2Circular(radius, radius) + Projectile.Center;

            PRTLoader.NewParticle(PRTLoader.GetParticleID<BubbleParticle>(), RandPos, new Vector2(0, 0.07f), default, 0.06f);

            if (auraTimer <= 0)
            {
                Projectile.Kill();
            }
        }
    }
}