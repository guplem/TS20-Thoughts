using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Console = System.Console;

public class TerrainChunk : MonoBehaviour
{

    //private const float colliderGenerationDistanceThreshold = 5f;

    //public event System.Action<TerrainChunk, bool> onVisibilityChanged;

    private Vector2 coord;
    
    private Vector2 sampleCenter;
    //private GameObject meshObject;
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
    private Vector2 viewerPosition => viewer? viewer.transform.position.ToVector2WithoutY() : Vector2.zero;

    public void Setup(Vector2 coord, LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, MapGenerator mapGenerator, Material material)
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
        
        //new GameObject($"Terrain Chunk {this.coord}");
        meshRenderer = gameObject.GetComponentRequired<MeshRenderer>();
        meshFilter = gameObject.GetComponentRequired<MeshFilter>();
        //Debug.Log($"Mesh Filter added: {meshFilter}", meshFilter);
        meshCollider = gameObject.GetComponentRequired<MeshCollider>();
        meshRenderer.material = material;

        Transform transform = this.transform;
        transform.position = new Vector3(position.x, 0, position.y);
        transform.parent = parent;
        
        //SetVisible(false);

        lodMeshes = new LODMesh[detailLevels.Length];
        for (int i = 0; i < detailLevels.Length; i++)
        {
            lodMeshes[i] = new LODMesh(detailLevels[i].lod);
            lodMeshes[i].updateCallback += UpdateChunkVisibility;
            /*if (i == colliderLODIndex)
            {
                lodMeshes[i].updateCallback += UpdateCollisionMesh;
            }*/
        }
        
    }

    public void Load()
    {
        //Debug.Log($"Requesting data for {ToString()}");
        mapGenerator.threadedDataRequester.RequestData(
            // () => ... // Creates a method with no parameters that calls the method with parameters. This is done because RequestData expect a method with no parameters
            () => HeightMapGenerator.GenerateHeightMap(mapGenerator.mapConfiguration.chunkSize, mapGenerator.mapConfiguration.chunkSize, mapGenerator.mapConfiguration.mapRadius, mapGenerator.mapConfiguration.heightMapSettings, sampleCenter), 
            OnHeightMapReceived
        );
    }

    public override string ToString()
    {
        return $"{nameof(TerrainChunk)} with coords {coord}";
    }

    private void OnHeightMapReceived(object heightMap)
    {
        //Debug.Log($"Received height data for {ToString()}");
        //terrainGenerator.RequestMeshData(mapData, mapConfiguration, OnMeshDataRecieved);
        this.heightMap = (HeightMap)heightMap;
        heightMapReceived = true;

        UpdateChunkVisibility();
    }
    /*void OnMeshDataRecieved(MeshData meshData)
    {
        meshFilter.mesh = meshData.CreateMesh();
    }*/
    
    
    public void UpdateChunkVisibility() // Todo: change to UpdateChunk
    {
        try
        {
            if (!heightMapReceived || !gameObject) 
                return;
        }
        catch (Exception) // In case the gameObject is destroyed it should throw an exception. It happens while regenerating the terrain in the editor
        {
            return;
        }

        UpdateVisualMesh();
        UpdateCollisionMesh();
        
        /*if (wasVisible != visible)
        {
            SetVisible(visible);

            if (onVisibilityChanged != null)
            {
                onVisibilityChanged(this, visible);
            }
            
        }*/

    }

    public void UpdateVisualMesh()
    {
        float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
        //bool wasVisible = IsVisible();
        //bool visible = viewerDistanceFromNearestEdge <= maxViewDistance;

        // if (visible) {
            int lodIndex = 0;
            for (int i = 0; i < detailLevels.Length-1; i++)
                if (viewerDistanceFromNearestEdge > detailLevels[i].visibleDistanceThreshold)
                    lodIndex = i + 1;
                else
                    break;

            if (lodIndex != previousLODIndex)
            {
                LODMesh lodMesh = lodMeshes[lodIndex];
                //Debug.Log($"New LOD for {ToString()}. Does LOD have mesh? {lodMesh.hasMesh}. ");

                if (!lodMesh.hasRequestedMesh)
                {
                    lodMesh.RequestMesh(heightMap, mapGenerator.threadedDataRequester, mapGenerator.mapConfiguration);
                }
                else if (lodMesh.hasMesh)
                {
                    previousLODIndex = lodIndex;
                    /*if (meshFilter == null)
                    {
                        Debug.LogWarning($"MeshFilter was null for {ToString()} while trying to set its mesh.");
                        meshFilter = gameObject.GetComponentRequired<MeshFilter>();
                    }*/
                    meshFilter.mesh = lodMesh.mesh;
                }
            }
        // }
    }
    
    public void UpdateCollisionMesh()
    {
        
        //Debug.Log("Updating collision mesh");
        
        if (hasSetCollider)
            return;
        
        //float sqrDistanceFromViewerToEdge = bounds.SqrDistance(viewerPosition);

        //if (sqrDistanceFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDistanceThreshold) {
            if (!lodMeshes[colliderLODIndex].hasRequestedMesh)
            {
                lodMeshes[colliderLODIndex].RequestMesh(heightMap, mapGenerator.threadedDataRequester, mapGenerator.mapConfiguration);
            }
        // }
        
        //if (sqrDistanceFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold) {
            else if (lodMeshes[colliderLODIndex].hasMesh)
            {
                meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
                hasSetCollider = true;
            }
        //}
    }

    /*public void SetVisible(bool state)
    {
        meshObject.SetActive(state);
    }

    public bool IsVisible()
    {
        return meshObject.activeSelf;
    }*/
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

    void OnMeshDataReceived(object meshData)
    {
        //Debug.Log($"OnMeshDataReceived");
        mesh = ((MeshData)meshData).CreateMesh();
        hasMesh = true;
        updateCallback();
    }
    
    public void RequestMesh(HeightMap heightMap, ThreadedDataRequester threadedDataRequester, MapConfiguration mapConfiguration)
    {
        //Debug.Log($"Requesting mesh for height map");

        hasRequestedMesh = true;
        
        threadedDataRequester.RequestData(
            // () => ... // Creates a method with no parameters that calls the method with parameters. This is done because RequestData expect a method with no parameters
            () => TerrainMeshGenerator.GenerateTerrainMesh(heightMap.values, mapConfiguration, lod),
            OnMeshDataReceived
        );
    }
}
