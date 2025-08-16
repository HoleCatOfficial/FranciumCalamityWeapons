using Terraria;
using Terraria.ID;
using System;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.UI.Chat;
using FranciumCalamityWeapons.Common.Rarities;

namespace FranciumCalamityWeapons.Common.Items
{
    public class TooltipColors : GlobalItem
        {
            public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
            {

            if (item.rare == ModContent.RarityType<NewCosmicRarity>() && line.Name == "ItemName")
                        {
                            // Define two colors to cycle between for the stroke
                            Color strokeColor1 = new Color(243, 82, 255);
                            Color strokeColor2 = new Color(90, 183, 255);

                            Color textColor1 = new Color(0, 0, 0);
                            Color textColor2 = new Color(100, 100, 100);

                            // Use a sine wave to smoothly transition between the two colors
                            float lerpAmount = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)));
                            Color strokeColor = Color.Lerp(strokeColor1, strokeColor2, lerpAmount);

                            // Use a sine wave to smoothly transition between the two colors
                            float lerpAmount2 = (float)(0.5 * (1 + Math.Sin(Main.GlobalTimeWrappedHourly * 2f * Math.PI)));
                            Color textColor = Color.Lerp(textColor1, textColor2, lerpAmount2);

                            Vector2 YOffset = new Vector2(0, Main.rand.NextBool(10) ? Main.rand.Next(3, 6) : 0);
                            Vector2 XOffset = new Vector2(Main.rand.NextBool(10) ? Main.rand.Next(3, 6) : 0, 0);

                            // Extract the correct font reference
                            DynamicSpriteFont font = FontAssets.MouseText.Value;

                            // Draw the outline first by offsetting in all directions
                            Vector2 position = new Vector2(line.X, line.Y);
                            for (int i = -1; i <= 1; i++)
                            {
                                for (int j = -1; j <= 1; j++)
                                {
                                    if (i == 0 && j == 0) continue; // Skip center (main text)
                                    ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position + new Vector2(i, j) + YOffset + XOffset, strokeColor, 0f, Vector2.Zero, Vector2.One);
                                }
                            }

                            // Draw the actual text on top
                            ChatManager.DrawColorCodedString(Main.spriteBatch, font, line.Text, position, textColor, 0f, Vector2.Zero, Vector2.One);

                            return false; // Prevents Terraria from drawing the default text
                        }
                        return true;
            }
        }
    }