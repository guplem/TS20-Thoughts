using Thoughts.Utils.Maths;
using UnityEngine;

namespace Thoughts.Game.Map.CreationSteps.Terrain
{
    /// <summary>
    /// Contains values related to the height of a terrain
    /// </summary>
    public struct HeightMap
    {
        /// <summary>
        /// The main data stored in the HeightMap (should be related to the height of a terrain).
        /// </summary>
        public readonly float[,] values;
        /// <summary>
        /// The pre recorded minimum value contained within the main data.
        /// </summary>
        public readonly float minValue;
        /// <summary>
        /// The pre recorded maximum value contained within the main data.
        /// </summary>
        public readonly float maxValue;
    
        /// <summary>
        /// Constructor of a HeightMap that contains values related to the height of a terrain
        /// </summary>
        /// <param name="values">The main data stored in the HeightMap related to the height of a terrain</param>
        /// <param name="minValue">The pre calculated minimum value contained within the main data</param>
        /// <param name="maxValue">The pre calculated maximum value contained within the main data</param>
        private HeightMap(float[,] values, float minValue, float maxValue)
        {
            //Debug.Log($"Generated Height map with minValue: {minValue} and maxValue: {maxValue}");
            this.values = values;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        /// <summary>
        /// Generates HeightMap that contains values related to the height of a terrain
        /// </summary>
        /// <param name="width">The desired width for the generated HeightMap</param>
        /// <param name="height">The desired height for the generated HeightMap</param>
        /// <param name="mapRadius">The theoretical radius of the map containing this terrain</param>
        /// <param name="settings">The HeightMapSettings for this HeightMap's pattern</param>
        /// <param name="sampleCenter">The cords at the center of this HeightMap in absolute values (relative to the center of the center of the scene)</param>
        /// <param name="generalMapSeed">The seed used for the general map generation</param>
        /// <param name="freeFalloffAreaRadius">The area compared to the radius of the map in which the falloff will not be applied (starting from the center)</param>
        /// <param name="seaHight">The normalized [0,1] height at which the sea starts</param>
        /// <returns></returns>
        public static HeightMap GenerateHeightMap(int width, int height, float mapRadius, TerrainHeightSettings settings, Vector2 sampleCenter, int generalMapSeed, float freeFalloffAreaRadius, float seaHight)
        {
            int heightSeed = generalMapSeed + 546132;
            //Debug.Log($"Generating height map for {sampleCenter}");
            float[,] heightNoiseMap = Noise.GenerateNoiseMap(width, height, settings.noiseMapSettings, sampleCenter, heightSeed);
            int falloffSeed = generalMapSeed + 7;
            float[,] falloffNoiseMap = Noise.GenerateNoiseMap(width, height, settings.falloffNoiseMapSettings, sampleCenter, falloffSeed);

            AnimationCurve aboveWaterHeigtCurve_threadSafe = new AnimationCurve(settings.aboveWaterHeightCurve.keys); // Accessing an AnimationCurve in multiple threads at the same time can lead to wrong evaluations. A copy is done to ensure evaluating it is safe. 
            AnimationCurve underWaterHeigtCurve_threadSafe = new AnimationCurve(settings.underWaterHeightCurve.keys); // Accessing an AnimationCurve in multiple threads at the same time can lead to wrong evaluations. A copy is done to ensure evaluating it is safe. 
            AnimationCurve falloffIntensity_threadSafe = new AnimationCurve(settings.falloffIntensity.keys); // Accessing an AnimationCurve in multiple threads at the same time can lead to wrong evaluations. A copy is done to ensure evaluating it is safe. 

            
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    float falloffValue = 0;
                    if (settings.useFalloff)
                    {
                        float coordX = sampleCenter.x - (width / 2f) + i;
                        float coordY = sampleCenter.y - (height / 2f) + (height-j); // to fix weird orientation of the falloff
                        falloffValue = GetFalloffValue(new Vector2(coordX, coordY), falloffIntensity_threadSafe, mapRadius, freeFalloffAreaRadius, falloffNoiseMap[i, j]);
                    }

                    float original01Value = heightNoiseMap[i, j];
                    float curveMultiplication = -1;
                    if (original01Value >= seaHight)
                        curveMultiplication = seaHight + (1-seaHight) * aboveWaterHeigtCurve_threadSafe.Evaluate((original01Value-seaHight)/(1-seaHight));
                    else
                        curveMultiplication = underWaterHeigtCurve_threadSafe.Evaluate(original01Value/seaHight) * seaHight;
                    float heightValue = curveMultiplication * settings.heightMultiplier;
                    heightNoiseMap [i, j] = heightValue * (1-falloffValue);

                    if (heightNoiseMap[i, j] > maxValue)
                        maxValue = heightNoiseMap[i, j];
                    if (heightNoiseMap[i, j] < minValue)
                        minValue = heightNoiseMap[i, j];
                }
            }
            return new HeightMap(heightNoiseMap, minValue, maxValue);
        }

        /// <summary>
        /// Returns a [0,1] value containing the intensity of the falloff at a given coords of the terrain 
        /// </summary>
        /// <param name="coords">The coords of the map to calculate the intensity of the falloff in that location</param>
        /// <param name="falloffIntensity">The AnimationCurve that defines the intensity of the falloff relative to the radius of the map from its center</param>
        /// <param name="mapRadius">The radius of the center of the map</param>
        /// <param name="freeFalloffAreaRadius">An area in which the falloff will not be applied starting from the center</param>
        /// <param name="noiseInstensityAtCoords">The intensity of the falloff noise map at the given coords. Used to generate irregularities</param>
        /// <returns></returns>
        private static float GetFalloffValue(Vector2 coords, AnimationCurve falloffIntensity, float mapRadius, float freeFalloffAreaRadius, float noiseInstensityAtCoords)
        {
            float normalizedFreeFalloffAreaRadius = (mapRadius * freeFalloffAreaRadius);
            float distanceToCenter = Mathf.Sqrt(coords.x*coords.x + coords.y*coords.y);
            if (distanceToCenter < normalizedFreeFalloffAreaRadius)
                return 0;
            distanceToCenter -= normalizedFreeFalloffAreaRadius;
            //float falloffValue = ((distanceToCenter*distanceToCenter)/(mapRadius*mapRadius*percentageOfMapWithoutMaxFalloff)); // Old method, with formula
            float normalizedDistanceToCenter = distanceToCenter / (mapRadius-normalizedFreeFalloffAreaRadius);
            float falloffIntensityAtCoords = falloffIntensity.Evaluate(normalizedDistanceToCenter);
            float falloffValue = falloffIntensityAtCoords + noiseInstensityAtCoords * falloffIntensityAtCoords;
            return Mathf.Clamp01(falloffValue);
        }
    
    }
}