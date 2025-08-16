using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Particles
{
    // Our first PRT particle, pretty cool right? It is generated in VaultSword, 
    // so grab the sword and check out the effect.
    internal class BubbleParticle : BasePRT
    {

        // The Texture property doesn't need to be overridden, as BasePRT has an automatic loading mechanism.
        // It automatically loads a .png file with the same name in the same directory.
        // This is similar to how ModProjectile works.
        // So, let's prepare a .png file called "ExamplePRT", which is an image with the same name as the class.
        // public override string Texture => base.Texture;

        // Override this function, it will be called once when the particle is generated.
        // PRT entities are independent instances, so the settings in this function
        // can also be applied to each instance individually, similar to ModProjectile.SetDefaults.
        public int MaxLifetime => 60;
        public override void SetProperty()
        {
            // PRTDrawMode determines which rendering mode the instance will be batched into.
            // This sets the color blending mode for the particle's rendering.
            // Here, we set it to additive blending mode. The effect brought by this field is real-time,
            // and it will batch all PRT instances in each draw call.
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = MaxLifetime; // Lifetime of 220 to 360 ticks.
            Color = new Color(93, 203, 243);
            Rotation = Main.rand.NextFloat(0, MathHelper.TwoPi); // Random rotation angle.
            Scale = Main.rand.NextFloat(0.2f, 0.8f); // Random scale between 0.5 and 1.5.
        }

        

        public float DrawScale;
        public float squishSpeed;
        public float squishAmount;
        public float squish;

        public override void AI()
        {

            // Adjust the rotation according to the movement direction.
            //Rotation += Main.rand.NextFloat(-0.1f, 0.1f);

            DrawScale = Scale;


            squishSpeed = 8f;
            squishAmount = 0.15f;
            squish = 1f + (float)System.Math.Sin(Lifetime * squishSpeed * 0.1f) * squishAmount;

            //// Relative position change
            //Position += Main.LocalPlayer.velocity;


            // Apply a fading effect near the end of its life.
            if (LifetimeCompletion > 0.8f)
            {
                Color *= 0.9f;
            }
        }

        // Override this drawing function. If you want to customize the drawing, return false here,
        // and the default drawing will not be applied.
         public override bool PreDraw(SpriteBatch spriteBatch)
            {


                Vector2 origin = new Vector2(TexValue.Width, TexValue.Height) * 0.5f;
                // Apply squish to the Y scale
                Vector2 scale = new Vector2(DrawScale, DrawScale * squish);

                spriteBatch.Draw(TexValue, Position, null, Color, Rotation, origin, scale, SpriteEffects.None, 0f);

                return true;
            }
    }
    
    
}