using System;
using Thoughts.Utils.ThreadsManagement;
using UnityEngine;

namespace Thoughts.Game.Map.Terrain
{
    /// <summary>
    /// Manager of a chunk of the terrain of the map
    /// </summary>
    public class TerrainChunk : MonoBehaviour
    {
        
        /// <summary>
        /// Coords of the chunk
        /// </summary>
        private Vector2 chunkIndex;
    
        /// <summary>
        /// The location of the center of the chunk in the world space
        /// </summary>
        private Vector2 centerWorldLocation;
    
        /// <summary>
        /// Reference to the object containing the visual mesh of the TerrainChunk
        /// </summary>
        [SerializeField] private GameObject visualMeshObject;
    
        /// <summary>
        /// The bounds of the TerrainChunk in the map
        /// </summary>
        private Bounds bounds;

        /// <summary>
        /// Reference to the meshRenderer used for the visuals
        /// </summary>
        private MeshRenderer meshRenderer;
        /// <summary>
        /// Reference to the MeshFilter used for the visuals
        /// </summary>
        private MeshFilter meshFilter;
        /// <summary>
        /// Reference to the MeshCollider used for the collisions
        /// </summary>
        private MeshCollider meshCollider;

        /// <summary>
        /// Reference to the mapGenerator managing the generation of the map that contains this TerrainChunk.
        /// </summary>
        private MapGenerator mapGenerator;

        /// <summary>
        /// An ordered list containing the LOD with the information about which one should be used until which distance. The LOD "0", has the maximum level of detail. The last threshold/distance in the list will be considered as the maximum view distance from the viewer's perspective.
        /// </summary>
        private LODInfo[] detailLevels;
    
        /// <summary>
        /// A list of meshes meant to be used with an specified LOD
        /// </summary>
        private LODMesh[] lodMeshes;
    
        /// <summary>
        /// The LOD that the collider must use.
        /// </summary>
        private int colliderLODIndex => mapGenerator.terrainGenerator.colliderLODIndex;

        /// <summary>
        /// The HeightMap of this TerrainChunk
        /// </summary>
        public HeightMap heightMap { get; private set; }

        /// <summary>
        /// If the data of the HeightMap has been recieved or not.
        /// </summary>
        private bool heightMapReceived = false;
    
        /// <summary>
        /// The last LOD used for the latest visuals of the terrain
        /// </summary>
        private int previousLODIndex = -1;

        /// <summary>
        /// Has the collier been set in this TerrainChunk
        /// </summary>
        private bool hasSetCollider
        {
            get => _hasSetCollider;
            set
            {
                _hasSetCollider = value;
                NotifyUpdateOnState();
            }
        }
        private bool _hasSetCollider = false;
        
        /// <summary>
        /// Has the visual mesh been set in this TerrainChunk in the MeshFilter
        /// </summary>
        private bool hasSetVisualMesh
        {
            get => _hasSetVisualMesh;
            set
            {
                _hasSetVisualMesh = value;
                NotifyUpdateOnState();
            }
        }
        private bool _hasSetVisualMesh = false;
        
        private void NotifyUpdateOnState()
        {
            if (hasSetCollider && hasSetVisualMesh)
            {
                //Debug.Log($"Terrain Chunk {this.gameObject.name} completed the gneration.");
                terrainCompletionCallback?.Invoke();
            }
        }
        /// <summary>
        /// The callback action to do after completing the generation of the terrain
        /// </summary>
        public event System.Action terrainCompletionCallback;

        /// <summary>
        /// A reference to the viewer (typically the player) of the map
        /// </summary>
        private Transform viewer => mapGenerator.terrainGenerator.viewer;
    
        /// <summary>
        /// The maximum distance at which the TerrainChunks should be visible
        /// </summary>
        private float maxViewDistance => detailLevels[detailLevels.Length - 1].visibleDistanceThreshold;
    
        /// <summary>
        /// The current position of the viewer
        /// </summary>
        private Vector2 viewerPosition => viewer? viewer.transform.position.ToVector2WithoutY() : Vector2.zero;

        /// <summary>
        /// Stores all the needed data and sets up the managing of the LOD meshes
        /// </summary>
        /// <param name="chunkIndex">Coords of the chunk relative to the other chunks</param>
        /// <param name="detailLevels">An ordered list containing the LOD with the information about which one should be used until which distance. The LOD "0", has the maximum level of detail. The last threshold/distance in the list will be considered as the maximum view distance from the viewer's perspective.</param>
        /// <param name="parent">The parent of this TerrainChunk's GameObject.</param>
        /// <param name="mapGenerator">Reference to the mapGenerator managing the generation of the map that contains this TerrainChunk.</param>
        public void Setup(Vector2 chunkIndex, LODInfo[] detailLevels, Transform parent, MapGenerator mapGenerator)
        {
            this.chunkIndex = chunkIndex;
            this.detailLevels = detailLevels;
            this.mapGenerator = mapGenerator;
            
            centerWorldLocation = chunkIndex * mapGenerator.mapConfiguration.chunkWorldSize;
        
            Vector2 position = chunkIndex * mapGenerator.mapConfiguration.chunkWorldSize;
            bounds = new Bounds(centerWorldLocation, Vector3.one * mapGenerator.mapConfiguration.chunkWorldSize);
        
            meshRenderer = visualMeshObject.GetComponentRequired<MeshRenderer>();
            meshFilter = visualMeshObject.GetComponentRequired<MeshFilter>();
            //Debug.Log($"Mesh Filter added: {meshFilter}", meshFilter);
            meshCollider = gameObject.GetComponentRequired<MeshCollider>();
            meshRenderer.material = mapGenerator.mapConfiguration.terrainTextureSettings.material;

            Transform transform = this.transform;
            transform.position = new Vector3(position.x, 0, position.y);
            transform.parent = parent;
        
            SetMeshVisible(false);

            lodMeshes = new LODMesh[detailLevels.Length];
            for (int i = 0; i < detailLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailLevels[i].lod);
                lodMeshes[i].updateCallback += UpdateChunk;
            }
        
        }

        /// <summary>
        /// Starts the process of loading and displaying this TerrainChunk
        /// </summary>
        /// <param name="completionRegisterer"></param>
        public void Load(Action completionRegisterer)
        {
            //Debug.Log($"Requesting data for {ToString()}");
            mapGenerator.threadedDataRequester.RequestData(
                // () => ... // Creates a method with no parameters that calls the method with parameters. This is done because RequestData expect a method with no parameters
                () => HeightMap.GenerateHeightMap(mapGenerator.mapConfiguration.numVertsPerLine, mapGenerator.mapConfiguration.numVertsPerLine, mapGenerator.mapConfiguration.mapRadius, mapGenerator.mapConfiguration.terrainHeightSettings, centerWorldLocation, mapGenerator.terrainGenerator.terrainSeed, mapGenerator.mapConfiguration.terrainHeightSettings.freeFalloffAreaRadius), 
                OnHeightMapReceived
            );
            terrainCompletionCallback += completionRegisterer;
        }
    
        /// <summary>
        /// Saves the HeightMap received and updates the TerrainChunk
        /// </summary>
        /// <param name="heightMap">The received HeightMap</param>
        private void OnHeightMapReceived(object heightMap)
        {
            //Debug.Log($"Received height data for {ToString()}");
            //terrainGenerator.RequestMeshData(mapData, mapConfiguration, OnMeshDataRecieved);
            this.heightMap = (HeightMap)heightMap;
            heightMapReceived = true;

            UpdateChunk();
        }
    
        /// <summary>
        /// Updates the visual mesh and tries to set the collision mesh if it has not been set before
        /// </summary>
        public void UpdateChunk()
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
            if (!hasSetCollider)
                SetCollisionMesh();
        }

        /// <summary>
        /// Updates the LOD and visibility of the TerrainChunk
        /// </summary>
        private void UpdateVisualMesh()
        {
            float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDistanceFromNearestEdge <= maxViewDistance;
            SetMeshVisible(visible);

            if (visible) {
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
                        meshFilter.mesh = lodMesh.mesh;
                        hasSetVisualMesh = true;
                    }
                }
            }
        }
    
        /// <summary>
        /// String representation of this TerrainChunk
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(TerrainChunk)} with coords {chunkIndex}";
        }
    
        /// <summary>
        /// Sets the collision mesh in this TerrainChunk. Requires at least 2 calls: request + set (after mesh has been returned by the thread).
        /// The calls should be done automatically thanks to the callback
        /// </summary>
        private void SetCollisionMesh()
        {
            //Debug.Log("Updating collision mesh");
        
            if (hasSetCollider)
                return;
        
            if (!lodMeshes[colliderLODIndex].hasRequestedMesh)
            {
                lodMeshes[colliderLODIndex].RequestMesh(heightMap, mapGenerator.threadedDataRequester, mapGenerator.mapConfiguration);
            }
            else if (lodMeshes[colliderLODIndex].hasMesh)
            {
                meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
                hasSetCollider = true;
            }
        }

        /// <summary>
        /// Sets a new state for the visibility of the Mesh
        /// </summary>
        /// <param name="state">True if the new state is "visible", false if it is "not visible."</param>
        public void SetMeshVisible(bool state)
        {
            visualMeshObject.SetActive(state);
        }

        /// <summary>
        /// Checks if the mesh of the TerrainChunk is currently visible
        /// </summary>
        /// <returns>True if visible, false otherwise</returns>
        public bool IsMeshVisible()
        {
            return visualMeshObject.activeSelf;
        }
    }

    /// <summary>
    /// A Mesh holder with related data meant to be used with an specified LOD
    /// </summary>
    class LODMesh
    {
        /// <summary>
        /// The mesh itself
        /// </summary>
        public Mesh mesh;
    
        /// <summary>
        /// If the mesh has been requested to a thread
        /// </summary>
        public bool hasRequestedMesh;
    
        /// <summary>
        /// If the mesh has been received after requesting it
        /// </summary>
        public bool hasMesh;
    
        /// <summary>
        /// The LOD related to this mesh
        /// </summary>
        private int lod;
    
        /// <summary>
        /// The callback action to do after receiving the requested mesh data
        /// </summary>
        public event System.Action updateCallback;

        /// <summary>
        /// Constructor of a Mesh holder with related data meant to be used with an specified LOD
        /// </summary>
        /// <param name="lod">The LOD related to the mesh</param>
        public LODMesh(int lod)
        {
            this.lod = lod;
        }

        /// <summary>
        /// Creates and stores a mesh from a MeshData object. This is automatically called once the MeshData is received after requesting it.
        /// </summary>
        /// <param name="meshData">A MeshData object from which the Mesh will be created</param>
        private void OnMeshDataReceived(object meshData)
        {
            //Debug.Log($"OnMeshDataReceived");
            mesh = ((MeshData)meshData).CreateMesh();
            hasMesh = true;
            updateCallback();
        }
    
        /// <summary>
        /// Requests data to automatically create and store a Mesh in this LODMesh object.
        /// </summary>
        /// <param name="heightMap">The HeightMap to use to create the Mesh</param>
        /// <param name="threadedDataRequester">The manager of the threaded requests to manage this one</param>
        /// <param name="mapConfiguration">The MapConfiguration with information to define the requested Mesh</param>
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
}