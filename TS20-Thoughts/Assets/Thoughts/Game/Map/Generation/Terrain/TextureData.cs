using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextureData", menuName = "Thoughts/Map/Terrain/TextureData", order = 22)]
public class TextureData : UpdatableData
{
    [SerializeField] public Material material;
    
    [Space]
    public Color color0;
    [Space]
    [Range(0, 1)]
    public float color1Start;
    public Color color1;
    [Space]
    [Range(0, 1)]
    public float color2Start;
    public Color color2;

    private float minHeight;
    private float maxHeight;
    
    public void ApplyToMaterial()
    {
        material.SetColor("color0", color0);
        material.SetFloat("color1Start", color1Start);
        material.SetColor("color1", color1);
        material.SetFloat("color2Start", color2Start);
        material.SetColor("color2", color2);
        
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
