using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Terraria;
using Terraria.Audio;

namespace FranciumCalamityWeapons.Common
{
    public class DTUtilsCalamity
    {
        public static Color GodSlayerInfernoGradient(float completion)
        {
            return Color.Lerp(new Color(39, 151, 171), new Color(252, 109, 202), completion);
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