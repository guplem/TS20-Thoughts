using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextureData", menuName = "Thoughts/Map/Terrain/TextureData", order = 22)]
public class TextureData : UpdatableData
{
    
    [SerializeField] public Material material;
    
    public void ApplyToMaterial()
    {
        
    }

    public void UpdateMeshHeights(float minHeight, float maxHeight)
    {
        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }
}
