using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TerrainData", menuName = "Thoughts/TerrainData", order = 11)]
public class TerrainData : ScriptableObject
{
    /// <summary>
    /// For how much each cell height will be multiplied.
    /// <para>The height of the cell is by default [0,1], multiplying it by 5, the maximum height will be 5 (cell height [0,5]</para>
    /// </summary>
    public float maxHeight = 15f;
    /// <summary>
    /// How much the height of the mesh should be affected by the maxHeight (AKA: "height multiplier")
    /// </summary>
    public AnimationCurve heightCurve;

    public NoiseData noiseData;
}
