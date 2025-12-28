using CalamityMod.Projectiles.Magic;
using DestroyerTest.Content.RiftArsenal;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Common
{
	public class OverlordCountPlayer : ModPlayer
	{
		// Here we create a custom resource, similar to mana or health.
		// Creating some variables to define the current value of our example resource as well as the current maximum value. We also include a temporary max value, as well as some variables to handle the natural regeneration of this resource.
		public int HitCount; // Current value of our example resource
		public const int DefaultMaximum = 35; // Default maximum value of example resource
		public int HitThreshold; // Buffer variable that is used to reset maximum resource to default value in ResetDefaults().
		public int HitThreshold2; // Maximum amount of our example resource. We will change that variable to increase maximum amount of our resource
		public float Decay; // By changing that variable we can increase/decrease regeneration rate of our resource

        public int DecayStartTimer = 0;
        // In order to make the Example Resource example straightforward, several things have been left out that would be needed for a fully functional resource similar to mana and health. 
        // Here are additional things you might need to implement if you intend to make a custom resource:
        // - Multiplayer Syncing: The current example doesn't require MP code, but pretty much any additional functionality will require this. ModPlayer.SendClientChanges and CopyClientState will be necessary, as well as SyncPlayer if you allow the user to increase exampleResourceMax.
        // - Save/Load permanent changes to max resource: You'll need to implement Save/Load to remember increases to your exampleResourceMax cap.

        public override void Initialize()
        {
            HitThreshold = DefaultMaximum;
        }

		public override void ResetEffects() {
			ResetVariables();
		}

		public override void UpdateDead() {
			ResetVariables();
		}

		// We need this to ensure that regeneration rate and maximum amount are reset to default values after increasing when conditions are no longer satisfied (e.g. we unequip an accessory that increases our resource)
		private void ResetVariables() {
			Decay = 0f;
			HitThreshold2 = HitThreshold;
		}

		public override void PostUpdateMiscEffects() {
			UpdateResource();
		}

        public override void PostUpdate()
        {
            Player player = Main.LocalPlayer;

            CapResourceGodMode();

            // Decay system
            if (HitCount > 0)
            {
                DecayStartTimer++;

                if (DecayStartTimer >= 120) // 2 seconds of no hits = decay starts
                {
                    HitCount--;
                    DecayStartTimer = 100; // Let it count back up again next tick
                }
            }

            // Summon the projectile once the threshold is hit
            if (HitCount >= HitThreshold2)
            {
                HitCount = 0; // Reset after summon
                DecayStartTimer = 0;

                SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/DevourerDeathImpact") with { PitchVariance = 0.3f });
                
                Vector2 spawnOffset = new(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10));
                Vector2 direction = Vector2.Normalize(Main.MouseWorld - player.Center);
                Vector2 velocity = direction * 12f; // Arbitrary speed
                Projectile.NewProjectile(
                    Entity.GetSource_FromThis(),
                    player.Center + spawnOffset,
                    velocity,
                    ModContent.ProjectileType<ApotheosisWorm>(),
                    80,
                    1f,
                    player.whoAmI
                );
            }
        }



		// Lets do all our logic for the custom resource here, such as limiting it, increasing it and so on.
		private void UpdateResource() {
			// Limit exampleResourceCurrent from going over the limit imposed by exampleResourceMax.
			//Timer = Utils.Clamp(Timer, 0, ImmunityThreshold2);
		}

		private void CapResourceGodMode() {
			if (Main.myPlayer == Player.whoAmI && Player.creativeGodMode) {
				Decay = HitThreshold2;
			}
		}
        
	}
}