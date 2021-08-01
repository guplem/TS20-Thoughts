using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    /// <summary>
    /// Generates a perlin noise map of a given width and height.
    /// </summary>
    /// <param name="size">Width and Height of the noise map</param>
    /// <param name="seed"></param>
    /// <param name="scale">The scale of the noise. The bigger it is, the ToDo: Finish </param>
    /// <param name="octaves"></param>
    /// <param name="persistance"></param>
    /// <param name="lacunarity"></param>
    /// <param name="offset"></param>
    /// <returns>An 2D array of floats containing the perlin noise map.</returns>
    public static float[,] GenerateNoiseMap(int size, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[size, size];
        RandomEssentials rng = new RandomEssentials(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float offsetX = rng.Next(-100000, +100000) + offset.x;
            float offsetY = rng.Next(-100000, +100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0f)
        {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = size / 2f;
        float halfHeight = size / 2f;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0f;
                
                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x-halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y-halfHeight + octaveOffsets[i].y) / scale * frequency;

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
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
