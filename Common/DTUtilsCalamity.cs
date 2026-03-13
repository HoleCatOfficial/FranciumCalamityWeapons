using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using OpusLib;
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
            return Color.Lerp(new Color(39, 151, 171), new Color(252, 109, 202), completion);
        }

        public static Color DeuxiemeColor = Opus.Sine(Color.DarkRed, Color.DarkOrchid);
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
    }

}