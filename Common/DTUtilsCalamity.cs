using DestroyerTest.Common;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using OpusLib;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace FranciumCalamityWeapons.Common
{
    public class DTUtilsCalamity
    {
        public static DTUtilsCalamity Instance = new DTUtilsCalamity();
        public static Color GodSlayerInfernoGradient(float completion)
        {
            return Color.Lerp(new Color(38, 148, 237), new Color(217, 46, 223), completion);
        }

        public static Color[] UC_Colormap = [Color.White, new Color(236, 255, 192), new Color(255, 241, 148), new Color(255, 166, 148), new Color(255, 93, 148), new Color(167, 46, 180)];

        public static Color DeuxiemeColor = Opus.Sine(Color.DarkRed, Color.DarkOrchid);

        public static void DrawOverlordChargeBar(float barScale, Vector2 position, float progress, float Opacity)
        {
            var barBG = DTAssetLibCalamity.OverlordBar.Back.Value;
            var barFG = DTAssetLibCalamity.OverlordBar.Front.Value;
            var barFrame = DTAssetLibCalamity.OverlordBar.Frame.Value;

            Vector2 barOrigin = barBG.Size() * 0.5f;
            Vector2 drawPos = position;
            Rectangle frameCrop = new Rectangle(0, 0, (int)(progress * barFG.Width), barFG.Height);

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(barBG, drawPos, null, Color.White * Opacity, 0f, barOrigin, barScale, 0f, 0f);
            spriteBatch.Draw(barFG, drawPos, frameCrop, Color.White * Opacity, 0f, barOrigin, barScale, 0f, 0f);
            spriteBatch.Draw(barFrame, drawPos, null, Color.White * Opacity, 0f, barOrigin, barScale, 0f, 0f);
        }
    }

    internal class CalamityColorUpdate : ModSystem
    {
        public override void PostUpdateDusts()
        {
            DTUtilsCalamity.DeuxiemeColor = Opus.Sine(Color.DarkRed, Color.DarkOrchid);
        }

    }

    public class DTAssetLibCalamity
    {
        public static readonly string AudioPath = "FranciumCalamityWeapons/Audio";
        public struct StealthStrike
        {
            public static SoundStyle SpearOfAspiration = new SoundStyle($"{AudioPath}/SpearOfAspirationStealthStrike");
        }

        public struct OverlordBar
        {
            public static Asset<Texture2D> Frame = ModContent.Request<Texture2D>("FranciumCalamityWeapons/Content/Extras/OverlordBarFrame");
            public static Asset<Texture2D> Front = ModContent.Request<Texture2D>("FranciumCalamityWeapons/Content/Extras/OverlordBarFront");
            public static Asset<Texture2D> Back = ModContent.Request<Texture2D>("FranciumCalamityWeapons/Content/Extras/OverlordBarBack");
        }
    }

}