using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject chunkPrefab;
    
    private const float viewerMoveThresholdForChunkUpdate = 25f;
    private const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;
    
    [SerializeField] private int colliderLODIndex;
    
    /// <summary>
    /// What LOD should be used until which distance. 0 LOD = max. The last threshold/distance in the list will be considered as the maximum view distance from the viewer's perspective.
    /// </summary>
    [Tooltip("What LOD should be used until which distance. 0 LOD = max. The last threshold/distance in the list will be considered as the maximum view distance from the viewer's perspective.")]
    [SerializeField] public LODInfo[] detailLevels;
    
    /*
    /// <summary>
    /// What LOD should be used until which distance. 0 LOD = max. The highest distance will be considered the maximum view distance from the viewer's perspective.
    /// </summary>
    //public static float maxViewDistance;
    */

    /// <summary>
    /// Reference to the viewer (usually the player) of the terrain. If null at Start, it will be set to 'Camera.main' then.
    /// </summary>
    public Transform viewer {
        get
        {
            return _viewer;
        }
        set
        {
            _viewer = value;
        }
    }
    public Transform _viewer;

    Vector2 viewerPosition;
    Vector2 viewerPositionOld;
    public MapGenerator mapGenerator;
    private float meshSize => mapGenerator.mapConfiguration.meshWorldSize;
    /// <summary>
    /// Based on the chunkSize and the maxViewDistance, how many chunks are visible
    /// </summary>
    private int chunkVisibleInViewDistance => Mathf.RoundToInt( (Application.isPlaying? mapGenerator.mapConfiguration.mapRadius : mapGenerator.mapConfiguration.mapPreviewRadius) / meshSize);

    private Dictionary<Vector2, TerrainChunk> terrainChunks = new Dictionary<Vector2, TerrainChunk>();
    //private List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();

    private void Start()
    {
        if (viewer == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera)
                viewer = mainCamera.transform;
            else
                Debug.LogWarning("The main camera has not been found to be set as the terrain' viewer.", this);

            Debug.Log($"EndlessTerrain viewer set at Start to '{viewer}'", viewer);
        }

        //float maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
        //meshSize = mapGenerator.mapConfiguration.meshWorldSize; // Because a mesh of dimensions of chunkSize-1 (240) is generated
        //chunkVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / meshSize);
        
        // UpdateVisibleChunks();
    }

    private void Update()
    {
        viewerPosition = viewer != null ? new Vector2(viewer.position.x, viewer.position.z) : Vector2.zero;

        /*if (viewerPosition != viewerPositionOld)
        {
            foreach (TerrainChunk chunk in visibleTerrainChunks)
            {
                chunk.UpdateCollisionMesh();
            }
        }*/
        
        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateChunks(false);
        }
    }

    public void UpdateChunks(bool clearPreviousMap)
    {
        Debug.Log($"Updating chunks. clearPreviousMap? {clearPreviousMap}");

        if (clearPreviousMap)
        {
            //HeightMapGenerator.ClearFalloffMap();
            DeleteTerrain();
        }

        //HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
        
        /*for (int i = visibleTerrainChunks.Count-1; i >= 0; i--) // Goes backwards because 'UpdateChunkVisibility' can remove the TerrainChunk from the list we are iterating on
        {
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
            visibleTerrainChunks[i].UpdateChunkVisibility ();
        }*/
        
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshSize);
        for (int yOffset = -chunkVisibleInViewDistance; yOffset <= chunkVisibleInViewDistance; yOffset ++)
        {
            for (int xOffset = -chunkVisibleInViewDistance; xOffset <= chunkVisibleInViewDistance; xOffset ++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                //if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord)) {
                    if (terrainChunks.ContainsKey(viewedChunkCoord))
                    {
                        terrainChunks[viewedChunkCoord].UpdateChunkVisibility();
                    }
                    else
                    {
                        GameObject chunkGameObject = Instantiate(chunkPrefab);
                        TerrainChunk terrainChunk = chunkGameObject.GetComponentRequired<TerrainChunk>();
                        terrainChunk.Setup(viewedChunkCoord, detailLevels, colliderLODIndex, this.transform, viewer, mapGenerator, mapGenerator.mapConfiguration.textureSettings.material);
                        terrainChunks.Add(viewedChunkCoord, terrainChunk);
                        //newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
                        terrainChunk.Load();
                    }
                //}
            }
        }
    }

    /*void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
    {
        if (isVisible)
            visibleTerrainChunks.Add(chunk);
        else
            visibleTerrainChunks.Remove(chunk);
    }*/

    public void DeleteTerrain()
    {
        terrainChunks.Clear();
        //terrainChunks = new Dictionary<Vector2, TerrainChunk>();
            
        if (Application.isPlaying)
            gameObject.transform.DestroyAllChildren(); 
        else
            gameObject.transform.DestroyImmediateAllChildren();
    }
}


[System.Serializable]
public struct LODInfo
{
    [Range(0,MapConfiguration.numSupportedTerrainLODs-1)]
    [SerializeField]public int lod;
    /// <summary>
    /// Once the viewer is outside of the threshold, it will switch over to the next level of detail lower resolution version)
    /// </summary>
    [Tooltip("Once the viewer is outside of the threshold, it will switch over to the next level of detail lower resolution version)")]
    [SerializeField] public float visibleDistanceThreshold;

    public float sqrVisibleDistanceThreshold
    {
        get
        {
            return visibleDistanceThreshold * visibleDistanceThreshold;
        }
    }
}