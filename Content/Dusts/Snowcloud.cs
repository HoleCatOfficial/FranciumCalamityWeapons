using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FranciumCalamityWeapons;

namespace FranciumCalamityWeapons.Content.Dusts
{
	public class Snowcloud : ModDust
	{
  
        public override void OnSpawn(Dust dust) {
			dust.velocity *= 0.2f; // Multiply the dust's start velocity by 0.4, slowing it down
			dust.noGravity = false; // Makes the dust have no gravity.
			dust.noLight = false; // Makes the dust emit no light.
			dust.scale *= 1.5f; // Multiplies the dust's initial scale by 1.5.
            int frameHeight = 24; // Adjust based on your texture
            dust.frame = new Rectangle(0, Main.rand.Next(4) * frameHeight, frameHeight, frameHeight);
            dust.alpha = 240;
		}

		public override bool Update(Dust dust) { // Calls every frame the dust is active
			dust.position += dust.velocity;
			dust.rotation += dust.velocity.X * 0.08f;
			dust.scale *= 0.99f;
            dust.velocity.Y += 0.0001f;

			float light = 0.35f * dust.scale;

            Lighting.AddLight(dust.position, dust.color.ToVector3() * 0.1f);

			if (dust.scale < 0.5f) {
				dust.active = false;
			}

			return false; // Return false to prevent vanilla behavior.
		}
	}
}