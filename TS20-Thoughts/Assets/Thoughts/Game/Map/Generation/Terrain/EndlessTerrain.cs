using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    
    private const float viewerMoveThresholdForChunkUpdate = 25f;
    private const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;
    private const float colliderGenerationDistanceThreshold = 5f;
    
    [SerializeField] private int colliderLODIndex;
    
    /// <summary>
    /// What LOD should be used until which distance. 0 LOD = max. The last threshold/distance in the list will be considered as the maximum view distance from the viewer's perspective.
    /// </summary>
    [Tooltip("What LOD should be used until which distance. 0 LOD = max. The last threshold/distance in the list will be considered as the maximum view distance from the viewer's perspective.")]
    [SerializeField] public LODInfo[] detailLevels;
    /// <summary>
    /// What LOD should be used until which distance. 0 LOD = max. The highest distance will be considered the maximum view distance from the viewer's perspective.
    /// </summary>
    public static float maxViewDistance;
    
    /// <summary>
    /// Reference to the viewer (usually the player) of the terrain.  If null at Start, it will be set to 'Camera.main'
    /// </summary>
    [Tooltip("Reference to the viewer (usually the player) of the terrain. If null at Start, it will be set to 'Camera.main'")]
    public Transform viewer;

    public static Vector2 viewerPosition;
    public static Vector2 viewerPositionOld;
    public MapGenerator mapGenerator;
    private int meshSize;
    /// <summary>
    /// Based on the chunkSize and the maxViewDistance, how many chunks are visible
    /// </summary>
    private int chunkVisibleInViewDistance;

    private Dictionary<Vector2, TerrainChunk> terrainChunks = new Dictionary<Vector2, TerrainChunk>();
    private static List<TerrainChunk> terrainChunksVisibleAtLastUpdate = new List<TerrainChunk>();
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

        maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
        meshSize = mapGenerator.mapConfiguration.chunkSizeWithoutBorder-1; // Because a mesh of dimensions of chunkSize-1 (240) is generated
        chunkVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / meshSize);
        
        UpdateVisibleChunks();
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z) / mapGenerator.mapConfiguration.terrainScale;

        if (viewerPosition != viewerPositionOld)
        {
            foreach (TerrainChunk chunk in terrainChunksVisibleAtLastUpdate)
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
        for (int i = 0; i < terrainChunksVisibleAtLastUpdate.Count; i++)
        {
            terrainChunksVisibleAtLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleAtLastUpdate.Clear();
        
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshSize);
        for (int yOffset = -chunkVisibleInViewDistance; yOffset <= chunkVisibleInViewDistance; yOffset ++)
        {
            for (int xOffset = -chunkVisibleInViewDistance; xOffset <= chunkVisibleInViewDistance; xOffset ++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunks.ContainsKey(viewedChunkCoord))
                {
                    terrainChunks[viewedChunkCoord].UpdateChunkVisibility();
//                    if (terrainChunks[viewedChunkCoord].IsVisible())
//                    {
//                        terrainChunksVisibleAtLastUpdate.Add(terrainChunks[viewedChunkCoord]);
//                    }
                }
                else
                {
                    terrainChunks.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, meshSize, detailLevels, colliderLODIndex, this.transform, mapGenerator, mapGenerator.mapConfiguration.terrainData.textureData.material));
                }
            }
        }
    }
    
    public class TerrainChunk
    {

        private Vector2 position;
        private GameObject meshObject;
        private Bounds bounds;

        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private MeshCollider meshCollider;

        private TerrainGenerator terrainGenerator;
        private MapConfiguration mapConfiguration;

        private LODInfo[] detailLevels;
        private LODMesh[] lodMeshes;
        private int colliderLODIndex;
        
        private MapData mapData;
        private bool mapDataReceived = false;
        private int previousLODIndex = -1;
        private bool hasSetCollider;
        
        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, MapGenerator mapGenerator, Material material)
        {
            this.terrainGenerator = mapGenerator.terrainGenerator;
            this.mapConfiguration = mapGenerator. mapConfiguration;
            this.colliderLODIndex = colliderLODIndex;
            
            this.detailLevels = detailLevels;
            
            position = coord * size;
            bounds = new Bounds(position, Vector3.one * size);
            Vector3 position3D = new Vector3(position.x, 0, position.y);
            
            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();
            meshRenderer.material = material;
            
            meshObject.transform.position = position3D * mapGenerator.mapConfiguration.terrainScale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * mapGenerator.mapConfiguration.terrainScale;
            SetVisible(false);

            lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < detailLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod);
                lodMeshes[i].updateCallback += UpdateChunkVisibility;
                if (i == colliderLODIndex)
                {
                    lodMeshes[i].updateCallback += UpdateCollisionMesh;
                }
            }
            
            terrainGenerator.RequestTerrainData(position, OnMapDataRecieved, mapConfiguration);
        }

        void OnMapDataRecieved(MapData mapData)
        {
            //terrainGenerator.RequestMeshData(mapData, mapConfiguration, OnMeshDataRecieved);
            this.mapData = mapData;
            mapDataReceived = true;

            UpdateChunkVisibility();
        }
        /*void OnMeshDataRecieved(MeshData meshData)
        {
            meshFilter.mesh = meshData.CreateMesh();
        }*/
        
        public void UpdateChunkVisibility()
        {
            if (!mapDataReceived) 
                return;
            
            
            float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDistanceFromNearestEdge <= maxViewDistance;

            if (visible)
            {
                int lodIndex = 0;

                for (int i = 0; i < detailLevels.Length-1; i++)
                {
                    if (viewerDistanceFromNearestEdge > detailLevels[i].visibleDistanceThreshold)
                    {
                        lodIndex = i + 1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (lodIndex != previousLODIndex)
                {
                    LODMesh lodMesh = lodMeshes[lodIndex];
                    if (lodMesh.hasMesh)
                    {
                        previousLODIndex = lodIndex;
                        meshFilter.mesh = lodMesh.mesh;
                    }
                    else if (!lodMesh.hasRequestedMesh)
                    {
                        lodMesh.RequestMesh(mapData, terrainGenerator, mapConfiguration);
                    }
                }

                terrainChunksVisibleAtLastUpdate.Add(this);
            }
            
            SetVisible(visible);
        }

        public void UpdateCollisionMesh()
        {
            if (hasSetCollider)
                return;
            
            float sqrDistanceFromViewerToEdge = bounds.SqrDistance(viewerPosition);

            if (sqrDistanceFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDistanceThreshold)
            {
                if (!lodMeshes[colliderLODIndex].hasRequestedMesh)
                {
                    lodMeshes[colliderLODIndex].RequestMesh(mapData, terrainGenerator, mapConfiguration);
                }
            }
            
            if (sqrDistanceFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold)
            {
                if (lodMeshes[colliderLODIndex].hasMesh)
                {
                    meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
                    hasSetCollider = true;
                }
            }
        }

        public void SetVisible(bool state)
        {
            meshObject.SetActive(state);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }

    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        private int lod;
        public event System.Action updateCallback;

        public LODMesh(int lod)
        {
            this.lod = lod;
        }

        void OnMeshDataRecieved(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            hasMesh = true;
            updateCallback();
        }
        
        public void RequestMesh(MapData mapData, TerrainGenerator terrainGenerator, MapConfiguration mapConfiguration)
        {
            hasRequestedMesh = true;
            terrainGenerator.RequestMeshData(mapData, mapConfiguration, lod, OnMeshDataRecieved);
        }
    }

    [System.Serializable]
    public struct LODInfo
    {
        [Range(0,MapDisplay.numSupportedLODs-1)]
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

}
