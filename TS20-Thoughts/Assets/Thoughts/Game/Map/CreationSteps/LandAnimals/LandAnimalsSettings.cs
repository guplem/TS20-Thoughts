using Thoughts.Utils.Inspector;
using Thoughts.Utils.Maths;
using UnityEngine;

public class LandAnimals : UpdatableData
{
    /// <summary>
    /// Settings of the noise map used for the land animals
    /// </summary>
    [Tooltip("Settings of the noise map used for the land animals")]
    public NoiseMapSettings noiseSettings;
    
    /// <summary>
    /// Probability of vegetation appearing.
    /// </summary>
    [Tooltip("Probability of vegetation appearing at any given spot")]
    [Range(0,1)]
    [SerializeField]
    public float probability = 0.5f;
    
    /// <summary>
    /// Density of vegetation appearing.
    /// </summary>
    [Tooltip("Density of vegetation appearing at any given spot")]
    [Range(0,1)]
    [SerializeField]
    public float density = 0.5f; //Todo: use
}
