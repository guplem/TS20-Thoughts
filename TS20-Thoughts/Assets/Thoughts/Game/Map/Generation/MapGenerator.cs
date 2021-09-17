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

    [SerializeField] public ThreadedDataRequester threadedDataRequester;

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        // Ensure continuous Update calls. Needed to generate the map in the editor (issues with threads)
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
        }
#endif
    }
    
    private void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            Debug.Log("OnValuesUpdated"); //Todo: it is called multiple times (3) whenever the mesh is being recreated due to an update of the values in the inspector
            GenerateMap(true);
        }
    }

    public void OnValidate()
    {
        if (mapConfiguration == null)
            return;
        mapConfiguration.OnValuesUpdated -= OnValuesUpdated; // So the subscription count stays at 1
        mapConfiguration.OnValuesUpdated += OnValuesUpdated;

        if (mapConfiguration == null)
            return;
        mapConfiguration.OnValuesUpdated -= OnValuesUpdated; // So the subscription count stays at 1
        mapConfiguration.OnValuesUpdated += OnValuesUpdated;

        if (mapConfiguration.heightMapSettings == null)
            return;
        mapConfiguration.heightMapSettings.OnValuesUpdated -= OnValuesUpdated; // So the subscription count stays at 1
        mapConfiguration.heightMapSettings.OnValuesUpdated += OnValuesUpdated;
        
        if (mapConfiguration.textureSettings == null)
            return;
        mapConfiguration.textureSettings.OnValuesUpdated -= OnTextureValuesUpdated; // So the subscription count stays at 1
        mapConfiguration.textureSettings.OnValuesUpdated += OnTextureValuesUpdated;
    }

    void OnTextureValuesUpdated()
    {
        mapConfiguration.textureSettings.ApplyToMaterial(mapConfiguration.heightMapSettings.minHeight, mapConfiguration.heightMapSettings.maxHeight);
    }


    public void GenerateMap(bool clearPreviousMap)
    {
        //terrainGenerator.DrawTerrainInEditor(mapConfiguration);
        terrainGenerator.UpdateChunks(clearPreviousMap);
    }

    public void DeleteCurrentMap()
    {
        //Delete terrain
        terrainGenerator.DeleteTerrain();
        
        //Todo: delete other elements of the map apart from the terrain
    }
}
