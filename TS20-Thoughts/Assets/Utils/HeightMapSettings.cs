using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "HeightMapSettings", menuName = "Thoughts/Map/Terrain/HeightMapSettings", order = 100)]
public class HeightMapSettings : UpdatableData
{
    public NoiseSettings noiseSettings;
    public const int numSupportedTerrainLODs = 5;
    public Vector2 offset = Vector2.zero;
    
    [Space]
    [SerializeField] public bool useFalloff;
    //[Range(0.1f, 1f)]
    //[SerializeField] public float percentageOfMapWithoutMaxFalloff = 1;
    [SerializeField] public AnimationCurve falloffIntensity;
    
    /// <summary>
    /// For how much each cell height will be multiplied.
    /// <para>The height of the cell is by default [0,1], multiplying it by 5, the maximum height will be 5 (cell height [0,5]</para>
    /// </summary>
    [FormerlySerializedAs("maxHeight")]
    public float heightMultiplier = 15f;
    /// <summary>
    /// How much the height of the mesh should be affected by the maxHeight (AKA: "height multiplier")
    /// </summary>
    public AnimationCurve heightCurve;
    
    /// <summary>
    /// The minimum height of the terrain
    /// </summary>
    public float minHeight{
        get
        {
            float ret = heightMultiplier * heightCurve.Evaluate(0);
            //Debug.Log($"MIN: {ret}");
            return ret;
        }
    }
    /// <summary>
    /// The maximum height of the terrain
    /// </summary>
    public float maxHeight{
        get {
            float ret = heightMultiplier * heightCurve.Evaluate(1);
            //Debug.Log($"MAX: {ret}");
            return ret;
        }
    }

    #if UNITY_EDITOR
    
    protected override void OnValidate()
    {
        noiseSettings.ValidateValues();

        base.OnValidate();
    }
    
    #endif
}
