using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    public class SeaPrismChain_Shaft : ModProjectile
    {
        private const int MaxChainMembers = 12; // Maximum number of chain members
        private bool initialized = false; // Tracks if the projectile has been initialized

        private int spawnTimer = 0;

        public override void SetStaticDefaults()
        {
            // Add any static defaults here if needed
        }

        public override void SetDefaults()
        {
            Projectile.width = 26; // Width of the projectile hitbox
            Projectile.height = 32; // Height of the projectile hitbox
            Projectile.aiStyle = -1; // Custom AI
            Projectile.friendly = true; // Can damage enemies
            Projectile.hostile = false; // Does not damage players
            Projectile.penetrate = -1; // Infinite penetration
            Projectile.timeLeft = 140; // Lifetime of the projectile in ticks
            Projectile.ignoreWater = true; // Ignores water physics
            Projectile.tileCollide = false; // Does not collide with tiles
            Projectile.DamageType = DamageClass.Magic; // Set the damage type
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // Store and use the initial spawn position only on the root segment
            if (Projectile.ai[0] == 0)
            {
                if (Projectile.localAI[1] == 0 && Projectile.localAI[2] == 0)
                {
                    Projectile.localAI[1] = Projectile.Center.X;
                    Projectile.localAI[2] = Projectile.Center.Y;
                }

                spawnTimer++;

                if (spawnTimer >= 4 && Projectile.localAI[0] < MaxChainMembers)
                {
                    Vector2 basePosition = new Vector2(Projectile.localAI[1], Projectile.localAI[2]);
                    Vector2 direction = Vector2.Normalize(Projectile.velocity);
                    float spacing = 26f;
                    Vector2 nextPosition = basePosition + direction * spacing * (Projectile.localAI[0] + 1);

                    int newProj = Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        nextPosition,
                        Projectile.velocity,
                        ModContent.ProjectileType<SeaPrismChain_Shaft>(),
                        Projectile.damage,
                        Projectile.knockBack,
                        Projectile.owner,
                        Projectile.localAI[0] + 1 // ai[0] for the next segment
                    );

                    Main.projectile[newProj].localAI[1] = Projectile.localAI[1]; // Pass base X
                    Main.projectile[newProj].localAI[2] = Projectile.localAI[2]; // Pass base Y

                    spawnTimer = 0;
                    Projectile.localAI[0]++;
                }

                // Spawn the head once the last segment is reached
                if (Projectile.localAI[0] == MaxChainMembers)
                {
                    Vector2 basePosition = new Vector2(Projectile.localAI[1], Projectile.localAI[2]);
                    Vector2 direction = Vector2.Normalize(Projectile.velocity);
                    float spacing = 26f;
                    Vector2 nextPosition = basePosition + direction * spacing * (Projectile.localAI[0] + 1);

                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        nextPosition,
                        Projectile.velocity,
                        ModContent.ProjectileType<SeaPrismChain_Head>(),
                        Projectile.damage,
                        Projectile.knockBack,
                        Projectile.owner
                    );

                    Projectile.localAI[0]++; // Prevent repeated head spawning
                }
            }

            // Fade out the chain when timeLeft falls below 120
            if (Projectile.timeLeft < 120)
            {
                Projectile.alpha += 2;
                if (Projectile.alpha > 255)
                    Projectile.alpha = 255;
            }

            // Emit soft light
            Lighting.AddLight(Projectile.Center, 0.2f, 0.2f, 0.2f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 120);
        }

    }
}