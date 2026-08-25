using BreadLibrary.Core.Graphics.Particles;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using DestroyerTest.Common;
using DestroyerTest.Content.Particles;
using FranciumCalamityWeapons.Common;
using FranciumCalamityWeapons.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpusLib.Content.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Projectiles
{
    public class CharmOfEgoExplosion : ModProjectile
    {
        public override string Texture => DTUtils.NoTexture;
        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 15;
            Projectile.tileCollide = false;
            Projectile.hide = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.DeerclopsIceAttack with { Volume = 1f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item122 with { Volume = 0.8f }, Projectile.Center);

            LerpingBloomRingSharp Ring = new();
            Ring.Prepare(Projectile.Center, Vector2.Zero, DTUtilsCalamity.CosmicPink, DTUtilsCalamity.CosmicBlue, 0.08f, 0.005f, 0.8f);
            ParticleEngine.Particles.Add(Ring);

            for (int i =0; i < 12; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.FireworksRGB, Main.rand.NextVector2Circular(17, 17), 0, DTUtilsCalamity.CosmicPink, 1f).noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 600);
            target.AddBuff(ModContent.BuffType<GalvanicCorrosion>(), 300);
        }
    }
}
