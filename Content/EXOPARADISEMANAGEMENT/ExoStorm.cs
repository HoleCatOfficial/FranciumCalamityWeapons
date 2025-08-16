using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.EXOPARADISEMANAGEMENT
{
    public class ExoplanetStormEffect : ModSceneEffect
    {
        private int screenWidth;
        private int screenHeight;
        private int GoreLifetime = 480;

        public override bool IsSceneEffectActive(Player player)
        {
            // You can replace this condition with any check you want.
            return true; // Effect always active for testing.
        }
        public override void SpecialVisuals(Player player, bool isActive)
        {
            EntitySource_Misc source = new EntitySource_Misc("ExoplanetStormEffect");
            

            GoreLifetime--;
            // Update screen dimensions each frame
            screenWidth = Main.screenWidth;
            screenHeight = Main.screenHeight;

            // Debug - Spawning a random effect at a random position
            if (Main.rand.NextFloat() < 0.05f) // 5% chance per frame
            {
                Vector2 spawnPos = new Vector2(
                    Main.rand.Next(0, screenWidth),
                    Main.rand.Next(0, screenHeight)
                );
                int StormGoreType = Mod.Find<ModGore>("StormGore").Type;

                // For now, just spawn a basic dust effect to confirm position
                Dust.NewDust(spawnPos, 10, 10, DustID.Smoke, 0, 0, 150, Color.White, 1.5f);
                int goreIndex = Gore.NewGore(source, spawnPos, new Vector2(Main.rand.Next(24), Main.rand.Next(4)), StormGoreType);
                Gore StormGoreEntity = Main.gore[goreIndex];
                if (GoreLifetime == 0)
                {
                    StormGoreEntity.active = false;
                }
            }
        }
    }
    public class StormGore : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.timeLeft = 600; 
        }
    }
}
