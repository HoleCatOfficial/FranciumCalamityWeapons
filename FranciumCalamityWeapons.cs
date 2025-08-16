using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;


namespace FranciumCalamityWeapons
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class FranciumCalamityWeapons : Mod
	{
		public override void Load()
		{
			
			if (!ModLoader.HasMod("CalamityMod"))
			{
				throw new Exception("Calamity Mod is required for this mod to function.");
			}
			if (!ModLoader.HasMod("DestroyerTest"))
			{
				throw new Exception("Now, let me get this fucking straight. You decided to download a cross-modding bridge and intentionally disable it's main root? I mean, seriously. Was there an intent to be funny here? Or is there a deficiency in logic and reasoning inside that already tiny noggin or yours? Such an insult to try and play this mod without Constantine's Arsenal Enabled. Go enable it and hopefully your dumbfuck face doesnt have to see this exception again. Good- actually no, Bad Day to you.");
			}
		}

		// Removed GetEffect method as it's no longer needed with Asset<Effect> usage.
    }
}
