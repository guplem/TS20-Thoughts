using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextureData", menuName = "Thoughts/Map/Terrain/TextureData", order = 22)]
public class TextureData : UpdatableData
{

    private float minHeight;
    private float maxHeight;
    
    [SerializeField] public Material material;
    
    public void ApplyToMaterial()
    {
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
