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
    internal class ColoredFireBase : BasePRT
    {

        // The Texture property doesn't need to be overridden, as BasePRT has an automatic loading mechanism.
        // It automatically loads a .png file with the same name in the same directory.
        // This is similar to how ModProjectile works.
        // So, let's prepare a .png file called "ExamplePRT", which is an image with the same name as the class.
        // public override string Texture => base.Texture;

        // Override this function, it will be called once when the particle is generated.
        // PRT entities are independent instances, so the settings in this function
        // can also be applied to each instance individually, similar to ModProjectile.SetDefaults.
        public int MaxLifetime => 20;
        public override void SetProperty()
        {
            // PRTDrawMode determines which rendering mode the instance will be batched into.
            // This sets the color blending mode for the particle's rendering.
            // Here, we set it to additive blending mode. The effect brought by this field is real-time,
            // and it will batch all PRT instances in each draw call.
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = MaxLifetime; // Lifetime of 220 to 360 ticks.
            Rotation = Main.rand.NextFloat(0, MathHelper.TwoPi); // Random rotation angle.
            Scale = Main.rand.NextFloat(0.2f, 0.8f); // Random scale between 0.5 and 1.5.

        }

        public override void AI()
        {
            // A simple AI that randomly changes direction.
            if (--ai[0] <= 0)
            {
                Velocity = Velocity.RotatedByRandom(0.3f);
                ai[0] = Main.rand.Next(10);
            }
            // Adjust the rotation according to the movement direction.
            //Rotation += Main.rand.NextFloat(-0.1f, 0.1f);



            //// Relative position change
            //Position += Main.LocalPlayer.velocity;


            // Apply a fading effect near the end of its life.
            if (LifetimeCompletion > 0.3f)
            {
                Color *= 0.9f;
            }
        }

        // Override this drawing function. If you want to customize the drawing, return false here,
        // and the default drawing will not be applied.
        public override bool PreDraw(SpriteBatch spriteBatch) => true;
    }

    internal class ColoredFire1 : ColoredFireBase
    {

    }

    internal class ColoredFire2 : ColoredFireBase
    {

    }
    internal class ColoredFire3 : ColoredFireBase
    {

    }
    internal class ColoredFire4 : ColoredFireBase
    {

    }
    internal class ColoredFire5 : ColoredFireBase
    {

    }
    internal class ColoredFire6 : ColoredFireBase
    {

    }
    internal class ColoredFire7 : ColoredFireBase
    {

    }
    
    
}