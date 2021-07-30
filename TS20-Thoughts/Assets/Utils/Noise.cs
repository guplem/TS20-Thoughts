using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    /// <summary>
    /// Generates a perlin noise map of a given width and height.
    /// </summary>
    /// <param name="width">Width of the NoiseMap</param>
    /// <param name="height">Height of the NoiseMap</param>
    /// <param name="scale">The scale of the noise. The bigger it is, the ToDo: Finish </param>
    /// <returns>An 2D array of floats containing the perlin noise map.</returns>
    public static float[,] GenerateNoiseMap(int width, int height, float scale)
    {
        float[,] noiseMap = new float[width, height];

        if (scale <= 0f)
        {
            scale = 0.0001f;
        }
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float sampleX = x / scale;
                float sampleY = y / scale;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                noiseMap[x, y] = perlinValue;
            }
        }
        
        return noiseMap;
    }
}
