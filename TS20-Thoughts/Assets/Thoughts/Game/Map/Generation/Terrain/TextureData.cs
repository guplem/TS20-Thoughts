using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextureData", menuName = "Thoughts/Map/Terrain/TextureData", order = 22)]
public class TextureData : UpdatableData
{

    public Color color0;
    public Color color1;
    [Range(0, 1)]
    public float color1Start;

    private float minHeight;
    private float maxHeight;
    
    [SerializeField] public Material material;
    
    public void ApplyToMaterial()
    {
        material.SetColor("color0", color0);
        material.SetColor("color1", color1);
        material.SetFloat("color1Start", color1Start);
        
        UpdateMeshHeights(minHeight, maxHeight);
    }

    public void UpdateMeshHeights(float minHeight, float maxHeight)
    {
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
        
        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }
}
