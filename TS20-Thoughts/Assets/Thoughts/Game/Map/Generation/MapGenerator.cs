using System;
using UnityEngine;
using UnityEngine.Serialization;



public class MapGenerator : MonoBehaviour
{

    [FormerlySerializedAs("autoRegenerate")]
    [Space]
    public bool autoRegenerateInEditor = false;

    [SerializeField] public TerrainGenerator terrainGenerator;
    public float scale = 1f; //todo: set the proper scale
    [SerializeField] public MapConfiguration mapConfiguration;
    

    public void GenerateMap()
    {
        Debug.LogWarning("TODO: GenerateMap");
        //terrainGenerator.GenerateTerrainData(mapConfiguration);
    }

    public void GenerateMapInEditor()
    {
        terrainGenerator.DrawTerrainInEditor(mapConfiguration, scale);
    }

}
