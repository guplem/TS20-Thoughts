using UnityEngine;

/// <summary>
/// The configuration of the texture of the terrain of the map
/// </summary>
[CreateAssetMenu(fileName = "TextureData", menuName = "Thoughts/Map/Terrain/TextureData", order = 22)]
public class TextureSettings : UpdatableData
{
    [SerializeField] public Material material;
    
    [Space]
    public Color color0;
    [Space]
    [Range(0, 1)]
    public float color1Start;
    public Color color1;
    [Range(0,1)]
    public float color1BaseBlend;
    [Space]
    [Range(0, 1)]
    public float color2Start;
    public Color color2;
    [Range(0,1)]
    public float color2BaseBlend;
    [Space]
    [Range(0, 1)]
    public float color3Start;
    public Color color3;
    [Range(0,1)]
    public float color3BaseBlend;

    private float minHeight;
    private float maxHeight;
    
    public void ApplyToMaterial(float minHeight, float maxHeight)
    {
        material.SetColor("color0", color0);
        
        material.SetFloat("color1Start", color1Start);
        material.SetColor("color1", color1);
        material.SetFloat("color1BaseBlend", color1BaseBlend);
        
        material.SetFloat("color2Start", color2Start);
        material.SetColor("color2", color2);
        material.SetFloat("color2BaseBlend", color2BaseBlend);

        material.SetFloat("color3Start", color3Start);
        material.SetColor("color3", color3);
        material.SetFloat("color3BaseBlend", color3BaseBlend);
        
        UpdateMeshHeights(minHeight, maxHeight);
    }

    public void UpdateMeshHeights(float minHeight, float maxHeight)
    {
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;
        
        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
        
        //Debug.Log($"Set the minHeight to {minHeight} and maxHeight to {maxHeight}");
    }
}
