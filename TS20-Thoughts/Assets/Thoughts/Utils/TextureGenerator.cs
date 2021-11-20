using Thoughts.Game.Map.CreationSteps.Terrain;
using UnityEngine;

namespace Thoughts.Utils
{
    public static class TextureGenerator
    {
        public static Texture2D TextureFromColorMap(Color[] colourMap, int width, int height)
        {
            Texture2D texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels(colourMap);
            texture.Apply();
            return texture;
        }

        public static Texture2D TextureFromHeightMap(float[,] heightMap) // All values in the 2D array heightMapAbsolute must be in the range [0,1]
        {
            int width = heightMap.GetLength(0);
            int height = heightMap.GetLength(1);

            Color[] colourMap = new Color[width * height]; // Be careful, it is 1D
        
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x,y]);
                }
            }

            return TextureFromColorMap(colourMap, width, height);
        }
    
        public static Texture2D TextureFromHeightMap(HeightMap heightMap)
        {
            int width = heightMap.values.GetLength(0);
            int height = heightMap.values.GetLength(1);

            Color[] colourMap = new Color[width * height]; // Be careful, it is 1D
        
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, Mathf.InverseLerp(heightMap.minValue, heightMap.maxValue, heightMap.values[x,y]) );
                }
            }

            return TextureFromColorMap(colourMap, width, height);
        }
    }
}
