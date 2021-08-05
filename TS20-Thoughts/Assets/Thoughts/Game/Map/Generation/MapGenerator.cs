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
