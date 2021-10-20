using System.Collections.Generic;
using UnityEngine;

namespace Thoughts.Game.Map.Terrain
{
    /// <summary>
    /// Unity Component in charge of generating a terrain in the scene 
    /// </summary>
    public class TerrainGenerator : MonoBehaviour
    {
        /// <summary>
        /// An specific prefab that will be cloned as needed to display the terrain.
        /// </summary>
        [SerializeField]
        [Tooltip("An specific prefab that will be cloned as needed to display the terrain.")]
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
        [SerializeField] private int colliderLODIndex;
    
        /// <summary>
        /// An ordered list containing the LOD with the information about which one should be used until which distance. The LOD "0", has the maximum level of detail. The last threshold/distance in the list will be considered as the maximum view distance from the viewer's perspective.
        /// </summary>
        [Tooltip("What LOD should be used until which distance. The LOD '0', has the maximum level of detail. The last threshold/distance in the list will be considered as the maximum view distance from the viewer's perspective.")]
        [SerializeField] public LODInfo[] detailLevels;

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

        /// <summary>
        /// The latest known position of the viewer.
        /// </summary>
        Vector2 viewerPosition;
        /// <summary>
        /// The previous to the latest known position of the viewer.
        /// </summary>
        Vector2 viewerPositionOld;
        /// <summary>
        /// Reference to the mapGenerator managing the generation of the map that contains this generator's terrain.
        /// </summary>
        [Tooltip("Reference to the mapGenerator managing the generation of the map that contains this generator's terrain.")]
        public MapGenerator mapGenerator;
    
        /// <summary>
        /// How many chunks exist in the map
        /// </summary>
        private int totalChunksInMap => Mathf.RoundToInt( mapGenerator.mapConfiguration.mapRadius ) * 2;

        /// <summary>
        /// A reference to all spawned GameObjects containing a TerrainChunk / Terrain chunks
        /// </summary>
        private Dictionary<Vector2, TerrainChunk> terrainChunks = new Dictionary<Vector2, TerrainChunk>();

        /// <summary>
        /// Sets the viewer to the mainCamera if it has not been set during the Awake calls
        /// </summary>
        private void Start()
        {
            if (viewer == null)
            {
                Camera mainCamera = Camera.main;
                if (mainCamera)
                    viewer = mainCamera.transform;
                else
                    Debug.LogWarning("The main camera has not been found to be set as the terrain' viewer.", this);

                Debug.Log($"EndlessTerrain viewer automatically set at Start to '{viewer}'", viewer);
            }

            // UpdateVisibleChunks();
        }

        /// <summary>
        /// Sends the order to update the TerrainChunk if the viewer has moved a distance bigger than the configured threshold
        /// </summary>
        private void Update()
        {
            //Todo: make it only active if it has been "manually set to active" (by MapGenerator for example).
        
            Vector3 currentPosition = viewer.position;
            viewerPosition = viewer != null ? new Vector2(currentPosition.x, currentPosition.z) : Vector2.zero;
        
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
        public void UpdateChunks(bool clearPreviousTerrain)
        {
            Debug.Log($"Creating and/or updating TerrainChunks {(clearPreviousTerrain? "previously deleting" : "without destroying")} the existing ones.");
        
            if (clearPreviousTerrain)
                DeleteTerrain();
        
            int currentViewerChunkCordX = Mathf.RoundToInt(viewerPosition.x / mapGenerator.mapConfiguration.meshWorldSize);
            int currentViewerChunkCordY = Mathf.RoundToInt(viewerPosition.y / mapGenerator.mapConfiguration.meshWorldSize);
            int halfOfChunksInMap = totalChunksInMap / 2;
            for (int yOffset = -halfOfChunksInMap; yOffset <= halfOfChunksInMap; yOffset ++)
            {
                for (int xOffset = -halfOfChunksInMap; xOffset <= halfOfChunksInMap; xOffset ++)
                {
                    Vector2 coordOfCurrentlyCheckingChunk = new Vector2(currentViewerChunkCordX + xOffset, currentViewerChunkCordY + yOffset);

                    //if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord)) {
                    if (terrainChunks.ContainsKey(coordOfCurrentlyCheckingChunk))
                    {
                        terrainChunks[coordOfCurrentlyCheckingChunk].UpdateChunk();
                    }
                    else
                    {
                        GameObject chunkGameObject = Instantiate(chunkPrefab);
                        TerrainChunk terrainChunk = chunkGameObject.GetComponentRequired<TerrainChunk>();
                        terrainChunk.Setup(coordOfCurrentlyCheckingChunk, detailLevels, colliderLODIndex, this.transform, viewer, mapGenerator, mapGenerator.mapConfiguration.textureSettings.material);
                        terrainChunks.Add(coordOfCurrentlyCheckingChunk, terrainChunk);
                        //newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
                        terrainChunk.Load();
                    }
                    //}
                }
            }
        }
    
        /// <summary>
        /// Destroys all the references to the TerrainChunks and the GameObjects themselves
        /// </summary>
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

    /// <summary>
    /// Data that relates a LOD with the distance to the viewer at which it should stop being used and a worse LOD should be used instead.
    /// </summary>
    [System.Serializable]
    public struct LODInfo
    {
        /// <summary>
        /// The LOD linked to the visibleDistanceThreshold data
        /// </summary>
        [Range(0,HeightMapSettings.numSupportedTerrainLODs-1)]
        [SerializeField]public int lod;
        /// <summary>
        /// The distance to the viewer at which it should stop being used and a worse LOD should be used instead
        /// <para>Once the viewer is outside of the threshold, it will switch over to the next level of detail lower resolution version)</para>
        /// </summary>
        [Tooltip("The distance to the viewer at which it should stop being used and a worse LOD should be used instead. Once the viewer is outside of the threshold, it will switch over to the next level of detail lower resolution version)")]
        [SerializeField] public float visibleDistanceThreshold;
    
    }
}