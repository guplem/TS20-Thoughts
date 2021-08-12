using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    
    private const float viewerMoveThresholdForChunkUpdate = 25f;
    private const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;
    
    [SerializeField] private int colliderLODIndex;
    
    /// <summary>
    /// What LOD should be used until which distance. 0 LOD = max. The last threshold/distance in the list will be considered as the maximum view distance from the viewer's perspective.
    /// </summary>
    [Tooltip("What LOD should be used until which distance. 0 LOD = max. The last threshold/distance in the list will be considered as the maximum view distance from the viewer's perspective.")]
    [SerializeField] public LODInfo[] detailLevels;
    /// <summary>
    /// What LOD should be used until which distance. 0 LOD = max. The highest distance will be considered the maximum view distance from the viewer's perspective.
    /// </summary>
    //public static float maxViewDistance;
    
    /// <summary>
    /// Reference to the viewer (usually the player) of the terrain.  If null at Start, it will be set to 'Camera.main'
    /// </summary>
    [Tooltip("Reference to the viewer (usually the player) of the terrain. If null at Start, it will be set to 'Camera.main'")]
    public Transform viewer;

    Vector2 viewerPosition;
    Vector2 viewerPositionOld;
    public MapGenerator mapGenerator;
    private float meshSize;
    /// <summary>
    /// Based on the chunkSize and the maxViewDistance, how many chunks are visible
    /// </summary>
    private int chunkVisibleInViewDistance;

    private Dictionary<Vector2, TerrainChunk> terrainChunks = new Dictionary<Vector2, TerrainChunk>();
    private List<TerrainChunk> visibleTerrainChunks = new List<TerrainChunk>();
    private bool isviewerNull;

    private void Start()
    {
        isviewerNull = viewer == null;
        if (isviewerNull)
        {
            viewer = Camera.main.transform;
            isviewerNull = viewer == null;
            Debug.LogWarning($"EndlessTerrain viewer set at Start to '{viewer}'", gameObject);
        }

        float maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
        meshSize = mapGenerator.mapConfiguration.meshWorldSize; // Because a mesh of dimensions of chunkSize-1 (240) is generated
        chunkVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / meshSize);
        
        UpdateVisibleChunks();
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z)  /*  / mapGenerator.mapConfiguration.scale  */  ;

        if (viewerPosition != viewerPositionOld)
        {
            foreach (TerrainChunk chunk in visibleTerrainChunks)
            {
                chunk.UpdateCollisionMesh();
            }
        }
        
        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    void UpdateVisibleChunks()
    {
        HashSet<Vector2> alreadyUpdatedChunkCoords = new HashSet<Vector2>();
        
        for (int i = visibleTerrainChunks.Count-1; i >= 0; i--) // Goes backwards because 'UpdateChunkVisibility' can remove the TerrainChunk from the list we are iterating on
        {
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
            visibleTerrainChunks[i].UpdateChunkVisibility ();
        }
        
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshSize);
        for (int yOffset = -chunkVisibleInViewDistance; yOffset <= chunkVisibleInViewDistance; yOffset ++)
        {
            for (int xOffset = -chunkVisibleInViewDistance; xOffset <= chunkVisibleInViewDistance; xOffset ++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                {
                    if (terrainChunks.ContainsKey(viewedChunkCoord))
                    {
                        terrainChunks[viewedChunkCoord].UpdateChunkVisibility();
                    }
                    else
                    {
                        TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, detailLevels, colliderLODIndex, this.transform, viewer, mapGenerator, mapGenerator.mapConfiguration.terrainData.textureSettings.material);
                        terrainChunks.Add(viewedChunkCoord, newChunk);
                        newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
                        newChunk.Load();
                    }
                }
            }
        }
    }

    void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
    {
        if (isVisible)
            visibleTerrainChunks.Add(chunk);
        else
            visibleTerrainChunks.Remove(chunk);
    }

}


[System.Serializable]
public struct LODInfo
{
    [Range(0,TerrainData.numSupportedLODs-1)]
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