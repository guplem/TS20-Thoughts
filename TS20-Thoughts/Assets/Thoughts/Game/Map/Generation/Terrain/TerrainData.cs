using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "TerrainData", menuName = "Thoughts/Map/Terrain/TerrainData", order = 21)]
public class TerrainData : UpdatableData
{
    public const int numSupportedLODs = 5;
    
    public HeightMapSettings heightMapSettings;
    private HeightMapSettings oldHeightMapSettings;
    
    public TextureSettings textureSettings;
    private TextureSettings oldTextureSettings;

    #if UNITY_EDITOR
    
    protected override void OnValidate()
    {
        if (oldHeightMapSettings != heightMapSettings)
        {
            oldHeightMapSettings = heightMapSettings;
            Debug.LogWarning("NoiseData updated. Preview won't work until the a map is manually generated using the MapGenerator's inspector.");
        }
        else if (oldTextureSettings != textureSettings)
        {
            oldTextureSettings = textureSettings;
            Debug.LogWarning("TextureData updated. Preview won't work until the a map is manually generated using the MapGenerator's inspector.");
        }
        else
        {
            base.OnValidate();
        }
    }
    
    #endif

}
