using Terraria;
using Terraria.ModLoader;

namespace FranciumCalamityWeapons.Content.Debuffs
{
    public class FracturedPhantomBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false; // This is a debuff
            Main.buffNoSave[Type] = true; // The buff won't persist after exiting the world
            Main.buffNoTimeDisplay[Type] = true; // Show buff duration
        }
        public static bool Inf = false;

        public override void Update(Player player, ref int buffIndex)
        {
            Inf = true;
        }
    }
}