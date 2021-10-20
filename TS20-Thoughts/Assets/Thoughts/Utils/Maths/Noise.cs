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
        public static float[,] GenerateNoiseMap( int width, int height, NoiseMapSettings mapSettings, Vector2 sampleCenter)
        {
            float[,] noiseMap = new float[width, height];
            RandomEssentials rng = new RandomEssentials(mapSettings.seed);
            Vector2[] octaveOffsets = new Vector2[mapSettings.octaves];

            float maxPossibleHeight = 0;
            float amplitude = 1f;
            float frequency = 1f;
        
            for (int i = 0; i < mapSettings.octaves; i++)
            {
                float offsetX = rng.Next(-100000, +100000) + mapSettings.offset.x + sampleCenter.x;
                float offsetY = rng.Next(-100000, +100000) - mapSettings.offset.y - sampleCenter.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);

                maxPossibleHeight += amplitude;
                amplitude *= mapSettings.persistance;
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
                
                    for (int i = 0; i < mapSettings.octaves; i++)
                    {
                        float sampleX = ( (x-halfWidth + octaveOffsets[i].x) / mapSettings.scale * frequency) ;
                        float sampleY = ( (y-halfHeight + octaveOffsets[i].y) / mapSettings.scale * frequency) ;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= mapSettings.persistance;
                        frequency *= mapSettings.lacunarity;
                    
                    }
                    if (noiseHeight > maxLocalNoiseHeight)
                        maxLocalNoiseHeight = noiseHeight;
                    if (noiseHeight < minLocalNoiseHeight)
                        minLocalNoiseHeight = noiseHeight;
                
                    noiseMap [x, y] = noiseHeight;

                    if (mapSettings.normalizeMode == NormalizeMode.Global) {
                        float normalizedHeight = (noiseMap [x, y] + 1) / (maxPossibleHeight / 0.9f);
                        noiseMap [x, y] = Mathf.Clamp (normalizedHeight, 0, int.MaxValue);
                    }
                }
            }

            if (mapSettings.normalizeMode == NormalizeMode.Local) 
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
    /// </summary>
    [System.Serializable]
    public class NoiseMapSettings
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
}