using UnityEngine;

namespace Thoughts.Utils.Maths
{
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
        /// <returns>An 2D array of floats containing the perlin noise map.</returns>
        public static float[,] GenerateNoiseMap( int width, int height, NoiseMapSettings noiseSettings, Vector2 sampleCenter, int seed)
        {
            float[,] noiseMap = new float[width, height];
            RandomEssentials rng = new RandomEssentials(seed);
            Vector2[] octaveOffsets = new Vector2[noiseSettings.octaves];

            float maxPossibleHeight = 0;
            float amplitude = 1f;
            float frequency = 1f;
        
            for (int i = 0; i < noiseSettings.octaves; i++)
            {
                float offsetX = rng.Next(-100000, +100000) + noiseSettings.offset.x + sampleCenter.x;
                float offsetY = rng.Next(-100000, +100000) - noiseSettings.offset.y - sampleCenter.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);

                maxPossibleHeight += amplitude;
                amplitude *= noiseSettings.persistance;
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
                
                    for (int i = 0; i < noiseSettings.octaves; i++)
                    {
                        float sampleX = ( (x-halfWidth + octaveOffsets[i].x) / noiseSettings.scale * frequency) ;
                        float sampleY = ( (y-halfHeight + octaveOffsets[i].y) / noiseSettings.scale * frequency) ;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= noiseSettings.persistance;
                        frequency *= noiseSettings.lacunarity;
                    
                    }
                    if (noiseHeight > maxLocalNoiseHeight)
                        maxLocalNoiseHeight = noiseHeight;
                    if (noiseHeight < minLocalNoiseHeight)
                        minLocalNoiseHeight = noiseHeight;
                
                    noiseMap [x, y] = noiseHeight;

                    if (noiseSettings.normalizeMode == NormalizeMode.Global) {
                        float normalizedHeight = (noiseMap [x, y] + 1) / (maxPossibleHeight / 0.9f);
                        noiseMap [x, y] = Mathf.Clamp (normalizedHeight, 0, int.MaxValue);
                    }
                }
            }

            if (noiseSettings.normalizeMode == NormalizeMode.Local) 
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

    /// <summary>
    /// Collection of settings used to generate a noise map
    /// More info about the settings here: https://www.youtube.com/watch?v=wbpMiKiSKm8
    /// </summary>
    [System.Serializable]
    public class NoiseMapSettings
    {
        public Noise.NormalizeMode normalizeMode => Noise.NormalizeMode.Global;  // I believe it should always be Global. It was a public serialized variable before but the value never changed.
        
        /// <summary>
        /// The scale of the noise
        /// </summary>
        [Tooltip("The scale of the noise")]
        public float scale = 50f;

        /// <summary>
        /// The amount of curves/noises that will be computed and overlapped for the noise
        /// </summary>
        [Tooltip("The amount of curves/noises that will be computed and overlapped for the noise. Usually, more means more realistic but can lead to worse performance.")]
        [Range(0,20)]
        public int octaves = 4;

        /// <summary>
        /// The increase in frequency for each of the octaves used in the noise map
        /// </summary>
        [Tooltip("Determines how many small features exist in the noise map")]
        public float lacunarity = 2f;
        
        /// <summary>
        /// Reduces the amplitude for each of the octaves used in the noise map
        /// </summary>
        [Tooltip("How much the small features affect the over all noise map")]
        [Range(0,1)]
        public float persistance = 0.6f;

        /// <summary>
        /// Offset of the noise of the map
        /// </summary>
        [Space]
        [Tooltip("Offset of the noise of the map")]
        public Vector2 offset;

        public void ValidateValues()
        {
            scale = Mathf.Max(scale, 0.01f);
            octaves = Mathf.Max(octaves, 1);
            persistance = Mathf.Clamp01(persistance);
            lacunarity = Mathf.Max(lacunarity, 1);
        }
    }
}