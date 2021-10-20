using UnityEngine;


/// <summary>
/// Contains values relative to the height of a terrain
/// </summary>
public struct HeightMap
{
    /// <summary>
    /// The main data stored in the HeightMap (should be relative to the height of a terrain).
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
    /// Constructor of a HeightMap that contains values relative to the height of a terrain
    /// </summary>
    /// <param name="values">The main data stored in the HeightMap relative to the height of a terrain</param>
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
    /// Generates HeightMap that contains values relative to the height of a terrain
    /// </summary>
    /// <param name="width">The desired width for the generated HeightMap</param>
    /// <param name="height">The desired height for the generated HeightMap</param>
    /// <param name="mapRadius">The theoretical radius of the map containing this terrain</param>
    /// <param name="settings">The HeightMapSettings for this HeightMap's pattern</param>
    /// <param name="sampleCenter">The cords at the center of this HeightMap relative to the center of the whole (scene) map</param>
    /// <returns></returns>
    public static HeightMap GenerateHeightMap(int width, int height, float mapRadius, HeightMapSettings settings, Vector2 sampleCenter)
    {
        //Debug.Log($"Generating height map for {sampleCenter}");
        float[,] values = Noise.GenerateNoiseMap(width, height, settings.terrainNoiseMapSettings, sampleCenter);

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

    /// <summary>
    /// Returns a [0,1] value containing the intensity of the falloff at a given coords of the terrain 
    /// </summary>
    /// <param name="coords">The coords of the map to calculate the intensity of the falloff in that location</param>
    /// <param name="falloffIntensity">The AnimationCurve that defines the intensity of the falloff relative to the radius of the center of the map</param>
    /// <param name="mapRadius">The radius of the center of the map</param>
    /// <returns></returns>
    private static float GetFalloffValue(Vector2 coords, AnimationCurve falloffIntensity, float mapRadius)
    {
        float distanceToCenter = Mathf.Sqrt(coords.x*coords.x + coords.y*coords.y);
        //float falloffValue = ((distanceToCenter*distanceToCenter)/(mapRadius*mapRadius*percentageOfMapWithoutMaxFalloff)); // Old method, with formula
        float normalizedDistanceToCenter = distanceToCenter / mapRadius;
        float falloffValue = falloffIntensity.Evaluate(normalizedDistanceToCenter);
        return Mathf.Clamp01(falloffValue);
    }
    
}