using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using FranciumCalamityWeapons.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Particles.Orchestrated
{
    public class DeuxiemeParticle : BasePRT
    {
        public override void SetProperty()
        {
            Lifetime = 60;
            ShouldKillWhenOffScreen = false;
        }

        public bool Spawned = false;
        public override void AI()
        {
            if (!Spawned)
            {
                Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), Position, Vector2.Zero, DTUtilsCalamity.DeuxiemeColor * 0.5f, 0.001f, 0.8f);
                Opus.NewParticleFloatAI(PRTLoader.GetParticleID<BloomRingSharp>(), Position, Vector2.Zero, DTUtilsCalamity.DeuxiemeColor * 0.8f, 0.001f, 0.3f);
                PRTLoader.NewParticle(PRTLoader.GetParticleID<StarParticle>(), Position, Vector2.Zero, Color.White, 1f);

                for (int i = 0; i < 4; i++)
                {
                    PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), Position, new Vector2(0, -5).RotatedByRandom(0.1f), DTColorUtils.Pastel(DTUtilsCalamity.DeuxiemeColor, 0.3f), 1f);
                    PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), Position, new Vector2(0, 5).RotatedByRandom(0.1f), DTColorUtils.Pastel(DTUtilsCalamity.DeuxiemeColor, 0.3f), 1f);
                }
               

                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), Position, new Vector2(0.5f, 0), DTUtilsCalamity.DeuxiemeColor, 0.5f);
                PRTLoader.NewParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), Position, new Vector2(-0.5f, 0), DTUtilsCalamity.DeuxiemeColor, 0.5f);
                
                Opus.RadialSpreadParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), 6, Position, 1, DTColorUtils.Pastel(DTUtilsCalamity.DeuxiemeColor, 0.6f), 0.2f, 0.5f, offset: 0f);
                Opus.RadialSpreadParticle(PRTLoader.GetParticleID<SparkParticleNoGravity>(), 6, Position, 1, DTColorUtils.Pastel(DTUtilsCalamity.DeuxiemeColor, 0.9f), 0.2f, 1f, offset: 0.5f);
                Spawned = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {
            return false;
        }
    }
}