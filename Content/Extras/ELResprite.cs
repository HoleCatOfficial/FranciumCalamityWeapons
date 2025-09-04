using CalamityMod.Items.Weapons.Melee;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Extras
{
    public class ELResprite : ElementalLance
    {
        public override void SetDefaults()
        {
            Item.shoot = ModContent.ProjectileType<ELProjResprite>();
            base.SetDefaults();
        }
    }
}