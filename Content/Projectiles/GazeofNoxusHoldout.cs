using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using DestroyerTest.Common;
using DestroyerTest.Content.MeleeWeapons;
using Microsoft.Xna.Framework.Graphics;
using DestroyerTest.Content.Particles;
using DestroyerTest.Content.Buffs;
using FranciumCalamityWeapons.Content.FalseVacuumSet;
using Terraria.DataStructures;
using InnoVault.PRT;
using FranciumCalamityWeapons.Content.Particles;
using System;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    public class GazeofNoxusHoldout : ModProjectile
    {
        public SoundStyle Charge = new SoundStyle("FranciumCalamityWeapons/Audio/GazeofNoxusChargeup");
        public SoundStyle Beam = new SoundStyle("FranciumCalamityWeapons/Audio/GazeofNoxusBeam");
                public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 4000; // persistent
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.netImportant = true;
			Projectile.netUpdate = true;

        }

        public Vector2 OuterPoint;
        public int ChargeupTimer = 0;
        public float SoundTimer = 0;

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            if (player.channel)
            {
                SoundEngine.PlaySound(Charge, Projectile.Center);
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            
            Vector2 StarOffsetUp = Projectile.Center + new Vector2(0, -50).RotatedBy(Projectile.rotation);
			Vector2 StarOffsetDown = Projectile.Center + new Vector2(0, 50).RotatedBy(Projectile.rotation);
            Vector2 mountedCenter = player.MountedCenter;
            Vector2 toCursor = Main.MouseWorld - mountedCenter;
            toCursor.Normalize();
			
			float sine = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f); // -1 to 1
			float t = (sine + 1f) / 2f; // 0 to 1
            // Calculate a perpendicular vector to the direction to the cursor
            Vector2 perpendicular = toCursor.RotatedBy(MathHelper.PiOver2);

            // Make the perpendicular offset oscillate (e.g., up and down)
            float perpendicularDistance = 50f; // How far to move perpendicular
            Vector2 perpOffset = perpendicular * MathHelper.Lerp(-perpendicularDistance, perpendicularDistance, t);

            // Move the star outwards along the beam (from the player to the cursor)
            float outwardDistance = MathHelper.Lerp(0f, 300f, t); // 0 to 300 pixels outwards
            Vector2 outwardOffset = toCursor * outwardDistance;

            Vector2 StarOffset = Projectile.Center + outwardOffset + perpOffset;

            // Check if the player is holding the item and channeled
            if (player.HeldItem.type == ModContent.ItemType<GazeofNoxus>() && player.channel)
            {
                ChargeupTimer++;

                //player.statMana -= player.HeldItem.mana;
                float holdDistance = 20f;
                
               
                Vector2 desiredPos = mountedCenter + toCursor * holdDistance;

                OuterPoint = Projectile.Center + toCursor * 3000f; // 3000f is a large distance to ensure it's offscreen
                // Lock the projectile's position relative to the player

                Projectile.Center = desiredPos;

                if (ChargeupTimer >= 180)
                {
                    if (Main.rand.NextBool(4))
                    {// 1 in 3 chance to spawn a streak
                    // Make the color more pastel by blending with white
                        Color baseColor = new Color(Main.DiscoR / 2, (byte)(Main.DiscoG / 1.25f), (byte)(Main.DiscoB / 1.5f));
                        Color pastelColor = Color.Lerp(baseColor, Color.White, 0.5f); // 0.5f blends halfway to white
                        PRTLoader.NewParticle(PRTLoader.GetParticleID<StarParticle>(), StarOffset, Vector2.Zero, Color.White, 1);
                    }
                    if (ChargeupTimer > 180 && ChargeupTimer < 181)
                    {
                        PRTLoader.NewParticle(PRTLoader.GetParticleID<Boom3>(), Projectile.Center, Vector2.Zero, new Color(Main.DiscoR / 2, (byte)(Main.DiscoG / 1.25f), (byte)(Main.DiscoB / 1.5f)), 1);
                    }
                    player.GetModPlayer<ScreenshakePlayer>().screenshakeMagnitude = 12;
                    player.GetModPlayer<ScreenshakePlayer>().screenshakeTimer = 24;
                    // Play the sound once when charge completes, then start looping
                    float beamSoundLength = 2.96f; // seconds
                    float beamSoundTicks = beamSoundLength * 60f;

                    if (SoundTimer == 0f)
                    {
                        SoundEngine.PlaySound(SoundID.Thunder);
                        SoundEngine.PlaySound(Beam);

                        SoundTimer = 1f; // Start timer after first play
                    }
                    else
                    {
                        SoundTimer += 1f; // Increment timer by 1 tick (assuming 60 FPS)
                        if (SoundTimer >= beamSoundTicks)
                        {
                            SoundEngine.PlaySound(Beam);
                            SoundTimer = 1f; // Reset timer but avoid retriggering at 0
                        }
                    }
                    SoundTimer += 1f; // Increment timer by 1 tick (assuming 60 FPS)
                    // Add a vibrating effect by jittering the position slightly
                    Vector2 vibration = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                    Projectile.position += vibration;
                }
                else
                {
                    SoundTimer = 0f; // Reset timer if not charged
                }




                // Rotate to face the cursor
                Projectile.rotation = toCursor.ToRotation() + MathHelper.PiOver2;



                // Constantly face the direction it's pointing
                Projectile.direction = toCursor.X > 0 ? 1 : -1;

                // Shoot dust particles in a line from the tip
                Vector2 dustDirection = toCursor;
                Vector2 dustSpawn = Projectile.Center + dustDirection * Projectile.width * 0.5f;

                Vector2 randomSpawn = Projectile.position + new Vector2(Main.rand.NextFloat(Projectile.width), Main.rand.NextFloat(Projectile.height));

                if (ChargeupTimer >= 180)
                {
                    foreach (NPC npc in Main.npc)
                    {
                        if (npc.active && !npc.friendly && !npc.dontTakeDamage)
                        {
                            if (CheckLineCollisionWithAABV(npc.Hitbox))
                            {
                                int hit = Main.rand.Next(80, 140); // Set the damage amount as needed
                                NPC.HitInfo hitInfo = new NPC.HitInfo()
                                {
                                    Damage = hit,
                                    Knockback = 0f,
                                    HitDirection = 0,
                                    Crit = false
                                };
                                npc.StrikeNPC(hitInfo, false, false);
                            }
                        }
                    }
                }


            }
            else
            {
                // Kill the projectile if the item is not being held
                Projectile.Kill();
            }
        }

        public bool CheckLineCollisionWithAABV(Rectangle aabb)
        {
            Vector2 start = Projectile.Center;
            Vector2 end = OuterPoint;

            // Use Collision.CheckAABBvLineCollision from Terraria
            return Collision.CheckAABBvLineCollision(
                aabb.TopLeft(), 
                aabb.Size(), 
                start, 
                end
            );
        }

        public override void PostDraw(Color lightColor)
        {
            if (ChargeupTimer >= 180)
            {
                DrawTelegraph(Projectile.Center + new Vector2(0, 20).RotatedBy(Projectile.rotation), OuterPoint, ModContent.Request<Texture2D>("FranciumCalamityWeapons/Content/Particles/BeamLine").Value);
            }
            base.PostDraw(lightColor);
        }
        
        public void DrawTelegraph(Vector2 start, Vector2 end, Texture2D texture)
        {
            Vector2 direction = end - start;
            float length = direction.Length();
            direction.Normalize();
            texture ??= ModContent.Request<Texture2D>("FranciumCalamityWeapons/Content/Particles/BeamLine").Value;
            SpriteBatch spriteBatch = Main.spriteBatch;

            float rotation = direction.ToRotation();

            // Assuming your texture is a chain segment, like 16px long
            float segmentLength = texture.Height * 0.75f; // or Width, depending on the texture orientation
            //Effect Wavy = Mod.Assets.Request<Effect>("Assets/Shaders/Custom/WavyDeform", AssetRequestMode.ImmediateLoad).Value;
            //Wavy.CurrentTechnique.Passes["WavyPass"].Apply();

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (float i = 0; i < length; i += segmentLength)
            {
                Vector2 position = start + direction * i;

                Main.spriteBatch.Draw(
                    texture,
                    position - Main.screenPosition,
                    null,
                    new Color(Main.DiscoR / 2, (byte)(Main.DiscoG / 1.25f), (byte)(Main.DiscoB / 1.5f)),
                    rotation + MathHelper.PiOver2, // Adjust if your texture points upward
                    new Vector2(texture.Width / 2f, texture.Height / 2f), // Origin at center
                    1f, // Scale
                    SpriteEffects.None,
                    0f
                );
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }       

		
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Bleeding, 120);
        }

    }
}