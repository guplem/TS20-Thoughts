using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public enum NormalizeMode
    {
        /// <summary>
        /// Using the local minimum and maximum
        /// </summary>
        Local,
        /// <summary>
        /// Estimating the global minimum and maximum
        /// </summary>
        Global
        
    }
    
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
    public static float[,] GenerateNoiseMap(int width, int height, NoiseSettings settings, Vector2 sampleCenter /*int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormalizeMode normalizeMode = Noise.NormalizeMode.Global*/)
    {
        float[,] noiseMap = new float[width, height];
        RandomEssentials rng = new RandomEssentials(settings.seed);
        Vector2[] octaveOffsets = new Vector2[settings.octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1f;
        float frequency = 1f;
        
        for (int i = 0; i < settings.octaves; i++)
        {
            float offsetX = rng.Next(-100000, +100000) + settings.offset.x + sampleCenter.x;
            float offsetY = rng.Next(-100000, +100000) - settings.offset.y - sampleCenter.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= settings.persistance;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = width / 2f;
        float halfHeight = height / 2f;
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                amplitude = 1f;
                frequency = 1f;
                float noiseHeight = 0f;
                
                for (int i = 0; i < settings.octaves; i++)
                {
                    float sampleX = (x-halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                    float sampleY = (y-halfHeight + octaveOffsets[i].y) / settings.scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= settings.persistance;
                    frequency *= settings.lacunarity;
                    
                }
                if (noiseHeight > maxLocalNoiseHeight)
                    maxLocalNoiseHeight = noiseHeight;
                if (noiseHeight < minLocalNoiseHeight)
                    minLocalNoiseHeight = noiseHeight;
                
                noiseMap [x, y] = noiseHeight;

                if (settings.normalizeMode == NormalizeMode.Global) {
                    float normalizedHeight = (noiseMap [x, y] + 1) / (maxPossibleHeight / 0.9f);
                    noiseMap [x, y] = Mathf.Clamp (normalizedHeight, 0, int.MaxValue);
                }
            }
        }

        if (settings.normalizeMode == NormalizeMode.Local) 
        {
            for (int y = 0; y < height; y++) 
            {
                for (int x = 0; x < width; x++) 
                {
                    noiseMap [x, y] = Mathf.InverseLerp (minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap [x, y]);
                }
            }
        }

        return noiseMap;
    }
}

[System.Serializable]
public class NoiseSettings
{
    [Tooltip("I believe it should always be Global")]
    public Noise.NormalizeMode normalizeMode = Noise.NormalizeMode.Global;  // I believe it should always be Global
        
    public float scale = 50f;
    [Range(0,20)]
    public int octaves = 6;
    [Range(0,1)]
    public float persistance = 0.6f;
    public float lacunarity = 2f;

    [Space]
    public int seed;
    public Vector2 offset;
    
    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        persistance = Mathf.Clamp01(persistance);
        lacunarity = Mathf.Max(lacunarity, 1);
    }
}