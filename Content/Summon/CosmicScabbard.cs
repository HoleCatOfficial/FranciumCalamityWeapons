
using FranciumCalamityWeapons.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;
using tModPorter.Rewriters;

namespace FranciumCalamityWeapons.Content.Summon
{
	// This file contains all the code necessary for a minion
	// - ModItem - the weapon which you use to summon the minion with
	// - ModBuff - the icon you can click on to despawn the minion
	// - ModProjectile - the minion itself

	// It is not recommended to put all these classes in the same file. For demonstrations sake they are all compacted together so you get a better overview.
	// To get a better understanding of how everything works together, and how to code minion AI, read the guide: https://github.com/tModLoader/tModLoader/wiki/Basic-Minion-Guide
	// This is NOT an in-depth guide to advanced minion AI
	public class CosmicScabbardBuff : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
			Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
		}

		public override void Update(Player player, ref int buffIndex) {
			// If the minions exist reset the buff time, otherwise remove the buff from the player
			if (player.ownedProjectileCounts[ModContent.ProjectileType<CosmicSaber>()] > 0) {
				player.buffTime[buffIndex] = 18000;
			}
			else {
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}

	public class CosmicScabbard : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; // This lets the player target anywhere on the whole screen while using a controller
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;

			ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f; // The default value is 1, but other values are supported. See the docs for more guidance. 
		}

		public override void SetDefaults() {
			Item.damage = 3600;
			Item.knockBack = 0f;
			Item.mana = 100; // mana cost
			Item.width = 46;
			Item.height = 46;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.RaiseLamp; // how the player's arm moves when using the item
			Item.value = 500000;
			Item.rare = ItemRarityID.Expert;
			Item.UseSound = new SoundStyle("FranciumCalamityWeapons/Audio/DevourerDeath");

			// These below are needed for a minion weapon
			Item.noMelee = true; // this item doesn't do any melee damage
			Item.DamageType = DamageClass.Summon; // Makes the damage register as summon. If your item does not have any damage type, it becomes true damage (which means that damage scalars will not affect it). Be sure to have a damage type
			Item.buffType = ModContent.BuffType<CosmicScabbardBuff>();
			// No buffTime because otherwise the item tooltip would say something like "1 minute duration"
			Item.shoot = ModContent.ProjectileType<CosmicSaber>(); // This item creates the minion projectile
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			// Here you can change where the minion is spawned. Most vanilla minions spawn at the cursor position
			position = Main.MouseWorld;
			ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.BlackLightningHit,
				new ParticleOrchestraSettings { PositionInWorld = position },
				type);
		}

       
       // Define minionTypes as a class field so both methods can access it
    private static readonly List<int> minionTypes = new List<int>
    {
        ModContent.ProjectileType<CosmicSaber>(),
        ModContent.ProjectileType<LesserDefiler>(),
        ModContent.ProjectileType<GreaterDefiler>(),
        ModContent.ProjectileType<NebulousShock>(),
		ModContent.ProjectileType<NebulousWinter>(),
        ModContent.ProjectileType<WhiteDwarf>(),
        ModContent.ProjectileType<Overlord>()
    };

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
        player.AddBuff(Item.buffType, 2);

        // Iterate through the list and spawn each minion
        foreach (int minionType in minionTypes)
        {
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, minionType, damage, knockback, Main.myPlayer);
            projectile.originalDamage = Item.damage;
        }

        // Since we spawned the projectile manually already, return false so the game doesn't spawn another one
        return false;
    }

		
		public override void AddRecipes() {
			if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod) && ModLoader.TryGetMod("DestroyerTest", out Mod DestroyerTest))
			{
				if (calamityMod.TryFind("CosmiliteBar", out ModItem CB) && calamityMod.TryFind("RuinousSoul", out ModItem RS) && calamityMod.TryFind("CosmicAnvil", out ModTile CA) && DestroyerTest.TryFind("Tenebris", out ModItem T))
				{
					Recipe recipe = CreateRecipe();
					recipe.AddIngredient(RS.Type, 28);
					recipe.AddIngredient(CB.Type, 12);
					recipe.AddIngredient(T.Type, 22);
					recipe.AddTile(CA.Type);
					recipe.Register();
				}
			}
		}

	}

	

	// This minion shows a few mandatory things that make it behave properly.
	// Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
	// If the player targets a certain NPC with right-click, it will fly through tiles to it
	// If it isn't attacking, it will float near the player with minimal movement
	public class CosmicSaber : ModProjectile
	{
		public override string Texture => "FranciumCalamityWeapons/Content/Projectiles/CosmicScabbard/CosmicSaber";
		private void GenerateDust()
		{
			
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, DustID.Copper,
					0, 0, 254, Scale: 1.0f);
				dust.velocity += Projectile.velocity * 0.5f;
				dust.velocity *= 0.5f;
				dust.noGravity = true;
		
		}

		public override void SetStaticDefaults() {
			ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

			Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion

			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
		}

		public sealed override void SetDefaults() {
			Projectile.width = 68;
			Projectile.height = 68;
			Projectile.tileCollide = false; // Makes the minion go through tiles freely

			// These below are needed for a minion weapon
			Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.minion = true; // Declares this as a minion (has many effects)
			Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
			Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
			Projectile.extraUpdates = 2;
			
		}

		public override bool PreDraw(ref Color lightColor) {
			// Draws an afterimage trail. See https://github.com/tModLoader/tModLoader/wiki/Basic-Projectile#afterimage-trail for more information.

			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = Projectile.oldPos.Length - 1; k > 0; k--) {
				Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}

			return true;
		}


		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles() {
			return false;
		}

		// This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
		public override bool MinionContactDamage() {
			return true;
		}

		// The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
		public override void AI() {

			
			GenerateDust();
			
			Player owner = Main.player[Projectile.owner];

			
			if (!CheckActive(owner)) {
				return;
			}

			GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
			SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
			Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
			Visuals();
		}

		// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
		private bool CheckActive(Player owner) {
			if (owner.dead || !owner.active) {
				owner.ClearBuff(ModContent.BuffType<CosmicScabbardBuff>());

				return false;
			}

			if (owner.HasBuff(ModContent.BuffType<CosmicScabbardBuff>())) {
				Projectile.timeLeft = 2;
			}

            
			return true;
		}



		
		private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition) {
			
			GenerateDust();

			Vector2 idlePosition = owner.Center;
			idlePosition.Y -= 48f; // Go up 48 coordinates (three tiles from the center of the player)

			
			// If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
			// The index is projectile.minionPos
			float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
			idlePosition.X += minionPositionOffsetX; // Go behind the player

			// All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

			// Teleport to player if distance is too big
			vectorToIdlePosition = idlePosition - Projectile.Center;
			distanceToIdlePosition = vectorToIdlePosition.Length();

			if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 1500f) {
				SoundStyle Warp = new SoundStyle("FranciumCalamityWeapons/Audio/ExobladeBeamSlash");
				SoundEngine.PlaySound(Warp);
				// Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
				// and then set netUpdate to true
				Projectile.position = idlePosition;
				Projectile.velocity *= 0.1f;
				Projectile.netUpdate = true;
				ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.BlackLightningHit,
				new ParticleOrchestraSettings { PositionInWorld = Projectile.Center },
				Projectile.owner);
			}

			// If your minion is flying, you want to do this independently of any conditions
			float overlapVelocity = 0.04f;

			// Fix overlap with other minions
			foreach (var other in Main.ActiveProjectiles) {
				if (other.whoAmI != Projectile.whoAmI && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width) {
					if (Projectile.position.X < other.position.X) {
						Projectile.velocity.X -= overlapVelocity;
					}
					else {
						Projectile.velocity.X += overlapVelocity;
					}

					if (Projectile.position.Y < other.position.Y) {
						Projectile.velocity.Y -= overlapVelocity;
					}
					else {
						Projectile.velocity.Y += overlapVelocity;
					}
				}
			}
		}

		private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter) {
			// Starting search distance
			distanceFromTarget = 700f;
			targetCenter = Projectile.position;
			foundTarget = false;

			GenerateDust();
			
			// This code is required if your minion weapon has the targeting feature
			if (owner.HasMinionAttackTargetNPC) {
				NPC npc = Main.npc[owner.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, Projectile.Center);

				// Reasonable distance away so it doesn't target across multiple screens
				if (between < 2000f) {
					distanceFromTarget = between;
					targetCenter = npc.Center;
					foundTarget = true;
				}
			}

			if (!foundTarget) {
				// This code is required either way, used for finding a target
				foreach (var npc in Main.ActiveNPCs) {
					if (npc.CanBeChasedBy()) {
						float between = Vector2.Distance(npc.Center, Projectile.Center);
						bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
						bool inRange = between < distanceFromTarget;
						bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
						// Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
						// The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
						bool closeThroughWall = between < 100f;

						if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall)) {
							distanceFromTarget = between;
							targetCenter = npc.Center;
							foundTarget = true;
						}
					}
				}
			}
			
			// friendly needs to be set to true so the minion can deal contact damage
			// friendly needs to be set to false so it doesn't damage things like target dummies while idling
			// Both things depend on if it has a target or not, so it's just one assignment here
			// You don't need this assignment if your minion is shooting things instead of dealing contact damage
			Projectile.friendly = foundTarget;
		}

		private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition) {
			float speed = 50f;
			float inertia = 140f;
			
			GenerateDust();

			if (foundTarget) {
				if (distanceFromTarget > 40f) {
					// If not in "strike-through" mode, home in
					if (Projectile.ai[1] == 0) {
						Vector2 direction = targetCenter - Projectile.Center;
						direction.Normalize();
						direction *= speed;

						float targetAngle = Projectile.AngleTo(targetCenter * MathHelper.ToRadians(360));
						Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;

						// If close enough, enter "strike-through" mode
						if (distanceFromTarget < 50f) {
							SoundEngine.PlaySound(SoundID.Item66);
							Projectile.ai[1] = 1; // Enter strike-through phase
							Projectile.ai[0] = 0; // Reset timer
						}
						Projectile.rotation = targetAngle;
					}
				}
			}

			// If in "strike-through" mode, keep moving forward without changing direction
			if (Projectile.ai[1] == 1) {
				Projectile.ai[0]++; // Increment timer

				if (Projectile.ai[0] < 20) {
					// Keep moving in the same direction for a bit
					Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed;
				} else {
					// Exit "strike-through" mode after 20 ticks (~1/3 of a second)
					Projectile.ai[1] = 0;
				}
			}

			if (!foundTarget) {
				// Reset "strike-through" state when there's no target
				Projectile.ai[1] = 0;

				if (distanceToIdlePosition > 600f) {
					speed = 12f;
					inertia = 60f;
				}
				else {
					speed = 4f;
					inertia = 80f;
				}

				if (distanceToIdlePosition > 20f) {
					vectorToIdlePosition.Normalize();
					vectorToIdlePosition *= speed;
					Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
				}
				else if (Projectile.velocity == Vector2.Zero) {
					Projectile.velocity.X = -0.15f;
					Projectile.velocity.Y = -0.05f;
				}
			}
		}


		private void Visuals() {
			// So it will lean slightly towards the direction it's moving
			Projectile.rotation = Projectile.velocity.X * 0.5f;

			GenerateDust();

			// This is a simple "loop through all frames from top to bottom" animation
			//int frameSpeed = 5;

			//Projectile.frameCounter++;

			//if (Projectile.frameCounter >= frameSpeed) {
				//Projectile.frameCounter = 0;
				//Projectile.frame++;

				//if (Projectile.frame >= Main.projFrames[Projectile.type]) {
					//Projectile.frame = 0;
				//}
			//}

			// Some visuals here
			// Define two colors to cycle between for the stroke
                Color lightColor1 = new Color(255, 82, 194);
                Color lightColor2 = new Color(89, 230, 255);

                // Use a sine wave to smoothly transition between the two colors
                float lerpAmount = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)));
                Color LightColor = Color.Lerp(lightColor1, lightColor2, lerpAmount);
			Lighting.AddLight(Projectile.Center, LightColor.ToVector3() * 0.78f);
		}

       public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
			{
				if (calamityMod.TryFind("GodSlayerInferno", out ModBuff GSI))
				{
					target.AddBuff(GSI.Type, 120);
				}
			}
		}

    }
	public class LesserDefiler : ModProjectile
	{
		public override string Texture => "FranciumCalamityWeapons/Content/Projectiles/CosmicScabbard/LesserDefiler";
		private void GenerateDust()
		{
			
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, DustID.Tin,
					0, 0, 254, Scale: 1.0f);
				dust.velocity += Projectile.velocity * 0.5f;
				dust.velocity *= 0.5f;
				dust.noGravity = true;
		
		}

		public override void SetStaticDefaults() {
			ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

			Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion

			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
		}

		public sealed override void SetDefaults() {
			Projectile.width = 60;
			Projectile.height = 60;
			Projectile.tileCollide = false; // Makes the minion go through tiles freely

			// These below are needed for a minion weapon
			Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.minion = true; // Declares this as a minion (has many effects)
			Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
			Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
			Projectile.extraUpdates = 2;
			
		}

		public override bool PreDraw(ref Color lightColor) {
			// Draws an afterimage trail. See https://github.com/tModLoader/tModLoader/wiki/Basic-Projectile#afterimage-trail for more information.

			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = Projectile.oldPos.Length - 1; k > 0; k--) {
				Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}

			return true;
		}



		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles() {
			return false;
		}

		// This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
		public override bool MinionContactDamage() {
			return true;
		}

		// The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
		public override void AI() {

			
			GenerateDust();
			
			Player owner = Main.player[Projectile.owner];

			
			if (!CheckActive(owner)) {
				return;
			}

			GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
			SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
			Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
			Visuals();
		}

		// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
		private bool CheckActive(Player owner) {
			if (owner.dead || !owner.active) {
				owner.ClearBuff(ModContent.BuffType<CosmicScabbardBuff>());

				return false;
			}

			if (owner.HasBuff(ModContent.BuffType<CosmicScabbardBuff>())) {
				Projectile.timeLeft = 2;
			}

            
			return true;
		}



		
		private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition) {
			
			GenerateDust();

			Vector2 idlePosition = owner.Center;
			idlePosition.Y -= 48f; // Go up 48 coordinates (three tiles from the center of the player)

			
			// If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
			// The index is projectile.minionPos
			float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
			idlePosition.X += minionPositionOffsetX; // Go behind the player

			// All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

			// Teleport to player if distance is too big
			vectorToIdlePosition = idlePosition - Projectile.Center;
			distanceToIdlePosition = vectorToIdlePosition.Length();

			if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 1500f) {
				SoundStyle Warp = new SoundStyle("FranciumCalamityWeapons/Audio/ExobladeBeamSlash");
				SoundEngine.PlaySound(Warp);
				// Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
				// and then set netUpdate to true
				Projectile.position = idlePosition;
				Projectile.velocity *= 0.1f;
				Projectile.netUpdate = true;
				ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.BlackLightningHit,
				new ParticleOrchestraSettings { PositionInWorld = Projectile.Center },
				Projectile.owner);
			}

			// If your minion is flying, you want to do this independently of any conditions
			float overlapVelocity = 0.04f;

			// Fix overlap with other minions
			foreach (var other in Main.ActiveProjectiles) {
				if (other.whoAmI != Projectile.whoAmI && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width) {
					if (Projectile.position.X < other.position.X) {
						Projectile.velocity.X -= overlapVelocity;
					}
					else {
						Projectile.velocity.X += overlapVelocity;
					}

					if (Projectile.position.Y < other.position.Y) {
						Projectile.velocity.Y -= overlapVelocity;
					}
					else {
						Projectile.velocity.Y += overlapVelocity;
					}
				}
			}
		}

		private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter) {
			// Starting search distance
			distanceFromTarget = 700f;
			targetCenter = Projectile.position;
			foundTarget = false;

			GenerateDust();
			
			// This code is required if your minion weapon has the targeting feature
			if (owner.HasMinionAttackTargetNPC) {
				NPC npc = Main.npc[owner.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, Projectile.Center);

				// Reasonable distance away so it doesn't target across multiple screens
				if (between < 2000f) {
					distanceFromTarget = between;
					targetCenter = npc.Center;
					foundTarget = true;
				}
			}

			if (!foundTarget) {
				// This code is required either way, used for finding a target
				foreach (var npc in Main.ActiveNPCs) {
					if (npc.CanBeChasedBy()) {
						float between = Vector2.Distance(npc.Center, Projectile.Center);
						bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
						bool inRange = between < distanceFromTarget;
						bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
						// Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
						// The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
						bool closeThroughWall = between < 100f;

						if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall)) {
							distanceFromTarget = between;
							targetCenter = npc.Center;
							foundTarget = true;
						}
					}
				}
			}
			
			// friendly needs to be set to true so the minion can deal contact damage
			// friendly needs to be set to false so it doesn't damage things like target dummies while idling
			// Both things depend on if it has a target or not, so it's just one assignment here
			// You don't need this assignment if your minion is shooting things instead of dealing contact damage
			Projectile.friendly = foundTarget;
		}

		private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition) {
			float speed = 50f;
			float inertia = 140f;
			
			GenerateDust();

			if (foundTarget) {
				if (distanceFromTarget > 40f) {
					// If not in "strike-through" mode, home in
					if (Projectile.ai[1] == 0) {
						Vector2 direction = targetCenter - Projectile.Center;
						direction.Normalize();
						direction *= speed;

						float targetAngle = Projectile.AngleTo(targetCenter * MathHelper.ToRadians(360));
						Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;

						// If close enough, enter "strike-through" mode
						if (distanceFromTarget < 50f) {
							SoundEngine.PlaySound(SoundID.Item66);
							Projectile.ai[1] = 1; // Enter strike-through phase
							Projectile.ai[0] = 0; // Reset timer
						}
						Projectile.rotation = targetAngle;
					}
				}
			}

			// If in "strike-through" mode, keep moving forward without changing direction
			if (Projectile.ai[1] == 1) {
				Projectile.ai[0]++; // Increment timer

				if (Projectile.ai[0] < 20) {
					// Keep moving in the same direction for a bit
					Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed;
				} else {
					// Exit "strike-through" mode after 20 ticks (~1/3 of a second)
					Projectile.ai[1] = 0;
				}
			}

			if (!foundTarget) {
				// Reset "strike-through" state when there's no target
				Projectile.ai[1] = 0;

				if (distanceToIdlePosition > 600f) {
					speed = 12f;
					inertia = 60f;
				}
				else {
					speed = 4f;
					inertia = 80f;
				}

				if (distanceToIdlePosition > 20f) {
					vectorToIdlePosition.Normalize();
					vectorToIdlePosition *= speed;
					Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
				}
				else if (Projectile.velocity == Vector2.Zero) {
					Projectile.velocity.X = -0.15f;
					Projectile.velocity.Y = -0.05f;
				}
			}
		}


		private void Visuals() {
			// So it will lean slightly towards the direction it's moving
			Projectile.rotation = Projectile.velocity.X * 0.5f;

			GenerateDust();

			// This is a simple "loop through all frames from top to bottom" animation
			//int frameSpeed = 5;

			//Projectile.frameCounter++;

			//if (Projectile.frameCounter >= frameSpeed) {
				//Projectile.frameCounter = 0;
				//Projectile.frame++;

				//if (Projectile.frame >= Main.projFrames[Projectile.type]) {
					//Projectile.frame = 0;
				//}
			//}

			// Some visuals here
			Lighting.AddLight(Projectile.Center, new Color(80, 80, 80).ToVector3() * 0.78f);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
			{
				if (calamityMod.TryFind("ArmorCrunch", out ModBuff AC))
				{
					target.AddBuff(AC.Type, 120);
				}
			}
		}
	}
	public class GreaterDefiler : ModProjectile
	{
		public override string Texture => "FranciumCalamityWeapons/Content/Projectiles/CosmicScabbard/GreaterDefiler";
		private void GenerateDust()
		{
			
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, DustID.Lead,
					0, 0, 254, Scale: 1.0f);
				dust.velocity += Projectile.velocity * 0.5f;
				dust.velocity *= 0.5f;
				dust.noGravity = true;
		
		}

		public override void SetStaticDefaults() {
			ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

			Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion

			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
		}

		public sealed override void SetDefaults() {
			Projectile.width = 84;
			Projectile.height = 84;
			Projectile.tileCollide = false; // Makes the minion go through tiles freely

			// These below are needed for a minion weapon
			Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.minion = true; // Declares this as a minion (has many effects)
			Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
			Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
			Projectile.extraUpdates = 2;
			
		}

		public override bool PreDraw(ref Color lightColor) {
			// Draws an afterimage trail. See https://github.com/tModLoader/tModLoader/wiki/Basic-Projectile#afterimage-trail for more information.

			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = Projectile.oldPos.Length - 1; k > 0; k--) {
				Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}

			return true;
		}


		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles() {
			return false;
		}

		// This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
		public override bool MinionContactDamage() {
			return true;
		}

		// The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
		public override void AI() {

			
			GenerateDust();
			
			Player owner = Main.player[Projectile.owner];

			
			if (!CheckActive(owner)) {
				return;
			}

			GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
			SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
			Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
			Visuals();
		}

		// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
		private bool CheckActive(Player owner) {
			if (owner.dead || !owner.active) {
				owner.ClearBuff(ModContent.BuffType<CosmicScabbardBuff>());

				return false;
			}

			if (owner.HasBuff(ModContent.BuffType<CosmicScabbardBuff>())) {
				Projectile.timeLeft = 2;
			}

            
			return true;
		}



		
		private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition) {
			
			GenerateDust();

			Vector2 idlePosition = owner.Center;
			idlePosition.Y -= 48f; // Go up 48 coordinates (three tiles from the center of the player)

			
			// If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
			// The index is projectile.minionPos
			float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
			idlePosition.X += minionPositionOffsetX; // Go behind the player

			// All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

			// Teleport to player if distance is too big
			vectorToIdlePosition = idlePosition - Projectile.Center;
			distanceToIdlePosition = vectorToIdlePosition.Length();

			if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 1500f) {
				SoundStyle Warp = new SoundStyle("FranciumCalamityWeapons/Audio/ExobladeBeamSlash");
				SoundEngine.PlaySound(Warp);
				// Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
				// and then set netUpdate to true
				Projectile.position = idlePosition;
				Projectile.velocity *= 0.1f;
				Projectile.netUpdate = true;
				ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.BlackLightningHit,
				new ParticleOrchestraSettings { PositionInWorld = Projectile.Center },
				Projectile.owner);
			}

			// If your minion is flying, you want to do this independently of any conditions
			float overlapVelocity = 0.04f;

			// Fix overlap with other minions
			foreach (var other in Main.ActiveProjectiles) {
				if (other.whoAmI != Projectile.whoAmI && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width) {
					if (Projectile.position.X < other.position.X) {
						Projectile.velocity.X -= overlapVelocity;
					}
					else {
						Projectile.velocity.X += overlapVelocity;
					}

					if (Projectile.position.Y < other.position.Y) {
						Projectile.velocity.Y -= overlapVelocity;
					}
					else {
						Projectile.velocity.Y += overlapVelocity;
					}
				}
			}
		}

		private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter) {
			// Starting search distance
			distanceFromTarget = 700f;
			targetCenter = Projectile.position;
			foundTarget = false;

			GenerateDust();
			
			// This code is required if your minion weapon has the targeting feature
			if (owner.HasMinionAttackTargetNPC) {
				NPC npc = Main.npc[owner.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, Projectile.Center);

				// Reasonable distance away so it doesn't target across multiple screens
				if (between < 2000f) {
					distanceFromTarget = between;
					targetCenter = npc.Center;
					foundTarget = true;
				}
			}

			if (!foundTarget) {
				// This code is required either way, used for finding a target
				foreach (var npc in Main.ActiveNPCs) {
					if (npc.CanBeChasedBy()) {
						float between = Vector2.Distance(npc.Center, Projectile.Center);
						bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
						bool inRange = between < distanceFromTarget;
						bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
						// Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
						// The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
						bool closeThroughWall = between < 100f;

						if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall)) {
							distanceFromTarget = between;
							targetCenter = npc.Center;
							foundTarget = true;
						}
					}
				}
			}
			
			// friendly needs to be set to true so the minion can deal contact damage
			// friendly needs to be set to false so it doesn't damage things like target dummies while idling
			// Both things depend on if it has a target or not, so it's just one assignment here
			// You don't need this assignment if your minion is shooting things instead of dealing contact damage
			Projectile.friendly = foundTarget;
		}

		private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition) {
			float speed = 50f;
			float inertia = 140f;
			
			GenerateDust();

			if (foundTarget) {
				if (distanceFromTarget > 40f) {
					// If not in "strike-through" mode, home in
					if (Projectile.ai[1] == 0) {
						Vector2 direction = targetCenter - Projectile.Center;
						direction.Normalize();
						direction *= speed;

						float targetAngle = Projectile.AngleTo(targetCenter * MathHelper.ToRadians(360));
						Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;

						// If close enough, enter "strike-through" mode
						if (distanceFromTarget < 50f) {
							SoundEngine.PlaySound(SoundID.Item66);
							Projectile.ai[1] = 1; // Enter strike-through phase
							Projectile.ai[0] = 0; // Reset timer
						}
						Projectile.rotation = targetAngle;
					}
				}
			}

			// If in "strike-through" mode, keep moving forward without changing direction
			if (Projectile.ai[1] == 1) {
				Projectile.ai[0]++; // Increment timer

				if (Projectile.ai[0] < 20) {
					// Keep moving in the same direction for a bit
					Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed;
				} else {
					// Exit "strike-through" mode after 20 ticks (~1/3 of a second)
					Projectile.ai[1] = 0;
				}
			}

			if (!foundTarget) {
				// Reset "strike-through" state when there's no target
				Projectile.ai[1] = 0;

				if (distanceToIdlePosition > 600f) {
					speed = 12f;
					inertia = 60f;
				}
				else {
					speed = 4f;
					inertia = 80f;
				}

				if (distanceToIdlePosition > 20f) {
					vectorToIdlePosition.Normalize();
					vectorToIdlePosition *= speed;
					Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
				}
				else if (Projectile.velocity == Vector2.Zero) {
					Projectile.velocity.X = -0.15f;
					Projectile.velocity.Y = -0.05f;
				}
			}
		}


		private void Visuals() {
			// So it will lean slightly towards the direction it's moving
			Projectile.rotation = Projectile.velocity.X * 0.5f;

			GenerateDust();

			// This is a simple "loop through all frames from top to bottom" animation
			//int frameSpeed = 5;

			//Projectile.frameCounter++;

			//if (Projectile.frameCounter >= frameSpeed) {
				//Projectile.frameCounter = 0;
				//Projectile.frame++;

				//if (Projectile.frame >= Main.projFrames[Projectile.type]) {
					//Projectile.frame = 0;
				//}
			//}

			// Some visuals here
			// Define two colors to cycle between for the stroke
                Color lightColor1 = new Color(203, 219, 252);
                Color lightColor2 = new Color(69, 40, 60);

                // Use a sine wave to smoothly transition between the two colors
                float lerpAmount = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)));
                Color LightColor = Color.Lerp(lightColor1, lightColor2, lerpAmount);
			Lighting.AddLight(Projectile.Center, LightColor.ToVector3() * 0.78f);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
			{
				if (calamityMod.TryFind("ArmorCrunch", out ModBuff AC))
				{
					target.AddBuff(AC.Type, 360);
				}
			}
		}
	}
	public class NebulousShock : ModProjectile
	{
		public override string Texture => "FranciumCalamityWeapons/Content/Projectiles/CosmicScabbard/NebulousShock";
		
		public override void SetStaticDefaults() {
			ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

			Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion

			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
		}

		public sealed override void SetDefaults() {
			Projectile.width = 60;
			Projectile.height = 60;
			Projectile.tileCollide = false; // Makes the minion go through tiles freely

			// These below are needed for a minion weapon
			Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.minion = true; // Declares this as a minion (has many effects)
			Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
			Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
			Projectile.extraUpdates = 2;
		}

		private void GenerateDust()
		{
			
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, DustID.Lead,
					0, 0, 254, Scale: 1.0f);
				dust.velocity += Projectile.velocity * 0.5f;
				dust.velocity *= 0.5f;
				dust.noGravity = true;
		
		}



		public override bool PreDraw(ref Color lightColor) {
			// Draws an afterimage trail. See https://github.com/tModLoader/tModLoader/wiki/Basic-Projectile#afterimage-trail for more information.

			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = Projectile.oldPos.Length - 1; k > 0; k--) {
				Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}

			return true;
		}


		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles() {
			return false;
		}

		// This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
		public override bool MinionContactDamage() {
			return true;
		}

		// The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
		public override void AI() {

			
			GenerateDust();
			
			Player owner = Main.player[Projectile.owner];

			
			if (!CheckActive(owner)) {
				return;
			}

			GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
			SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
			Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
			Visuals();
		}

		// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
		private bool CheckActive(Player owner) {
			if (owner.dead || !owner.active) {
				owner.ClearBuff(ModContent.BuffType<CosmicScabbardBuff>());

				return false;
			}

			if (owner.HasBuff(ModContent.BuffType<CosmicScabbardBuff>())) {
				Projectile.timeLeft = 2;
			}

            
			return true;
		}



		
		private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition) {
			
			GenerateDust();

			Vector2 idlePosition = owner.Center;
			idlePosition.Y -= 48f; // Go up 48 coordinates (three tiles from the center of the player)

			
			// If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
			// The index is projectile.minionPos
			float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
			idlePosition.X += minionPositionOffsetX; // Go behind the player

			// All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

			// Teleport to player if distance is too big
			vectorToIdlePosition = idlePosition - Projectile.Center;
			distanceToIdlePosition = vectorToIdlePosition.Length();

			if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 1500f) {
				SoundStyle Warp = new SoundStyle("FranciumCalamityWeapons/Audio/ExobladeBeamSlash");
				SoundEngine.PlaySound(Warp);
				// Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
				// and then set netUpdate to true
				Projectile.position = idlePosition;
				Projectile.velocity *= 0.1f;
				Projectile.netUpdate = true;
				ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.BlackLightningHit,
				new ParticleOrchestraSettings { PositionInWorld = Projectile.Center },
				Projectile.owner);
			}

			// If your minion is flying, you want to do this independently of any conditions
			float overlapVelocity = 0.04f;

			// Fix overlap with other minions
			foreach (var other in Main.ActiveProjectiles) {
				if (other.whoAmI != Projectile.whoAmI && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width) {
					if (Projectile.position.X < other.position.X) {
						Projectile.velocity.X -= overlapVelocity;
					}
					else {
						Projectile.velocity.X += overlapVelocity;
					}

					if (Projectile.position.Y < other.position.Y) {
						Projectile.velocity.Y -= overlapVelocity;
					}
					else {
						Projectile.velocity.Y += overlapVelocity;
					}
				}
			}
		}

		private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter) {
			// Starting search distance
			distanceFromTarget = 700f;
			targetCenter = Projectile.position;
			foundTarget = false;

			GenerateDust();
			
			// This code is required if your minion weapon has the targeting feature
			if (owner.HasMinionAttackTargetNPC) {
				NPC npc = Main.npc[owner.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, Projectile.Center);

				// Reasonable distance away so it doesn't target across multiple screens
				if (between < 2000f) {
					distanceFromTarget = between;
					targetCenter = npc.Center;
					foundTarget = true;
				}
			}

			if (!foundTarget) {
				// This code is required either way, used for finding a target
				foreach (var npc in Main.ActiveNPCs) {
					if (npc.CanBeChasedBy()) {
						float between = Vector2.Distance(npc.Center, Projectile.Center);
						bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
						bool inRange = between < distanceFromTarget;
						bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
						// Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
						// The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
						bool closeThroughWall = between < 100f;

						if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall)) {
							distanceFromTarget = between;
							targetCenter = npc.Center;
							foundTarget = true;
						}
					}
				}
			}
			
			// friendly needs to be set to true so the minion can deal contact damage
			// friendly needs to be set to false so it doesn't damage things like target dummies while idling
			// Both things depend on if it has a target or not, so it's just one assignment here
			// You don't need this assignment if your minion is shooting things instead of dealing contact damage
			Projectile.friendly = foundTarget;
		}

		private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition) {
			float speed = 50f;
			float inertia = 140f;
			
			GenerateDust();

			if (foundTarget) {
				if (distanceFromTarget > 40f) {
					// If not in "strike-through" mode, home in
					if (Projectile.ai[1] == 0) {
						Vector2 direction = targetCenter - Projectile.Center;
						direction.Normalize();
						direction *= speed;

						float targetAngle = Projectile.AngleTo(targetCenter * MathHelper.ToRadians(360));
						Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;

						// If close enough, enter "strike-through" mode
						if (distanceFromTarget < 50f) {
							SoundEngine.PlaySound(SoundID.Item66);
							Projectile.ai[1] = 1; // Enter strike-through phase
							Projectile.ai[0] = 0; // Reset timer
						}
						Projectile.rotation = targetAngle;
					}
				}
			}

			// If in "strike-through" mode, keep moving forward without changing direction
			if (Projectile.ai[1] == 1) {
				Projectile.ai[0]++; // Increment timer

				if (Projectile.ai[0] < 20) {
					// Keep moving in the same direction for a bit
					Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed;
				} else {
					// Exit "strike-through" mode after 20 ticks (~1/3 of a second)
					Projectile.ai[1] = 0;
				}
			}

			if (!foundTarget) {
				// Reset "strike-through" state when there's no target
				Projectile.ai[1] = 0;

				if (distanceToIdlePosition > 600f) {
					speed = 12f;
					inertia = 60f;
				}
				else {
					speed = 4f;
					inertia = 80f;
				}

				if (distanceToIdlePosition > 20f) {
					vectorToIdlePosition.Normalize();
					vectorToIdlePosition *= speed;
					Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
				}
				else if (Projectile.velocity == Vector2.Zero) {
					Projectile.velocity.X = -0.15f;
					Projectile.velocity.Y = -0.05f;
				}
			}
		}


		private void Visuals() {
			// So it will lean slightly towards the direction it's moving
			Projectile.rotation = Projectile.velocity.X * 0.5f;

			GenerateDust();

			// This is a simple "loop through all frames from top to bottom" animation
			//int frameSpeed = 5;

			//Projectile.frameCounter++;

			//if (Projectile.frameCounter >= frameSpeed) {
				//Projectile.frameCounter = 0;
				//Projectile.frame++;

				//if (Projectile.frame >= Main.projFrames[Projectile.type]) {
					//Projectile.frame = 0;
				//}
			//}

			// Some visuals here
			// Define two colors to cycle between for the stroke
                Color lightColor1 = new Color(203, 219, 252);
                Color lightColor2 = new Color(193, 239, 255);

                // Use a sine wave to smoothly transition between the two colors
                float lerpAmount = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)));
                Color LightColor = Color.Lerp(lightColor1, lightColor2, lerpAmount);
			Lighting.AddLight(Projectile.Center, LightColor.ToVector3() * 0.78f);
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
			{
				if (calamityMod.TryFind("GlacialState", out ModBuff GS))
				{
					target.AddBuff(GS.Type, 360);
				}
			}
		}
	}
	public class WhiteDwarf : ModProjectile
	{
		public override string Texture => "FranciumCalamityWeapons/Content/Projectiles/CosmicScabbard/WhiteDwarf";
		private void GenerateDust()
		{
			
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, DustID.Gold,
					0, 0, 254, Scale: 1.0f);
				dust.velocity += Projectile.velocity * 0.5f;
				dust.velocity *= 0.5f;
				dust.noGravity = true;
		
		}

		public override void SetStaticDefaults() {
			ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

			Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion

			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
		}

		public sealed override void SetDefaults() {
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.tileCollide = false; // Makes the minion go through tiles freely

			// These below are needed for a minion weapon
			Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.minion = true; // Declares this as a minion (has many effects)
			Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
			Projectile.minionSlots = 1f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
			Projectile.extraUpdates = 2;
		}

		public override bool PreDraw(ref Color lightColor) {
			// Draws an afterimage trail. See https://github.com/tModLoader/tModLoader/wiki/Basic-Projectile#afterimage-trail for more information.

			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = Projectile.oldPos.Length - 1; k > 0; k--) {
				Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}

			return true;
		}



		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles() {
			return false;
		}

		// This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
		public override bool MinionContactDamage() {
			return true;
		}

		// The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
		public override void AI() {

			
			GenerateDust();
			
			Player owner = Main.player[Projectile.owner];

			
			if (!CheckActive(owner)) {
				return;
			}

			GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
			SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
			Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
			Visuals();
		}

		// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
		private bool CheckActive(Player owner) {
			if (owner.dead || !owner.active) {
				owner.ClearBuff(ModContent.BuffType<CosmicScabbardBuff>());

				return false;
			}

			if (owner.HasBuff(ModContent.BuffType<CosmicScabbardBuff>())) {
				Projectile.timeLeft = 2;
			}

            
			return true;
		}



		
		private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition) {
			
			GenerateDust();

			Vector2 idlePosition = owner.Center;
			idlePosition.Y -= 48f; // Go up 48 coordinates (three tiles from the center of the player)

			
			// If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
			// The index is projectile.minionPos
			float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
			idlePosition.X += minionPositionOffsetX; // Go behind the player

			// All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

			// Teleport to player if distance is too big
			vectorToIdlePosition = idlePosition - Projectile.Center;
			distanceToIdlePosition = vectorToIdlePosition.Length();

			if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 1500f) {
				SoundStyle Warp = new SoundStyle("FranciumCalamityWeapons/Audio/ExobladeBeamSlash");
				SoundEngine.PlaySound(Warp);
				// Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
				// and then set netUpdate to true
				Projectile.position = idlePosition;
				Projectile.velocity *= 0.1f;
				Projectile.netUpdate = true;
				ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.BlackLightningHit,
				new ParticleOrchestraSettings { PositionInWorld = Projectile.Center },
				Projectile.owner);
			}

			// If your minion is flying, you want to do this independently of any conditions
			float overlapVelocity = 0.04f;

			// Fix overlap with other minions
			foreach (var other in Main.ActiveProjectiles) {
				if (other.whoAmI != Projectile.whoAmI && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width) {
					if (Projectile.position.X < other.position.X) {
						Projectile.velocity.X -= overlapVelocity;
					}
					else {
						Projectile.velocity.X += overlapVelocity;
					}

					if (Projectile.position.Y < other.position.Y) {
						Projectile.velocity.Y -= overlapVelocity;
					}
					else {
						Projectile.velocity.Y += overlapVelocity;
					}
				}
			}
		}

		private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter) {
			// Starting search distance
			distanceFromTarget = 700f;
			targetCenter = Projectile.position;
			foundTarget = false;

			GenerateDust();
			
			// This code is required if your minion weapon has the targeting feature
			if (owner.HasMinionAttackTargetNPC) {
				NPC npc = Main.npc[owner.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, Projectile.Center);

				// Reasonable distance away so it doesn't target across multiple screens
				if (between < 2000f) {
					distanceFromTarget = between;
					targetCenter = npc.Center;
					foundTarget = true;
				}
			}

			if (!foundTarget) {
				// This code is required either way, used for finding a target
				foreach (var npc in Main.ActiveNPCs) {
					if (npc.CanBeChasedBy()) {
						float between = Vector2.Distance(npc.Center, Projectile.Center);
						bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
						bool inRange = between < distanceFromTarget;
						bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
						// Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
						// The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
						bool closeThroughWall = between < 100f;

						if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall)) {
							distanceFromTarget = between;
							targetCenter = npc.Center;
							foundTarget = true;
						}
					}
				}
			}
			
			// friendly needs to be set to true so the minion can deal contact damage
			// friendly needs to be set to false so it doesn't damage things like target dummies while idling
			// Both things depend on if it has a target or not, so it's just one assignment here
			// You don't need this assignment if your minion is shooting things instead of dealing contact damage
			Projectile.friendly = foundTarget;
		}

		private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition) {
			float speed = 50f;
			float inertia = 140f;
			
			GenerateDust();

			if (foundTarget) {
				if (distanceFromTarget > 40f) {
					// If not in "strike-through" mode, home in
					if (Projectile.ai[1] == 0) {
						Vector2 direction = targetCenter - Projectile.Center;
						direction.Normalize();
						direction *= speed;

						float targetAngle = Projectile.AngleTo(targetCenter * MathHelper.ToRadians(360));
						Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;

						// If close enough, enter "strike-through" mode
						if (distanceFromTarget < 50f) {
							SoundEngine.PlaySound(SoundID.Item66);
							Projectile.ai[1] = 1; // Enter strike-through phase
							Projectile.ai[0] = 0; // Reset timer
						}
						Projectile.rotation = targetAngle;
					}
				}
			}

			// If in "strike-through" mode, keep moving forward without changing direction
			if (Projectile.ai[1] == 1) {
				Projectile.ai[0]++; // Increment timer

				if (Projectile.ai[0] < 20) {
					// Keep moving in the same direction for a bit
					Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed;
				} else {
					// Exit "strike-through" mode after 20 ticks (~1/3 of a second)
					Projectile.ai[1] = 0;
				}
			}

			if (!foundTarget) {
				// Reset "strike-through" state when there's no target
				Projectile.ai[1] = 0;

				if (distanceToIdlePosition > 600f) {
					speed = 12f;
					inertia = 60f;
				}
				else {
					speed = 4f;
					inertia = 80f;
				}

				if (distanceToIdlePosition > 20f) {
					vectorToIdlePosition.Normalize();
					vectorToIdlePosition *= speed;
					Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
				}
				else if (Projectile.velocity == Vector2.Zero) {
					Projectile.velocity.X = -0.15f;
					Projectile.velocity.Y = -0.05f;
				}
			}
		}


		private void Visuals() {
			// So it will lean slightly towards the direction it's moving
			Projectile.rotation = Projectile.velocity.X * 0.5f;

			GenerateDust();

			// This is a simple "loop through all frames from top to bottom" animation
			//int frameSpeed = 5;

			//Projectile.frameCounter++;

			//if (Projectile.frameCounter >= frameSpeed) {
				//Projectile.frameCounter = 0;
				//Projectile.frame++;

				//if (Projectile.frame >= Main.projFrames[Projectile.type]) {
					//Projectile.frame = 0;
				//}
			//}

			// Some visuals here
			Lighting.AddLight(Projectile.Center, new Color(203, 219, 252).ToVector3() * 0.78f);
		}
	}
	public class Overlord : ModProjectile

	{
		public override string Texture => "FranciumCalamityWeapons/Content/Projectiles/CosmicScabbard/Overlord";
		private void GenerateDust()
		{
			Color smokeColor1 = new Color(243, 82, 255);
                Color smokeColor2 = new Color(90, 183, 255);

                // Use a sine wave to smoothly transition between the two colors
                float lerpAmount = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)));
                Color SmokeColor = Color.Lerp(smokeColor1, smokeColor2, lerpAmount);
				if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
			{
				if (calamityMod.TryFind("HeavySmokeParticle", out ModDust HSP))
				{
					Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.height, Projectile.width, HSP.Type, 255, 255, 255, Scale: 1.0f);
					dust.velocity += Projectile.velocity * 0.5f;
					dust.velocity *= 0.5f;
					dust.noGravity = true;
				}
			}

		
		}

		public override void SetStaticDefaults() {
			ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
			// This is necessary for right-click targeting
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

			Main.projPet[Projectile.type] = true; // Denotes that this projectile is a pet or minion

			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
		}

		public sealed override void SetDefaults() {
			Projectile.width = 78;
			Projectile.height = 78;
			Projectile.tileCollide = false; // Makes the minion go through tiles freely

			// These below are needed for a minion weapon
			Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.minion = true; // Declares this as a minion (has many effects)
			Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
			Projectile.minionSlots = 2f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
			Projectile.knockBack = 28;
			Projectile.extraUpdates = 2;
			
		}

		public override bool PreDraw(ref Color lightColor) {
			// Draws an afterimage trail. See https://github.com/tModLoader/tModLoader/wiki/Basic-Projectile#afterimage-trail for more information.

			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = Projectile.oldPos.Length - 1; k > 0; k--) {
				Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
			}

			return true;
		}


		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles() {
			return false;
		}

		// This is mandatory if your minion deals contact damage (further related stuff in AI() in the Movement region)
		public override bool MinionContactDamage() {
			return true;
		}

		// The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
		public override void AI() {

			
			GenerateDust();
			
			Player owner = Main.player[Projectile.owner];

			
			if (!CheckActive(owner)) {
				return;
			}

			GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
			SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
			Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
			Visuals();
		}

		// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
		private bool CheckActive(Player owner) {
			if (owner.dead || !owner.active) {
				owner.ClearBuff(ModContent.BuffType<CosmicScabbardBuff>());

				return false;
			}

			if (owner.HasBuff(ModContent.BuffType<CosmicScabbardBuff>())) {
				Projectile.timeLeft = 2;
			}

            
			return true;
		}



		
		private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition) {
			
			GenerateDust();

			Vector2 idlePosition = owner.Center;
			idlePosition.Y -= 48f; // Go up 48 coordinates (three tiles from the center of the player)

			
			// If your minion doesn't aimlessly move around when it's idle, you need to "put" it into the line of other summoned minions
			// The index is projectile.minionPos
			float minionPositionOffsetX = (10 + Projectile.minionPos * 40) * -owner.direction;
			idlePosition.X += minionPositionOffsetX; // Go behind the player

			// All of this code below this line is adapted from Spazmamini code (ID 388, aiStyle 66)

			// Teleport to player if distance is too big
			vectorToIdlePosition = idlePosition - Projectile.Center;
			distanceToIdlePosition = vectorToIdlePosition.Length();

			if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 1500f) {
				SoundStyle Warp = new SoundStyle("FranciumCalamityWeapons/Audio/ExobladeBeamSlash");
				SoundEngine.PlaySound(Warp);
				// Whenever you deal with non-regular events that change the behavior or position drastically, make sure to only run the code on the owner of the projectile,
				// and then set netUpdate to true
				Projectile.position = idlePosition;
				Projectile.velocity *= 0.1f;
				Projectile.netUpdate = true;
				ParticleOrchestrator.RequestParticleSpawn(clientOnly: false, ParticleOrchestraType.BlackLightningHit,
				new ParticleOrchestraSettings { PositionInWorld = Projectile.Center },
				Projectile.owner);
			}

			// If your minion is flying, you want to do this independently of any conditions
			float overlapVelocity = 0.04f;

			// Fix overlap with other minions
			foreach (var other in Main.ActiveProjectiles) {
				if (other.whoAmI != Projectile.whoAmI && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width) {
					if (Projectile.position.X < other.position.X) {
						Projectile.velocity.X -= overlapVelocity;
					}
					else {
						Projectile.velocity.X += overlapVelocity;
					}

					if (Projectile.position.Y < other.position.Y) {
						Projectile.velocity.Y -= overlapVelocity;
					}
					else {
						Projectile.velocity.Y += overlapVelocity;
					}
				}
			}
		}

		private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter) {
			// Starting search distance
			distanceFromTarget = 700f;
			targetCenter = Projectile.position;
			foundTarget = false;

			GenerateDust();
			
			// This code is required if your minion weapon has the targeting feature
			if (owner.HasMinionAttackTargetNPC) {
				NPC npc = Main.npc[owner.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, Projectile.Center);

				// Reasonable distance away so it doesn't target across multiple screens
				if (between < 2000f) {
					distanceFromTarget = between;
					targetCenter = npc.Center;
					foundTarget = true;
				}
			}

			if (!foundTarget) {
				// This code is required either way, used for finding a target
				foreach (var npc in Main.ActiveNPCs) {
					if (npc.CanBeChasedBy()) {
						float between = Vector2.Distance(npc.Center, Projectile.Center);
						bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
						bool inRange = between < distanceFromTarget;
						bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
						// Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
						// The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
						bool closeThroughWall = between < 100f;

						if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall)) {
							distanceFromTarget = between;
							targetCenter = npc.Center;
							foundTarget = true;
						}
					}
				}
			}
			
			// friendly needs to be set to true so the minion can deal contact damage
			// friendly needs to be set to false so it doesn't damage things like target dummies while idling
			// Both things depend on if it has a target or not, so it's just one assignment here
			// You don't need this assignment if your minion is shooting things instead of dealing contact damage
			Projectile.friendly = foundTarget;
		}

		private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition) {
			float speed = 50f;
			float inertia = 140f;
			
			GenerateDust();

			if (foundTarget) {
				if (distanceFromTarget > 40f) {
					// If not in "strike-through" mode, home in
					if (Projectile.ai[1] == 0) {
						Vector2 direction = targetCenter - Projectile.Center;
						direction.Normalize();
						direction *= speed;

						float targetAngle = Projectile.AngleTo(targetCenter * MathHelper.ToRadians(360));
						Projectile.velocity = (Projectile.velocity * (inertia - 1) + direction) / inertia;

						// If close enough, enter "strike-through" mode
						if (distanceFromTarget < 50f) {
							SoundStyle Strike = new SoundStyle("FranciumCalamityWeapons/Audio/GalaxySmasherSmash");
							SoundEngine.PlaySound(Strike);
							Projectile.ai[1] = 1; // Enter strike-through phase
							Projectile.ai[0] = 0; // Reset timer
						}
						Projectile.rotation = targetAngle;
					}
				}
			}

			// If in "strike-through" mode, keep moving forward without changing direction
			if (Projectile.ai[1] == 1) {
				Projectile.ai[0]++; // Increment timer

				if (Projectile.ai[0] < 20) {
					// Keep moving in the same direction for a bit
					Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * speed;
				} else {
					// Exit "strike-through" mode after 20 ticks (~1/3 of a second)
					Projectile.ai[1] = 0;
				}
			}

			if (!foundTarget) {
				// Reset "strike-through" state when there's no target
				Projectile.ai[1] = 0;

				if (distanceToIdlePosition > 600f) {
					speed = 12f;
					inertia = 60f;
				}
				else {
					speed = 4f;
					inertia = 80f;
				}

				if (distanceToIdlePosition > 20f) {
					vectorToIdlePosition.Normalize();
					vectorToIdlePosition *= speed;
					Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
				}
				else if (Projectile.velocity == Vector2.Zero) {
					Projectile.velocity.X = -0.15f;
					Projectile.velocity.Y = -0.05f;
				}
			}
		}


		private void Visuals() {
			// So it will lean slightly towards the direction it's moving
			Projectile.rotation = Projectile.velocity.X * 0.5f;

			GenerateDust();

			// This is a simple "loop through all frames from top to bottom" animation
			//int frameSpeed = 5;

			//Projectile.frameCounter++;

			//if (Projectile.frameCounter >= frameSpeed) {
				//Projectile.frameCounter = 0;
				//Projectile.frame++;

				//if (Projectile.frame >= Main.projFrames[Projectile.type]) {
					//Projectile.frame = 0;
				//}
			//}

			// Some visuals here
			// Define two colors to cycle between for the stroke
                Color lightColor1 = new Color(255, 82, 194);
                Color lightColor2 = new Color(89, 230, 255);

                // Use a sine wave to smoothly transition between the two colors
                float lerpAmount = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)));
                Color LightColor = Color.Lerp(lightColor1, lightColor2, lerpAmount);
			Lighting.AddLight(Projectile.Center, LightColor.ToVector3() * 0.78f);
		}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
			{
				if (calamityMod.TryFind("GalvanicCorrosion", out ModBuff GC))
				{
					target.AddBuff(GC.Type, 120);
				}
			}
		}

	}

	public class NebulousWinter : ModProjectile
	{
		public float radius = 50f; // Orbit distance
		public float speed = 0.5f; // Rotation speed

		public int IceDebuffTime = 360;
		public override string Texture => "FranciumCalamityWeapons/Content/Projectiles/CosmicScabbard/NebulousWinter";
		public sealed override void SetDefaults() {
			Projectile.width = 60;
			Projectile.height = 60;
			Projectile.tileCollide = false; // Makes the minion go through tiles freely

			// These below are needed for a minion weapon
			Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)
			Projectile.minion = true; // Declares this as a minion (has many effects)
			Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage)
			Projectile.minionSlots = 0.01f; // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
			Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
		}

		private void GenerateDust()
		{
			// Adjust the hilt position based on the projectile's position
			Vector2 SpawnPosition = Projectile.Center;

			// Create the first dust
			Dust dust = Dust.NewDustPerfect(SpawnPosition, ModContent.DustType<BigSnowflake>(), Vector2.Zero, 120, Color.White, 1.5f);
			dust.velocity += Projectile.velocity * 0.05f;  // Dust inherits velocity from the projectile
			dust.velocity *= 0.5f;
			dust.noGravity = false; // No gravity applied to the dust

			// Create additional dust particles with a loop
			for (int i = 0; i < 5; i++)  // Adjust for more dust particles
			{
				Dust dust2 = Dust.NewDustPerfect(SpawnPosition, ModContent.DustType<Snowcloud>(), Vector2.Zero, 240, Color.White, 1.5f);
				dust2.alpha += 12;  // Adds transparency to the dust
				dust2.velocity += Projectile.velocity * 0.05f; // Dust inherits velocity from the projectile
				dust2.velocity *= 0.5f;  // Slows down the dust
				dust2.noGravity = true; // No gravity for the dust
			}
		}

        public override void AI()
		{
			float angle = Main.GameUpdateCount * speed; // Time-based rotation

			// Attempt to find the NebulousShock projectile
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile proj = Main.projectile[i];

				if (proj.active && proj.type == ModContent.ProjectileType<NebulousShock>()) 
				{
					Projectile.timeLeft = 600;
					Vector2 center = proj.Center; // Orbit around this projectile
					Vector2 offset = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * radius;

					Projectile.position = center + offset - new Vector2(Projectile.width / 2, Projectile.height / 2);
					GenerateDust();
					return; // Stop searching once we find it
				}
			}
		}

		public void IceEffects(Player player)
		{
			if (player.ZoneSnow)
					{
						IceDebuffTime *= 2;
						speed += 2.0f;
						radius += 20f;
					}
		}
		public void OnHitNPC(NPC target, Player player, NPC.HitInfo hit, int damageDone)
		{
			SoundStyle Freeze = new SoundStyle("FranciumCalamityWeapons/Audio/IceImpact");
			SoundEngine.PlaySound(Freeze);
			
			if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
			{
				if (calamityMod.TryFind("GlacialState", out ModBuff GS) && calamityMod.TryFind("CosmicFreeze", out ModBuff CF))
				{
					target.AddBuff(GS.Type, IceDebuffTime);
					target.AddBuff(BuffID.Frostburn, IceDebuffTime);
					player.AddBuff(CF.Type, 120);	
				}
			}
		}
	}


}