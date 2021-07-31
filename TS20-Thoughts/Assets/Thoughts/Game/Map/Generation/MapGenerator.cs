using System;
using UnityEngine;



public class MapGenerator : MonoBehaviour
{

    [Space]
    public bool autoRegenerate = false;

    [SerializeField] public TerrainGenerator terrainGenerator;
    [SerializeField] public MapConfiguration mapConfiguration;

    public void GenerateMap()
    {
        Debug.LogWarning("TODO");
        //terrainGenerator.GenerateTerrainData(mapConfiguration);
    }

    public void GenerateMapInEditor()
    {
        terrainGenerator.DrawTerrainInEditor(mapConfiguration);
    }

}
