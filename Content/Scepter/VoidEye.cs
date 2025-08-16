using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.Audio;
using DestroyerTest.Content.Consumables;
using DestroyerTest.Common;
using DestroyerTest.Content.Buffs;
using DestroyerTest.Content.Magic;
using InnoVault.PRT;
using DestroyerTest.Content.Particles;
using System.Text;
using FranciumCalamityWeapons.Content.Particles;

namespace FranciumCalamityWeapons.Content.Scepter
{
    public class VoidEye : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            
        }

        public override void SetDefaults()
        {
            Projectile.width = 148;
            Projectile.height = 74;
            Projectile.friendly = true;
            Projectile.penetrate = -1; // Infinite pierce
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180; // 10 seconds max lifespan
            Projectile.DamageType = ModContent.GetInstance<ScepterClass>();
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }

        
        SoundStyle Puppet = new SoundStyle("FranciumCalamityWeapons/Audio/VoidScepterPuppet") with { Volume = 0.75f, PitchVariance = 0.8f };
        public Vector2 CurrentCenter;

        public float ProjSpawnTimer = 0;

        public override void AI()
        {
            ProjSpawnTimer++;
            Player player = Main.player[Projectile.owner];
            if (!player.channel || player.dead || player.CCed)
            {
                Projectile.Kill(); // Stop when player stops channeling, dies, or is crowd controlled
                return;
            }

            CurrentCenter = Projectile.Center;




            if (player.HeldItem.type == ModContent.ItemType<ExarchicEnforcer>() && player.channel)
            {

                Projectile.timeLeft = 120;
                Projectile.Center = Main.MouseWorld;

                float rad = 1000;
                Vector2 Spawn = Projectile.Center + Main.rand.NextVector2CircularEdge(rad, rad);
                Vector2 toOrigin = CurrentCenter - Spawn;
                toOrigin = toOrigin.SafeNormalize(Vector2.UnitY); // fallback to downwards if zero

                if (ProjSpawnTimer >= 30)
                {
                    SoundEngine.PlaySound(Puppet, Projectile.Center);

                    for (int a = 0; a < 3; a++)
                    {
                        Spawn = Projectile.Center + Main.rand.NextVector2CircularEdge(rad, rad);
                        toOrigin = Projectile.Center + Spawn;
                        toOrigin = toOrigin.SafeNormalize(Vector2.UnitY);
                        Projectile.NewProjectile(Entity.GetSource_FromThis(), Projectile.Center, toOrigin * 70f, ModContent.ProjectileType<MiniCruiserHead>(), Projectile.damage / 3, 2, Main.LocalPlayer.whoAmI);
                    }
                    PRTLoader.NewParticle(PRTLoader.GetParticleID<BloomRingSharp>(), Projectile.Center, Projectile.velocity, ExarchicEnforcerThrown.VoidBarColors.Colors[3], 0.4f);
                    ProjSpawnTimer = 0;
                }
            }
            
            foreach(Projectile parent in Main.projectile)
            {
                if (!parent.active && parent.type == ModContent.ProjectileType<ExarchicEnforcerHoldout>())
                {
                    Projectile.Kill();
                    break;
                }
            }
        }
    }
}

