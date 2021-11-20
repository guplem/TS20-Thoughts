using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Thoughts.Game.Map.CreationSteps.Terrain
{
    /// <summary>
    /// Unity Component in charge of generating a terrain in the scene 
    /// </summary>
    public class TerrainGenerator : CreationStepGenerator
    {
        /// <summary>
        /// An specific prefab that will be cloned as needed to display the terrain.
        /// </summary>
        [SerializeField]
        [Tooltip("An specific prefab that will be cloned as needed to display the terrain.")]
        [AssetsOnly]
        private GameObject chunkPrefab;
    
        /// <summary>
        /// Amount of distance that the viewer must move to update the chunks
        /// </summary>
        private const float viewerMoveThresholdForChunkUpdate = 25f;
        /// <summary>
        /// The squared amount of distance  that the viewer must move to update the chunks
        /// </summary>
        private const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;
    
        /// <summary>
        /// The LOD that the collider of the terrain must use
        /// </summary>
        [Tooltip("The LOD that the collider of the terrain must use")]
        [SerializeField] public int colliderLODIndex;
    
        /// <summary>
        /// An ordered list containing the LOD with the information about which one should be used until which distance. The LOD "0", has the maximum level of detail. The last threshold/distance in the list will be considered as the maximum view distance from the viewer's perspective.
        /// </summary>
        [Tooltip("What LOD should be used until which distance. The LOD '0', has the maximum level of detail. The last threshold/distance in the list will be considered as the maximum view distance from the viewer's perspective.")]
        [SerializeField] public LODInfo[] detailLevels;

        /// <summary>
        /// The layer mask of the terrain
        /// </summary>
        [Tooltip("The layer mask of the terrain")]
        [SerializeField] private LayerMask _terrainLayerMask;
        public LayerMask terrainLayerMask => _terrainLayerMask;
        
        /// <summary>
        /// Reference to the viewer (usually the player) of the terrain. If null, it will be set to 'Camera.main' when requested.
        /// </summary>
        public Transform viewer {
            get
            {
                if (_viewer == null)
                {
                    Camera mainCamera = Camera.main;
                    if (mainCamera)
                        _viewer = mainCamera.transform;
                    else
                        Debug.LogWarning("The main camera has not been found to be set as the terrain' viewer.", this);

                    Debug.Log($"EndlessTerrain viewer automatically set to '{_viewer}'", _viewer);
                }
                return _viewer;
            }
            set
            {
                _viewer = value;
            }
        }
        public Transform _viewer;

        /// <summary>
        /// The latest known position of the viewer.
        /// </summary>
        Vector2 viewerPosition;
        /// <summary>
        /// The previous to the latest known position of the viewer.
        /// </summary>
        Vector2 viewerPositionOld;

        /// <summary>
        /// How many chunks exist in a single row of the map
        /// </summary>
        private int totalChunksInMapRow => 1 + Mathf.RoundToInt( mapManager.mapConfiguration.mapRadius ) * 2 / MapConfiguration.supportedChunkSizes[mapManager.mapConfiguration.chunkSizeIndex];
        
        public int terrainSeed => _randomNumberToAlterMainSeed + mapManager.mapConfiguration.seed; //IT MUST NEVER CHANGE
        private const int _randomNumberToAlterMainSeed = 84624; //IT MUST NEVER CHANGE and be completely unique per generator (except the mapGenerator and those that do not need randomness)
        
        /// <summary>
        /// A reference to all spawned GameObjects containing a TerrainChunk with its relative chunk coords
        /// </summary>
        private Dictionary<Vector2Int, TerrainChunk> terrainChunks = new Dictionary<Vector2Int, TerrainChunk>();

        /// <summary>
        /// The amount of 
        /// </summary>
        private int loadingChunks;
        
        [SerializeField] private Transform seaVisuals;

        /// <summary>
        /// The callback action to do after fully loading the terrain
        /// </summary>
        public event System.Action terrainFullyLoadCallback;
        

        /// <summary>
        /// Sends the order to update the TerrainChunk if the viewer has moved a distance bigger than the configured threshold
        /// </summary>
        private void Update()
        {
            viewerPosition = viewer != null ? new Vector2(viewer.position.x, viewer.position.z) : Vector2.zero;
        
            if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
            {
                viewerPositionOld = viewerPosition;
                UpdateChunks(false);
            }
        }

        /// <summary>
        /// Updates the TerrainChunks by creating it (if missing) or updating its visuals.
        /// </summary>
        /// <param name="clearPreviousTerrain">If existent, should the previously created terrain be deleted?</param>
        private void UpdateChunks(bool clearPreviousTerrain)
        {
            //Debug.Log($"Creating and/or updating TerrainChunks {(clearPreviousTerrain? "previously deleting" : "without destroying")} the existing ones.");
            
            if (clearPreviousTerrain)
            {
                Delete();
            }
            int chunksAtSideOfCentralRow = totalChunksInMapRow / 2;
            loadingChunks = totalChunksInMapRow*totalChunksInMapRow;
            
            terrainFullyLoadCallback += OnTerrainFullyLoad;
            int currentViewerChunkCordX = Mathf.RoundToInt(viewerPosition.x / mapManager.mapConfiguration.chunkWorldSize);
            int currentViewerChunkCordY = Mathf.RoundToInt(viewerPosition.y / mapManager.mapConfiguration.chunkWorldSize);
            for (int yOffset = -chunksAtSideOfCentralRow; yOffset <= chunksAtSideOfCentralRow; yOffset ++)
            {
                for (int xOffset = -chunksAtSideOfCentralRow; xOffset <= chunksAtSideOfCentralRow; xOffset ++)
                {
                    //Debug.Log("XXX");
                    Vector2Int chunkIndex = new Vector2Int(currentViewerChunkCordX + xOffset, currentViewerChunkCordY + yOffset);

                    //if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord)) {
                    if (terrainChunks.ContainsKey(chunkIndex))
                    {
                        terrainChunks[chunkIndex].UpdateChunk();
                    }
                    else
                    {
                        GameObject chunkGameObject = Instantiate(chunkPrefab);
                        TerrainChunk terrainChunk = chunkGameObject.GetComponentRequired<TerrainChunk>();
                        terrainChunk.Setup(chunkIndex, detailLevels, this.transform, mapManager);
                        terrainChunks.Add(chunkIndex, terrainChunk);
                        //newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
                        terrainChunk.Load(CompletionRegisterer);
                        chunkGameObject.name = $"Chunk {chunkIndex} ({chunkGameObject.transform.position})";
                    }
                    //}
                }
            }
        }
        /// <summary>
        /// Registers the competition of the generation/loading of a chunk. If no more chunks are left to load, calls the terrainFullyLoadCallback
        /// </summary>
        private void CompletionRegisterer()
        {
            loadingChunks -= 1;
            //Debug.Log($"COMPETITION REGISTER. REMAINING: {loadingChunks}");
            if (loadingChunks > 0)
                return;
            if (loadingChunks < 0)
            {
                //Debug.LogWarning($"The count of loading chunks shouldn't be negative but it is {loadingChunks}. Setting it to 0.");
                loadingChunks = 0;
            }
            terrainFullyLoadCallback?.Invoke();
        }

        /// <summary>
        /// Notifies the end of the loading of the terrain
        /// </summary>
        private void OnTerrainFullyLoad()
        {
            terrainFullyLoadCallback -= OnTerrainFullyLoad;
            // Debug.Log($"=========== TERRAIN FULLY LOAD ===========. Next step? {generateNextStepOnFinish}");

            if (generateNextStepOnFinishGeneration)
                base.InvokeOnFinishStepGeneration();
        }

        private Vector2Int GetArrayCoordsOfSea(TerrainType[,] terrainTypes)
        {
            for (int x = -mapManager.mapConfiguration.mapRadius; x <= mapManager.mapConfiguration.mapRadius; x++)
            {
                for (int y = -mapManager.mapConfiguration.mapRadius; y <= mapManager.mapConfiguration.mapRadius; y++)
                {
                    Vector2Int arrayCoords = new Vector2Int(x + mapManager.mapConfiguration.mapRadius, y + mapManager.mapConfiguration.mapRadius);
                    bool seaFound = terrainTypes[arrayCoords.x, arrayCoords.y] == TerrainType.sea;
                    if (seaFound)
                        return arrayCoords;
                }
            }
            Debug.LogWarning("No sea has been found.");
            return Vector2Int.zero;
        }

        public float GetHeightAt(Vector2 worldCoords)
        {
            TerrainChunk chunk = terrainChunks[mapManager.mapGenerator.GetRelativeChunksCoordsAt(worldCoords)];
            
            /*Vector2 absoluteChunkCoords = chunk.transform.position.ToVector2WithoutY();
            float chunkAbsoluteSize = mapGenerator.mapConfiguration.chunkWorldSize;
            Vector2 distanceFromChunkCenter = new Vector2(worldCoords.x - absoluteChunkCoords.x, worldCoords.y - absoluteChunkCoords.y);
            float chunkAbsoluteWidth = mapGenerator.mapConfiguration.chunkWorldSize;
            int heightMapWidth = chunk.heightMapAbsolute.values.GetLength(0);
            int heightMapCoordsX = Mathf.RoundToInt(chunkAbsoluteWidth / heightMapWidth * distanceFromChunkCenter.x + heightMapWidth / 2f);//(int)(coords.x * chunkAbsoluteSize / 2f / absoluteChunkCoords.x);
            int heightMapCoordsY = Mathf.RoundToInt(heightMapWidth - (chunkAbsoluteWidth / heightMapWidth * distanceFromChunkCenter.y + heightMapWidth / 2f));//(int)(coords.y * chunkAbsoluteSize / 2f / absoluteChunkCoords.y);
            //Debug.Log($"CHECKING HEIGHT AT {heightMapCoordsX}, {heightMapCoordsY}. coords = {coords}, chunkAbsoluteSize = {chunkAbsoluteSize}, absoluteChunkCoords = {absoluteChunkCoords}");
            return chunk.heightMapAbsolute.values[heightMapCoordsX,heightMapCoordsY];*/

            return TerrainMeshGenerator.GetHeight(chunk.heightMapAbsolute.values, mapManager.mapConfiguration,  worldCoords - chunk.transform.position.ToVector2WithoutY());
        }


        /// <summary>
        /// Destroys all the references to the TerrainChunks and the GameObjects themselves
        /// </summary>
        protected override void _DeleteStep()
        {
            terrainChunks.Clear();
            //terrainChunks = new Dictionary<Vector2, TerrainChunk>();
            
            if (Application.isPlaying)
                gameObject.transform.DestroyAllChildren(); 
            else
                gameObject.transform.DestroyImmediateAllChildren();
            
            
            InvokeOnFinishStepDeletion();
        }
        
        protected override void _GenerateStep(bool clearPrevious)
        {
            ReconfigureSea();
            UpdateChunks(clearPrevious);
        }
        
        private void ReconfigureSea()
        {
            float seaHeight = mapManager.mapConfiguration.seaHeightAbsolute;
            seaVisuals.transform.position = Vector3.zero.WithY(seaHeight);
            float seaSize = mapManager.mapConfiguration.mapRadius * 2 * 2;
            seaVisuals.transform.SetGlobalScale(new Vector3(seaSize, 0.05f, seaSize));
        }
        
        
    }

    /// <summary>
    /// Data that relates a LOD with the distance to the viewer at which it should stop being used and a worse LOD should be used instead.
    /// </summary>
    [System.Serializable]
    public struct LODInfo
    {
        /// <summary>
        /// The LOD linked to the visibleDistanceThreshold data
        /// </summary>
        [Range(0,TerrainHeightSettings.numSupportedTerrainLODs-1)]
        [SerializeField]public int lod;
        /// <summary>
        /// The distance to the viewer at which it should stop being used and a worse LOD should be used instead
        /// <para>Once the viewer is outside of the threshold, it will switch over to the next level of detail lower resolution version)</para>
        /// </summary>
        [Tooltip("The distance to the viewer at which it should stop being used and a worse LOD should be used instead. Once the viewer is outside of the threshold, it will switch over to the next level of detail lower resolution version)")]
        [SerializeField] public float visibleDistanceThreshold;
    
    }
}