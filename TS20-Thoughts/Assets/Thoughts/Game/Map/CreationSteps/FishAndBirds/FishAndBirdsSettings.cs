using Thoughts.Utils.Inspector;
using Thoughts.Utils.Maths;
using UnityEngine;

public class FishAndBirdsSettings : UpdatableData
{
    /// <summary>
    /// Settings of the noise map used for the fish
    /// </summary>
    [Tooltip("Settings of the noise map used for the fish")]
    public NoiseMapSettings fishNoiseSettings;
    /// <summary>
    /// Probability of fish appearing.
    /// </summary>
    [Tooltip("Probability of fish appearing at any given spot")]
    [Range(0,1)]
    [SerializeField]
    public float fishProbability = 0.5f;
    /// <summary>
    /// Density of fish appearing.
    /// </summary>
    [Tooltip("Density of fish appearing at any given spot")]
    [Range(0,1)]
    [SerializeField]
    public float fishDensity = 0.5f;
    
    /// <summary>
    /// Settings of the noise map used for the birds
    /// </summary>
    [Tooltip("Settings of the noise map used for the birds")]
    public NoiseMapSettings birdsNoiseSettings;
    /// <summary>
    /// Probability of birds appearing.
    /// </summary>
    [Tooltip("Probability of birds appearing at any given spot")]
    [Range(0,1)]
    [SerializeField]
    public float birdsProbability = 0.5f;
    /// <summary>
    /// Density of birds appearing.
    /// </summary>
    [Tooltip("Density of birds appearing at any given spot")]
    [Range(0,1)]
    [SerializeField]
    public float birdsDensity = 0.5f;
}
