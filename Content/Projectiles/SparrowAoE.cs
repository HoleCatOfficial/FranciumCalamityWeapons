using System;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using FranciumCalamityWeapons.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles
{
	// This example is similar to the Wooden Arrow projectile
	public class SparrowAoE : ModProjectile
	{
        
		public override void SetStaticDefaults()
		{
			// If this arrow would have strong effects (like Holy Arrow pierce), we can make it fire fewer projectiles from Daedalus Stormbow for game balance considerations like this:
			//ProjectileID.Sets.FiresFewerFromDaedalusStormbow[Type] = true;
		}

        public override void SetDefaults()
        {
            Projectile.width = 172; // The width of projectile hitbox
            Projectile.height = 172; // The height of projectile hitbox
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.timeLeft = 1200;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.tileCollide = false;
            Projectile.scale = 8;
            //Projectile.usesLocalNPCImmunity = true;
            //Projectile.localNPCHitCooldown = 1;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            Color baseColor = new Color(Main.DiscoR / 2, (byte)(Main.DiscoG / 1.25f), (byte)(Main.DiscoB / 1.5f));
            //Color pastelColor = Color.Lerp(baseColor, Color.White, 0.5f);

            SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

            spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            spriteBatch.Draw(projectileTexture, Projectile.Center - Main.screenPosition, null, baseColor, Projectile.rotation, projectileTexture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }



        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && !npc.dontTakeDamage && npc.lifeMax > 5)
                {
                    if (Projectile.Hitbox.Intersects(npc.Hitbox))
                    {
                        // You can add immunity logic if needed to prevent damage spam.
                        // Example: store hit cooldowns using Projectile.localNPCImmunity[npc.whoAmI]
                        npc.StrikeNPC(new NPC.HitInfo
                        {
                            Damage = 40,
                            Knockback = 0f,
                            HitDirection = Projectile.direction,
                            Crit = false
                        }, false, false);

                        npc.AddBuff(ModContent.BuffType<MiracleBlight>(), 240);
                        SoundEngine.PlaySound(SoundID.Item65, Projectile.position);
                    }
                }
            }
        }


		public override void OnKill(int timeLeft) {
			
			
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item65, Projectile.position); // Plays the basic sound most projectiles make when hitting blocks.
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 240);
            Color baseColor = new Color(Main.DiscoR / 2, (byte)(Main.DiscoG / 1.25f), (byte)(Main.DiscoB / 1.5f));
            Color pastelColor = Color.Lerp(baseColor, Color.White, 0.5f);
            NPC.HitInfo customHit = new NPC.HitInfo()
            {
                Damage = 40,
                Knockback = 0f,
                HitDirection = hit.HitDirection,
                Crit = hit.Crit
            };
            target.StrikeNPC(customHit, false, true);
        }

        
	}
}