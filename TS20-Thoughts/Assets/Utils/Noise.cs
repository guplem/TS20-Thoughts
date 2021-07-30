using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    /// <summary>
    /// Generates a perlin noise map of a given width and height.
    /// </summary>
    /// <param name="widthResolution">Width of the NoiseMap</param>
    /// <param name="heightResolution">Height of the NoiseMap</param>
    /// <param name="seed"></param>
    /// <param name="scale">The scale of the noise. The bigger it is, the ToDo: Finish </param>
    /// <param name="octaves"></param>
    /// <param name="persistance"></param>
    /// <param name="lacunarity"></param>
    /// <returns>An 2D array of floats containing the perlin noise map.</returns>
    public static float[,] GenerateNoiseMap(int widthResolution, int heightResolution, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[widthResolution, heightResolution];
        RandomEssentials rng = new RandomEssentials(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rng.Next(-100000, +100000) + offset.x;
            float offsetY = rng.Next(-100000, +100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0f)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = widthResolution / 2f;
        float halfHeight = heightResolution / 2f;
        
        for (int y = 0; y < heightResolution; y++)
        {
            for (int x = 0; x < widthResolution; x++)
            {
                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0f;
                
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x-halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y-halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                    
                }
                if (noiseHeight > maxNoiseHeight)
                    maxNoiseHeight = noiseHeight;
                if (noiseHeight < minNoiseHeight)
                    minNoiseHeight = noiseHeight;
                
                noiseMap[x, y] = noiseHeight;

            }
        }

        //Normalization of the noise map
        for (int y = 0; y < heightResolution; y++)
        {
            for (int x = 0; x < widthResolution; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
