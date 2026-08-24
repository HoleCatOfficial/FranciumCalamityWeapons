using BreadLibrary.Core.Graphics.Particles;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using FranciumCalamityWeapons.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using Terraria;
using Terraria.Graphics.Renderers;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Particles.Orchestrated
{
    public class DeuxiemeParticle : BaseParticle<DeuxiemeParticle>
    {
        public Vector2 position;

        public bool Spawned = false;

        public void Initiate(Vector2 Position)
        {
            this.position = Position;
        }

        public override void Update(ref ParticleRendererSettings settings)
        {
            if (!Spawned)
            {

                StarParticle Star = new();
                Star.Initialize(position, Vector2.Zero, Color.White, 1f);
                ParticleEngine.Particles.Add(Star);


                Spark Up = new Spark();
                Spark Down = new Spark();
                Spark Left = new Spark();
                Spark Right = new Spark();

                Spark UpLeft = new Spark();
                Spark UpRight = new Spark();
                Spark DownLeft = new Spark();
                Spark DownRight = new Spark();

                Up.PrepareSpark(position, new Vector2(0, -1), 0f, DTUtilsCalamity.DeuxiemeColor, 1f, false, 40, SparkDrawMode.Additive);
                Down.PrepareSpark(position, new Vector2(0, 1), 0f, DTUtilsCalamity.DeuxiemeColor, 1f, false, 40, SparkDrawMode.Additive);

                Left.PrepareSpark(position, new Vector2(-0.5f, 0), 0f, DTUtilsCalamity.DeuxiemeColor, 0.75f, false, 40, SparkDrawMode.Additive);
                Right.PrepareSpark(position, new Vector2(0.5f, 0), 0f, DTUtilsCalamity.DeuxiemeColor, 0.75f, false, 40, SparkDrawMode.Additive);

                UpLeft.PrepareSpark(position, new Vector2(-0.3f, -0.3f), 0f, DTColorUtils.Pastel(DTUtilsCalamity.DeuxiemeColor, 0.9f), 0.5f, false, 40, SparkDrawMode.Additive);
                UpRight.PrepareSpark(position, new Vector2(0.3f, -0.3f), 0f, DTColorUtils.Pastel(DTUtilsCalamity.DeuxiemeColor, 0.9f), 0.5f, false, 40, SparkDrawMode.Additive);
                DownLeft.PrepareSpark(position, new Vector2(-0.3f, 0.3f), 0f, DTColorUtils.Pastel(DTUtilsCalamity.DeuxiemeColor, 0.9f), 0.5f, false, 40, SparkDrawMode.Additive);
                DownRight.PrepareSpark(position, new Vector2(0.3f, 0.3f), 0f, DTColorUtils.Pastel(DTUtilsCalamity.DeuxiemeColor, 0.9f), 0.5f, false, 40, SparkDrawMode.Additive);

                ParticleEngine.BehindProjectiles.Add(Up);
                ParticleEngine.BehindProjectiles.Add(Down);
                ParticleEngine.BehindProjectiles.Add(Left);
                ParticleEngine.BehindProjectiles.Add(Right);
                ParticleEngine.BehindProjectiles.Add(UpLeft);
                ParticleEngine.BehindProjectiles.Add(UpRight);
                ParticleEngine.BehindProjectiles.Add(DownLeft);
                ParticleEngine.BehindProjectiles.Add(DownRight);
                Spawned = true;
            }
            else
            {
                ShouldBeRemovedFromRenderer = true;
            }
        }
    }
}