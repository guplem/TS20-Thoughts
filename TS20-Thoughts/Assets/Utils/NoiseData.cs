using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NoiseData", menuName = "Thoughts/Utils/NoiseData", order = 100)]
public class NoiseData : UpdatableData
{
    
    [Space]
    public float noiseScale = 27f;
    [Range(0,20)]
    public int octaves = 4;
    [Range(0,1)]
    public float persistance = 0.5f;
    public float lacunarity = 2f;  
        
    public Vector2 offset;

    #if UNITY_EDITOR
    
    protected override void OnValidate()
    {
        if (lacunarity < 1)
            lacunarity = 1;
        if (octaves < 0)
            octaves = 0;
        
        base.OnValidate();
    }
    
    #endif
}
