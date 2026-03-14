using DestroyerTest.Common;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Particles
{
    internal class DeuxiemeParticle2 : BasePRT
    {
        public int MaxLifetime => 180;
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = MaxLifetime;
            ShouldKillWhenOffScreen = false;
        }

        public override bool ShouldUpdatePosition()
        {
            return true;
        }

        public override void AI()
        {

            if (LifetimeCompletion > 0.8f)
            {
                Scale *= 0.9f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(TexValue, Position - Main.screenPosition, null, Color, Rotation, TexValue.Size() / 2, Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(TexValue, Position - Main.screenPosition, null, DTColorUtils.Pastel(Color, 0.9f), Rotation, TexValue.Size() / 2, Scale * 0.2f, SpriteEffects.None, 0f);

            return false;
        }
    }

    internal class DeuxiemeParticle3 : BasePRT
    {
        public int MaxLifetime => 15;
        public override void SetProperty()
        {
            PRTDrawMode = PRTDrawModeEnum.AdditiveBlend;
            Lifetime = MaxLifetime;
            ShouldKillWhenOffScreen = false;
        }

        public override bool ShouldUpdatePosition()
        {
            return true;
        }

        public override void AI()
        {

            if (LifetimeCompletion > 0.3f)
            {
                Scale *= 0.9f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(TexValue, Position - Main.screenPosition, null, Color, Rotation, TexValue.Size() / 2, Scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(TexValue, Position - Main.screenPosition, null, DTColorUtils.Pastel(Color, 0.9f), Rotation, TexValue.Size() / 2, Scale * 0.2f, SpriteEffects.None, 0f);

            return false;
        }
    }


}