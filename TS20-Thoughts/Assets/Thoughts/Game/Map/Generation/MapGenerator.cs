using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Component in charge of generating a Map
/// </summary>
public class MapGenerator : MonoBehaviour
{

    [FormerlySerializedAs("autoRegenerate")]
    [Space]
    public bool autoRegenerateInEditor = false;

    /// <summary>
    /// Reference to the TerrainGenerator component in charge of generating the Terrain
    /// </summary>
    [Tooltip("Reference to the TerrainGenerator component in charge of generating the Terrain")]
    [SerializeField] public TerrainGenerator terrainGenerator;

    /// <summary>
    /// Reference to the MapConfiguration with he settings to generate a map
    /// </summary>
    [Tooltip("Reference to the MapConfiguration with he settings to generate a map")]
    [SerializeField] public MapConfiguration mapConfiguration;

    /// <summary>
    /// Reference to the ThreadedDataRequester component in charge doing threaded requests of data
    /// </summary>
    [Tooltip("Reference to the ThreadedDataRequester component in charge doing threaded requests of data")]
    [SerializeField] public ThreadedDataRequester threadedDataRequester;

    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // Ensure continuous Update calls. Needed to generate the map in the editor (issues with threads)
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();
        }
    }
    #endif
    
    /// <summary>
    /// If the app is not in Play Mode, the previously created map is destroyed and a new map is generated.
    /// </summary>
    private void RegenerateMapNotPlaying()
    {
        if (!Application.isPlaying)
        {
            Debug.Log("OnValuesUpdated"); //Todo: it is called multiple times (3) whenever the mesh is being recreated due to an update of the values in the inspector
            GenerateMap(true);
        }
    }

    //TODO: Improve the auto update system (time intervals, wait for the previous preview to fully load, ...)
    public void OnValidate()
    {
        if (mapConfiguration == null)
            return;
        mapConfiguration.OnValuesUpdated -= RegenerateMapNotPlaying; // So the subscription count stays at 1
        mapConfiguration.OnValuesUpdated += RegenerateMapNotPlaying;

        /*if (mapConfiguration == null)
            return;
        mapConfiguration.OnValuesUpdated -= RegenerateMapNotPlaying; // So the subscription count stays at 1
        mapConfiguration.OnValuesUpdated += RegenerateMapNotPlaying;*/

        if (mapConfiguration.heightMapSettings == null)
            return;
        mapConfiguration.heightMapSettings.OnValuesUpdated -= RegenerateMapNotPlaying; // So the subscription count stays at 1
        mapConfiguration.heightMapSettings.OnValuesUpdated += RegenerateMapNotPlaying;
        
        if (mapConfiguration.textureSettings == null)
            return;
        mapConfiguration.textureSettings.OnValuesUpdated -= OnTextureValuesUpdated; // So the subscription count stays at 1
        mapConfiguration.textureSettings.OnValuesUpdated += OnTextureValuesUpdated;
    }

    /// <summary>
    /// Manages the update of the TextureSettings by applying them to the map's Material
    /// </summary>
    void OnTextureValuesUpdated()
    {
        mapConfiguration.textureSettings.ApplyToMaterial(mapConfiguration.heightMapSettings.minHeight, mapConfiguration.heightMapSettings.maxHeight);
    }


    /// <summary>
    /// Updates or generates the map.
    /// </summary>
    /// <param name="clearPreviousMap">If existent, should the previously created map be deleted?</param>
    public void GenerateMap(bool clearPreviousMap)
    {
        //terrainGenerator.DrawTerrainInEditor(mapConfiguration);
        terrainGenerator.UpdateChunks(clearPreviousMap);
    }

    /// <summary>
    /// Deletes the currently (generated) existent map
    /// </summary>
    public void DeleteCurrentMap()
    {
        //Delete terrain
        terrainGenerator.DeleteTerrain();
        
        //Todo: delete other elements of the map apart from the terrain
    }
}
