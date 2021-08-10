using System;
using UnityEngine;
using UnityEngine.Serialization;



public class MapGenerator : MonoBehaviour
{

    [FormerlySerializedAs("autoRegenerate")]
    [Space]
    public bool autoRegenerateInEditor = false;

    [SerializeField] public TerrainGenerator terrainGenerator;

    [SerializeField] public MapConfiguration mapConfiguration;

    private void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            //Debug.Log("TTT"); //Todo: it is called multiple times (3) whenever the mesh is being recreated due to an update of the values in the inspector
            GenerateMapInEditor();
        }
    }

    public void OnValidate()
    {
        if (mapConfiguration == null)
            return;
        mapConfiguration.OnValuesUpdated -= OnValuesUpdated; // So the subscription count stays at 1
        mapConfiguration.OnValuesUpdated += OnValuesUpdated;

        if (mapConfiguration.terrainData == null)
            return;
        mapConfiguration.terrainData.OnValuesUpdated -= OnValuesUpdated; // So the subscription count stays at 1
        mapConfiguration.terrainData.OnValuesUpdated += OnValuesUpdated;

        if (mapConfiguration.terrainData.heightMapSettings == null)
            return;
        mapConfiguration.terrainData.heightMapSettings.OnValuesUpdated -= OnValuesUpdated; // So the subscription count stays at 1
        mapConfiguration.terrainData.heightMapSettings.OnValuesUpdated += OnValuesUpdated;
        
        if (mapConfiguration.terrainData.textureData == null)
            return;
        mapConfiguration.terrainData.textureData.OnValuesUpdated -= OnTextureValuesUpdated; // So the subscription count stays at 1
        mapConfiguration.terrainData.textureData.OnValuesUpdated += OnTextureValuesUpdated;
    }

    void OnTextureValuesUpdated()
    {
        mapConfiguration.terrainData.textureData.ApplyToMaterial();
    }


    public void GenerateMap()
    {
        Debug.LogWarning("TODO: GenerateMap");
        //BringMapToLife(); // Enable auto update of "endlessTerrain"
    }

    public void GenerateMapInEditor()
    {
        terrainGenerator.DrawTerrainInEditor(mapConfiguration);
    }

}
