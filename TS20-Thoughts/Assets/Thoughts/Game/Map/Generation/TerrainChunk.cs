using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk
{

    private const float colliderGenerationDistanceThreshold = 5f;

    public event System.Action<TerrainChunk, bool> onVisibilityChanged;

    public Vector2 coord;
    
    private Vector2 sampleCenter;
    private GameObject meshObject;
    private Bounds bounds;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;

    private MapGenerator mapGenerator;

    private LODInfo[] detailLevels;
    private LODMesh[] lodMeshes;
    private int colliderLODIndex;
    
    private HeightMap heightMap;
    private bool heightMapReceived = false;
    private int previousLODIndex = -1;
    private bool hasSetCollider;
    private float maxViewDistance;

    private Transform viewer;
    private Vector2 viewerPosition => viewer.transform.position.ToVector2WithoutY();

    public TerrainChunk(Vector2 coord, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, MapGenerator mapGenerator, Material material)
    {
        this.coord = coord;
        this.viewer = viewer;
        
        this.mapGenerator = mapGenerator;
        //this.terrainGenerator = mapGenerator.terrainGenerator;
        //this.mapConfiguration = mapGenerator. mapConfiguration;
        
        this.colliderLODIndex = colliderLODIndex;
        
        this.detailLevels = detailLevels;
        this.maxViewDistance = detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
        
        sampleCenter = coord * mapGenerator.mapConfiguration.meshWorldSize / mapGenerator.mapConfiguration.scale;
        Vector2 position = coord * mapGenerator.mapConfiguration.meshWorldSize;
        bounds = new Bounds(sampleCenter, Vector3.one * mapGenerator.mapConfiguration.meshWorldSize);
        
        meshObject = new GameObject("Terrain Chunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshRenderer.material = material;

        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        meshObject.transform.parent = parent;
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
        
    }

    public void Load()
    {
        ThreadedDataRequester.RequestData(
            // () => ... // Creates a method with no parameters that calls the method with parameters. This is done because RequestData expect a method with no parameters
            () => HeightMapGenerator.GenerateHeightMap(mapGenerator.mapConfiguration.chunkSize, mapGenerator.mapConfiguration.chunkSize, mapGenerator.mapConfiguration.terrainData.heightMapSettings, sampleCenter), 
            OnHeightMapRecieved
        );
    }
    
    void OnHeightMapRecieved(object heightMap)
    {
        //terrainGenerator.RequestMeshData(mapData, mapConfiguration, OnMeshDataRecieved);
        this.heightMap = (HeightMap)heightMap;
        heightMapReceived = true;

        UpdateChunkVisibility();
    }
    /*void OnMeshDataRecieved(MeshData meshData)
    {
        meshFilter.mesh = meshData.CreateMesh();
    }*/
    
    public void UpdateChunkVisibility()
    {
        if (!heightMapReceived) 
            return;
        
        float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
        bool wasVisible = IsVisible();
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
                    lodMesh.RequestMesh(heightMap, mapGenerator.terrainGenerator, mapGenerator.mapConfiguration);
                }
            }

        }

        if (wasVisible != visible)
        {
            SetVisible(visible);

            if (onVisibilityChanged != null)
            {
                onVisibilityChanged(this, visible);
            }
            
        }
        
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
                lodMeshes[colliderLODIndex].RequestMesh(heightMap, mapGenerator.terrainGenerator, mapGenerator.mapConfiguration);
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

    void OnMeshDataRecieved(object meshData)
    {
        mesh = ((MeshData)meshData).CreateMesh();
        hasMesh = true;
        updateCallback();
    }
    
    public void RequestMesh(HeightMap heightMap, TerrainGenerator terrainGenerator, MapConfiguration mapConfiguration)
    {
        hasRequestedMesh = true;
        
        // () => ... // Creates a method with no parameters that calls the method with parameters. This is done because RequestData expect a method with no parameters
        ThreadedDataRequester.RequestData(()=>MapDisplay.GenerateTerrainMesh(heightMap.values, mapConfiguration, lod), OnMeshDataRecieved);
    }
}
