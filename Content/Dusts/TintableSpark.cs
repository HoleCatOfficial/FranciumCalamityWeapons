using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace FranciumCalamityWeapons.Content.Dusts
{
	public class TintableSpark : ModDust
	{
  
        public override void OnSpawn(Dust dust) {
			dust.velocity *= 0.2f; // Multiply the dust's start velocity by 0.4, slowing it down
			dust.noGravity = true; // Makes the dust have no gravity.
			dust.noLight = false; // Makes the dust emit no light.
			dust.scale *= 3.0f; // Multiplies the dust's initial scale by 1.5.
		}
		/// <summary>
		/// The color of the dust that is lerped between white and dark for the "ember" effect.
		/// </summary>
        public Color baseColor = new Color(255, 255, 255);

		public override bool Update(Dust dust) { // Calls every frame the dust is active
			dust.position += dust.velocity;
			dust.rotation += dust.velocity.ToRotation();
			dust.scale *= 0.99f;
            dust.velocity.Y += 0.0004f;
            
            ChangeColor(out Color finalColor);
           
		    dust.noLightEmittence = false; 
		    dust.color = finalColor;

			float light = 0.35f * dust.scale;
			Lighting.AddLight(dust.position, new Vector3(baseColor.R * light / 255f, baseColor.G * light / 255f, baseColor.B * light / 255f));
			if (dust.scale < 0.2f) {
				dust.active = false;
			}

			return false; // Return false to prevent vanilla behavior.
		}

        public void ChangeColor (out Color finalColor)
        {
            Color StartColor = Color.White;
            Color MidColor = baseColor;
            Color EndColor = Color.DimGray;

            float time = (Main.GlobalTimeWrappedHourly % 2f);

            if (time < 1f)
                finalColor = Color.Lerp(StartColor, MidColor, time);
            else
                finalColor = Color.Lerp(MidColor, EndColor, time - 1f);
        }
	}

	public static class TintableSparkExtension
	{
		public static TintableSpark GetTintableSpark(this ModDust modDust)
		{
			return (TintableSpark)ModContent.GetInstance<TintableSpark>();
		}
	}

}