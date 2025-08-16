using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class WaveTrail
{
    private List<Vector2> positions = new List<Vector2>();
    private const int MaxPositions = 10;  // Max number of stored positions for the trail.
    private const float WaveAmplitude = 5f; // How tall the wave is.
    private const float WaveFrequency = 0.1f; // How wide the wave is.

    public void Update(Vector2 currentPosition)
    {
        // Add the new position to the list.
        positions.Add(currentPosition);

        // Remove old positions to keep the trail size constant.
        if (positions.Count > MaxPositions)
        {
            positions.RemoveAt(0);
        }
    }

    Texture2D whiteTexture;

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        whiteTexture = new Texture2D(graphicsDevice, 10, 10);
        whiteTexture.SetData(new Color[] { Color.White });
    }


    public void Draw(SpriteBatch sb)
    {
        if (positions.Count < 2) return; // Need at least 2 positions to draw a line.

        // Draw the trail as a series of lines between positions.
        for (int i = 0; i < positions.Count - 1; i++)
        {
            // Calculate the direction vector between the current and next position.
            Vector2 start = positions[i];
            Vector2 end = positions[i + 1];

            // Interpolate along the line to create the wave effect.
            int segmentCount = 10;  // How many intermediate points per segment.
            for (int j = 0; j < segmentCount; j++)
            {
                // Calculate interpolated point along the line segment.
                float t = j / (float)segmentCount;
                Vector2 point = Vector2.Lerp(start, end, t);

                // Apply a sine function to the Y value for the wave effect.
                float waveOffset = (float)Math.Sin(t * MathHelper.TwoPi * WaveFrequency) * WaveAmplitude;
                point.Y += waveOffset;

                // Draw a small rectangle at each point (simulating the trail).
                sb.Draw(whiteTexture, point, null, Color.White, 0f, Vector2.Zero, 10f, SpriteEffects.None, 0f);
            }
        }
    }
}
