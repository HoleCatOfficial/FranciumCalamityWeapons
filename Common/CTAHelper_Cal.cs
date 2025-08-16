using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace FranciumCalamityWeapons.Common
{
    public class CalCTAUtils
    {
        public static float LeftDir = 0f;
        public static float DownDir = MathHelper.PiOver2;
        public static float RightDir = MathHelper.Pi;
        public static float UpDir = MathHelper.Pi + MathHelper.PiOver2;

        public static Color GodSlayerInfernoGradient(float completion)
        {
            
            return Color.Lerp(new Color(39, 151, 171), new Color(252, 109, 202), completion);
        }
    }

}