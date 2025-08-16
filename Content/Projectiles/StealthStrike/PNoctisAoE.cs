using System;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using FranciumCalamityWeapons.Content.Debuffs;
using FranciumCalamityWeapons.Content.Particles;
using InnoVault.PRT;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles.StealthStrike
{
	// This example is similar to the Wooden Arrow projectile
	public class PNoctisAoE : ModProjectile
	{
        
		public override void SetStaticDefaults()
		{
			// If this arrow would have strong effects (like Holy Arrow pierce), we can make it fire fewer projectiles from Daedalus Stormbow for game balance considerations like this:
			//ProjectileID.Sets.FiresFewerFromDaedalusStormbow[Type] = true;
		}

        public override void SetDefaults()
        {
            Projectile.width = 72; // The width of projectile hitbox
            Projectile.height = 128; // The height of projectile hitbox
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.timeLeft = 1200;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.netUpdate = true;
            Projectile.tileCollide = false;
            Projectile.scale = 3;
            //Projectile.usesLocalNPCImmunity = true;
            //Projectile.localNPCHitCooldown = 1;
		}
        
        public override bool PreDraw(ref Color lightColor)
        {
            Color baseColor = new Color(Main.DiscoR / 2, (byte)(Main.DiscoG / 1.25f), (byte)(Main.DiscoB / 1.5f));
            //Color pastelColor = Color.Lerp(baseColor, Color.White, 0.5f);

            SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;

            spriteBatch.Draw(projectileTexture, Projectile.Center - Main.screenPosition, null, baseColor, Projectile.rotation, projectileTexture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);



            return false;
        }



        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            PRTLoader.NewParticle(PRTLoader.GetParticleID<StarParticle>(), Main.rand.NextVector2FromRectangle(Projectile.getRect()), Vector2.Zero, new Color(251, 242, 54), 1);

        }


		public override void OnKill(int timeLeft) {
			
			
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(new SoundStyle("FranciumCalamityWeapons/Audio/SupressionApply"), target.Center); // Plays the basic sound most projectiles make when hitting blocks.
            target.AddBuff(ModContent.BuffType<Supression>(), 240);
            
        }

        
	}
}