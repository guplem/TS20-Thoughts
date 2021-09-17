using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class HeightMapGenerator
{
    //static float[,] falloffMap;

    [MenuItem("Thoughts/MapGenerator/Clear Falloff Map️", false, 0)]
    /*public static void ClearFalloffMap()
    {
        falloffMap = null;
        Debug.Log("Falloff map cleared (set to null)");
    }*/
    
    public static HeightMap GenerateHeightMap(int width, int height, float mapRadius, HeightMapSettings settings, Vector2 sampleCenter)
    {
        //Debug.Log($"Generating height map for {sampleCenter}");
        float[,] values = Noise.GenerateNoiseMap(width, height, settings.noiseSettings, sampleCenter);

        AnimationCurve heigtCurve_threadSafe = new AnimationCurve(settings.heightCurve.keys); // Accessing an AnimationCurve in multiple threads at the same time can lead to wrong evaluations. A copy is done to ensure evaluating it is safe. 
        AnimationCurve falloffIntensity_threadSafe = new AnimationCurve(settings.falloffIntensity.keys); // Accessing an AnimationCurve in multiple threads at the same time can lead to wrong evaluations. A copy is done to ensure evaluating it is safe. 

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;
        
        /*if (settings.useFalloff) {
            if (falloffMap == null) {
                falloffMap = FalloffGenerator.GenerateFalloffMap (width, settings.percentageOfMapWithoutMaxFalloff, mapRadius);
            }
        }*/
        
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float falloffValue = 0;
                if (settings.useFalloff)
                {
                    float coordX = sampleCenter.x - (width / 2f) + i;
                    float coordY = sampleCenter.y - (height / 2f) + (height-j); // to fix weird orientation of the falloff
                    falloffValue = GetFalloffValue(new Vector2(coordX, coordY), falloffIntensity_threadSafe, mapRadius);
                }
                
                values [i, j] *= heigtCurve_threadSafe.Evaluate (values [i, j] - falloffValue) * settings.heightMultiplier;

                if (values[i, j] > maxValue)
                    maxValue = values[i, j];
                if (values[i, j] < minValue)
                    minValue = values[i, j];
            }
        }
        return new HeightMap(values, minValue, maxValue);
    }

    private static float GetFalloffValue(Vector2 coords, AnimationCurve falloffIntensity, float mapRadius)
    {
        float distanceToCenter = Mathf.Sqrt(coords.x*coords.x + coords.y*coords.y);
        //float falloffValue = ((distanceToCenter*distanceToCenter)/(mapRadius*mapRadius*percentageOfMapWithoutMaxFalloff)); // Old method, with formula
        float normalizedDistanceToCenter = distanceToCenter / mapRadius;
        float falloffValue = falloffIntensity.Evaluate(normalizedDistanceToCenter);
        float value = Mathf.Clamp01(falloffValue);
        return value;
    }
}



public struct HeightMap
{
    public readonly float[,] values;
    public readonly float minValue;
    public readonly float maxValue;
    
    public HeightMap(float[,] values, float minValue, float maxValue)
    {
        //Debug.Log($"Generated Height map with minValue: {minValue} and maxValue: {maxValue}");
        this.values = values;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}