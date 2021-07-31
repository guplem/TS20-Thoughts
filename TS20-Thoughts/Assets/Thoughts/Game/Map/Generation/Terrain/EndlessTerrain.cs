using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    /// <summary>
    /// How far "the player" (viewer) should see
    /// </summary>
    [Tooltip("How far 'the player' (viewer) should see")]
    public const float maxViewDistance = 300f;
    /// <summary>
    /// Reference to the viewer (usually the player) of the terrain.  If null at Start, it will be set to 'Camera.main' on Update
    /// </summary>
    [Tooltip("Reference to the viewer (usually the player) of the terrain. If null at Start, it will be set to 'Camera.main' on Update")]
    public Transform viewer;

    [SerializeField] private Material material;

    public static Vector2 viewerPosition;
    public MapGenerator mapGenerator;
    private int chunkSize;
    /// <summary>
    /// Based on the chunkSize and the maxViewDistance, how many chunks are visible
    /// </summary>
    private int chunkVisibleInViewDistance;

    private Dictionary<Vector2, TerrainChunk> terrainChunks = new Dictionary<Vector2, TerrainChunk>();
    private List<TerrainChunk> terrainChunksVisibleAtLastUpdate = new List<TerrainChunk>();
    private bool isviewerNull;

    private void Start()
    {
        isviewerNull = viewer == null;

        chunkSize = MapConfiguration.chunkSize-1; // Because a mesh of dimensions of chunkSize-1 (240) is generated
        chunkVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / chunkSize);
    }

    private void Update()
    {
        if (isviewerNull)
        {
            viewer = Camera.main.transform;
            isviewerNull = viewer == null;
        }
        
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }

    void UpdateVisibleChunks()
    {
        for (int i = 0; i < terrainChunksVisibleAtLastUpdate.Count; i++)
        {
            terrainChunksVisibleAtLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleAtLastUpdate.Clear();
        
        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);
        for (int yOffset = -chunkVisibleInViewDistance; yOffset <= chunkVisibleInViewDistance; yOffset ++)
        {
            for (int xOffset = -chunkVisibleInViewDistance; xOffset <= chunkVisibleInViewDistance; xOffset ++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunks.ContainsKey(viewedChunkCoord))
                {
                    terrainChunks[viewedChunkCoord].UpdateChunkVisibility();
                    if (terrainChunks[viewedChunkCoord].IsVisible())
                    {
                        terrainChunksVisibleAtLastUpdate.Add(terrainChunks[viewedChunkCoord]);
                    }
                }
                else
                {
                    terrainChunks.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, this.transform, mapGenerator.terrainGenerator, mapGenerator.mapConfiguration, material));
                }
            }
        }
    }
    
    public class TerrainChunk
    {

        private Vector2 position;
        private GameObject meshObject;
        private Bounds bounds;

        private MapData mapData;

        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;

        private TerrainGenerator terrainGenerator;
        private MapConfiguration mapConfiguration;
        
        public TerrainChunk(Vector2 coord, int size, Transform parent, TerrainGenerator terrainGenerator, MapConfiguration mapConfiguration, Material material)
        {
            this.terrainGenerator = terrainGenerator;
            this.mapConfiguration = mapConfiguration;
            
            position = coord * size;
            bounds = new Bounds(position, Vector3.one * size);
            Vector3 position3D = new Vector3(position.x, 0, position.y);
            
            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;
            
            meshObject.transform.position = position3D;
            meshObject.transform.parent = parent;
            SetVisible(false);
            
            terrainGenerator.RequestTerrainData(OnMapDataRecieved, mapConfiguration);
        }

        void OnMapDataRecieved(MapData mapData)
        {
            terrainGenerator.RequestMeshData(mapData, mapConfiguration, OnMeshDataRecieved);
        }

        void OnMeshDataRecieved(MeshData meshData)
        {
            meshFilter.mesh = meshData.CreateMesh();
        }
        
        public void UpdateChunkVisibility()
        {
            float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDistanceFromNearestEdge <= maxViewDistance;
            SetVisible(visible);
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

}
