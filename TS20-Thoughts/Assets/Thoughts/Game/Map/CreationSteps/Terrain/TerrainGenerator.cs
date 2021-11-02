using System;
using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.Map.MapElements;
using UnityEngine;
using Console = System.Console;

namespace Thoughts.Game.Map.Terrain
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

        [SerializeField] private LayerMask terrainLayerMask;
        
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
        /// How many chunks exist in a single row of the map
        /// </summary>
        private int totalChunksInMapRow => 1 + Mathf.RoundToInt( mapGenerator.mapConfiguration.mapRadius ) * 2 / MapConfiguration.supportedChunkSizes[mapGenerator.mapConfiguration.chunkSizeIndex];
        
        /// <summary>
        /// A reference to all spawned GameObjects containing a TerrainChunk with its relative chunk coords
        /// </summary>
        private Dictionary<Vector2Int, TerrainChunk> terrainChunks = new Dictionary<Vector2Int, TerrainChunk>();

        /// <summary>
        /// The amount of 
        /// </summary>
        private int loadingChunks;
        

        /// <summary>
        /// The callback action to do after fully loading the terrain
        /// </summary>
        public event System.Action terrainFullyLoadCallback;
        
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
        private void UpdateChunks(bool clearPreviousTerrain)
        {
            //Debug.Log($"Creating and/or updating TerrainChunks {(clearPreviousTerrain? "previously deleting" : "without destroying")} the existing ones.");
            
            if (clearPreviousTerrain)
            {
                DeleteTerrain();
            }
            int chunksAtSideOfCentralRow = totalChunksInMapRow / 2;
            loadingChunks = totalChunksInMapRow*totalChunksInMapRow;
            
            terrainFullyLoadCallback += OnTerrainFullyLoad;
            int currentViewerChunkCordX = Mathf.RoundToInt(viewerPosition.x / mapGenerator.mapConfiguration.chunkWorldSize);
            int currentViewerChunkCordY = Mathf.RoundToInt(viewerPosition.y / mapGenerator.mapConfiguration.chunkWorldSize);
            for (int yOffset = -chunksAtSideOfCentralRow; yOffset <= chunksAtSideOfCentralRow; yOffset ++)
            {
                for (int xOffset = -chunksAtSideOfCentralRow; xOffset <= chunksAtSideOfCentralRow; xOffset ++)
                {
                    //Debug.Log("XXX");
                    Vector2Int chunkCoordOfCurrentChunk = new Vector2Int(currentViewerChunkCordX + xOffset, currentViewerChunkCordY + yOffset);

                    //if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord)) {
                    if (terrainChunks.ContainsKey(chunkCoordOfCurrentChunk))
                    {
                        terrainChunks[chunkCoordOfCurrentChunk].UpdateChunk();
                    }
                    else
                    {
                        GameObject chunkGameObject = Instantiate(chunkPrefab);
                        TerrainChunk terrainChunk = chunkGameObject.GetComponentRequired<TerrainChunk>();
                        terrainChunk.Setup(chunkCoordOfCurrentChunk, detailLevels, colliderLODIndex, this.transform, viewer, mapGenerator, mapGenerator.mapConfiguration.terrainTextureSettings.material);
                        terrainChunks.Add(chunkCoordOfCurrentChunk, terrainChunk);
                        //newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
                        terrainChunk.Load(CompletionRegisterer);
                        chunkGameObject.name = $"Chunk {chunkCoordOfCurrentChunk} ({chunkGameObject.transform.position})";
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
            Debug.Log($"=========== TERRAIN FULLY LOAD ===========. Next step? {generateNextStepOnFinish}");
            //StartCoroutine(nameof(SpawnWaterSources));
            SpawnWaterSources();
            if (generateNextStepOnFinish)
                base.InvokeOnFinishStepGeneration();
        }
        
        private enum TerrainType
        {
            none,
            sea,
            interior,
            interiorShoreline,
            land,
        }
        [SerializeField] private Transform waterSourceParent;
        [SerializeField] private GameObject waterSourcePrefab;
        
        private void SpawnWaterSources()
        {
            TerrainType[,] terrainTypes = new TerrainType[mapGenerator.mapConfiguration.mapRadius * 2 +1, mapGenerator.mapConfiguration.mapRadius * 2 +1];

            for (int x = -mapGenerator.mapConfiguration.mapRadius; x <= mapGenerator.mapConfiguration.mapRadius; x++)
            {
                for (int y = -mapGenerator.mapConfiguration.mapRadius; y <= mapGenerator.mapConfiguration.mapRadius; y++)
                {
                    Vector2Int arrayCoords = new Vector2Int(x + mapGenerator.mapConfiguration.mapRadius, y + mapGenerator.mapConfiguration.mapRadius);
                    if (terrainTypes[arrayCoords.x,arrayCoords.y] != TerrainType.none)
                        continue;
                    if (!IsLocationUnderWater(new Vector2(x, y)))
                    {
                        terrainTypes[arrayCoords.x, arrayCoords.y] = TerrainType.land;
                        Debug.DrawRay(new Vector3(x,GetHeightAt(new Vector2(x,y)),y),Vector3.up, Color.yellow, 25f );
                    }
                    else
                    {
                        if (arrayCoords.x == 0 || arrayCoords.y == 0 || arrayCoords.x == terrainTypes.GetLength(0) - 1 || arrayCoords.y == terrainTypes.GetLength(1) - 1)
                        {
                            terrainTypes[arrayCoords.x, arrayCoords.y] = TerrainType.sea;
                            Debug.DrawRay(new Vector3(x,GetHeightAt(new Vector2(x,y)),y),Vector3.up, Color.magenta, 25f );
                        }
                    }
                }                
            }

            terrainTypes = PropagateSea(terrainTypes);
            
            for (int x = -mapGenerator.mapConfiguration.mapRadius; x <= mapGenerator.mapConfiguration.mapRadius; x++)
            {
                for (int y = -mapGenerator.mapConfiguration.mapRadius; y <= mapGenerator.mapConfiguration.mapRadius; y++)
                {
                    Vector2Int arrayCoords = new Vector2Int(x + mapGenerator.mapConfiguration.mapRadius, y + mapGenerator.mapConfiguration.mapRadius);
                    if (terrainTypes[arrayCoords.x,arrayCoords.y] != TerrainType.none)
                        continue;
                    if (IsLocationUnderWater(new Vector2(x, y)))
                    {
                        TerrainType topLeftTerrainType = TerrainType.none;
                            try { topLeftTerrainType = terrainTypes[x - 1, y + 1];
                            } catch (Exception) { /* ignored */ }
                        TerrainType topRightTerrainType = TerrainType.none;
                            try { topRightTerrainType = terrainTypes[x + 1, y + 1];
                            } catch (Exception) { /* ignored */ }
                        TerrainType bottomLeftTerrainType = TerrainType.none;
                            try { bottomLeftTerrainType = terrainTypes[x - 1, y - 1];
                            } catch (Exception) { /* ignored */ }
                        TerrainType bottomRightTerrainType = TerrainType.none;
                            try { bottomRightTerrainType = terrainTypes[x + 1, y - 1];
                            } catch (Exception) { /* ignored */ }

                        bool isAnyNeighbourInterior = topLeftTerrainType == TerrainType.interior || topRightTerrainType == TerrainType.interior || bottomLeftTerrainType == TerrainType.interior || bottomRightTerrainType == TerrainType.interior;
                        bool isAnyNeighbourLand = topLeftTerrainType == TerrainType.land || topRightTerrainType == TerrainType.land || bottomLeftTerrainType == TerrainType.land || bottomRightTerrainType == TerrainType.land;
                        
                        if (isAnyNeighbourLand || isAnyNeighbourInterior)
                        {
                            terrainTypes[arrayCoords.x, arrayCoords.y] = TerrainType.interior;
                            Debug.DrawRay(new Vector3(x,GetHeightAt(new Vector2(x,y)),y),Vector3.up, Color.red, 25f );
                        }
                    }
                }                
            }

            for (int x = -mapGenerator.mapConfiguration.mapRadius; x <= mapGenerator.mapConfiguration.mapRadius; x++)
            {
                for (int y = -mapGenerator.mapConfiguration.mapRadius; y <= mapGenerator.mapConfiguration.mapRadius; y++)
                {
                    Vector2Int arrayCoords = new Vector2Int(x + mapGenerator.mapConfiguration.mapRadius, y + mapGenerator.mapConfiguration.mapRadius);
                    if (terrainTypes[arrayCoords.x, arrayCoords.y] == TerrainType.interior)
                    {
                        //MapElement spawned = mapGenerator.SpawnMapElement(waterSourcePrefab, new Vector3(x, GetHeightAt(new Vector2(x, y)), y), Quaternion.identity, waterSourceParent);
                        //Debug.Log("SPAWN", spawned.gameObject);
                    }
                }
            }
        }

        private Vector2Int GetArrayCoordsOfSea(TerrainType[,] terrainTypes)
        {
            for (int x = -mapGenerator.mapConfiguration.mapRadius; x <= mapGenerator.mapConfiguration.mapRadius; x++)
            {
                for (int y = -mapGenerator.mapConfiguration.mapRadius; y <= mapGenerator.mapConfiguration.mapRadius; y++)
                {
                    Vector2Int arrayCoords = new Vector2Int(x + mapGenerator.mapConfiguration.mapRadius, y + mapGenerator.mapConfiguration.mapRadius);
                    bool seaFound = terrainTypes[arrayCoords.x, arrayCoords.y] == TerrainType.sea;
                    if (seaFound)
                        return arrayCoords;
                }
            }
            Debug.LogWarning("No sea has been found.");
            return Vector2Int.zero;
        }
        
        //The given coords must have been defined as sea in the given 'terrainTypes'
        private TerrainType[,] PropagateSea(TerrainType[,] terrainTypes)
        {
            bool waterAddedOnLastPass = false;
            do
            {
                waterAddedOnLastPass = false;
                
                for (int x = -mapGenerator.mapConfiguration.mapRadius+1; x <= mapGenerator.mapConfiguration.mapRadius-1; x++)
                {
                    for (int y = -mapGenerator.mapConfiguration.mapRadius+1; y <= mapGenerator.mapConfiguration.mapRadius-1; y++)
                    {
                        Vector2Int arrayCoords = new Vector2Int(x + mapGenerator.mapConfiguration.mapRadius, y + mapGenerator.mapConfiguration.mapRadius);

                        if (terrainTypes[arrayCoords.x,arrayCoords.y] != TerrainType.none)
                            continue;

                        //try {
                            terrainTypes[arrayCoords.x, arrayCoords.y] = (terrainTypes[arrayCoords.x + 1, arrayCoords.y + 1] == TerrainType.sea) ? TerrainType.sea : TerrainType.none;
                            terrainTypes[arrayCoords.x, arrayCoords.y] = (terrainTypes[arrayCoords.x + 1, arrayCoords.y + 0] == TerrainType.sea) ? TerrainType.sea : TerrainType.none;
                            terrainTypes[arrayCoords.x, arrayCoords.y] = (terrainTypes[arrayCoords.x + 1, arrayCoords.y - 1] == TerrainType.sea) ? TerrainType.sea : TerrainType.none;
                            terrainTypes[arrayCoords.x, arrayCoords.y] = (terrainTypes[arrayCoords.x + 0, arrayCoords.y - 1] == TerrainType.sea) ? TerrainType.sea : TerrainType.none;
                            terrainTypes[arrayCoords.x, arrayCoords.y] = (terrainTypes[arrayCoords.x + 0, arrayCoords.y + 1] == TerrainType.sea) ? TerrainType.sea : TerrainType.none;
                            terrainTypes[arrayCoords.x, arrayCoords.y] = (terrainTypes[arrayCoords.x - 1, arrayCoords.y + 1] == TerrainType.sea) ? TerrainType.sea : TerrainType.none;
                            terrainTypes[arrayCoords.x, arrayCoords.y] = (terrainTypes[arrayCoords.x - 1, arrayCoords.y - 0] == TerrainType.sea) ? TerrainType.sea : TerrainType.none;
                            terrainTypes[arrayCoords.x, arrayCoords.y] = (terrainTypes[arrayCoords.x - 1, arrayCoords.y - 1] == TerrainType.sea) ? TerrainType.sea : TerrainType.none;
                        //} catch (Exception) { }


                        if (terrainTypes[arrayCoords.x, arrayCoords.y] == TerrainType.sea) {}
                            waterAddedOnLastPass = true;
                    }
                }
            }
            while (waterAddedOnLastPass);


            /*bool success = false;
            if (pendingLocations == null)
                pendingLocations = new List<Vector2Int>();
            if (!success)
                terrainTypes = CheckSetAndPropagateSea(terrainTypes, arrayCoordsWithSea+Vector2Int.up, pendingLocations, out success);
            else 
                pendingLocations.Add(arrayCoordsWithSea+Vector2Int.up);
            if (!success)
                terrainTypes = CheckSetAndPropagateSea(terrainTypes, arrayCoordsWithSea+Vector2Int.down, pendingLocations, out success);
            else 
                pendingLocations.Add(arrayCoordsWithSea+Vector2Int.down);
            if (!success)
                terrainTypes = CheckSetAndPropagateSea(terrainTypes, arrayCoordsWithSea+Vector2Int.right, pendingLocations, out success);
            else 
                pendingLocations.Add(arrayCoordsWithSea+Vector2Int.right);
            if (!success)
                terrainTypes = CheckSetAndPropagateSea(terrainTypes, arrayCoordsWithSea+Vector2Int.left, pendingLocations, out success);
            else 
                pendingLocations.Add(arrayCoordsWithSea+Vector2Int.left);
            if (!success)
                terrainTypes = CheckSetAndPropagateSea(terrainTypes, arrayCoordsWithSea+Vector2Int.up+Vector2Int.right, pendingLocations, out success);
            else 
                pendingLocations.Add(arrayCoordsWithSea+Vector2Int.up+Vector2Int.right);
            if (!success)
                terrainTypes = CheckSetAndPropagateSea(terrainTypes, arrayCoordsWithSea+Vector2Int.up+Vector2Int.left, pendingLocations, out success);
            else 
                pendingLocations.Add(arrayCoordsWithSea+Vector2Int.up+Vector2Int.left);
            if (!success)
                terrainTypes = CheckSetAndPropagateSea(terrainTypes, arrayCoordsWithSea+Vector2Int.down+Vector2Int.left, pendingLocations, out success);
            else 
                pendingLocations.Add(arrayCoordsWithSea+Vector2Int.down+Vector2Int.left);
            if (!success)
                terrainTypes = CheckSetAndPropagateSea(terrainTypes, arrayCoordsWithSea+Vector2Int.down+Vector2Int.right, pendingLocations, out success);
            else 
                pendingLocations.Add(arrayCoordsWithSea+Vector2Int.down+Vector2Int.right);

            
            /*if (!seaFound)
                Debug.LogError("No sea found on the map to propagate.");
            else if (terrainTypes[coordsOfInitialSeaArray.x, coordsOfInitialSeaArray.y] != TerrainType.sea)
                Debug.LogError($"The provided coords {coordsOfInitialSea} are not defined as {TerrainType.sea}, they are defined as {terrainTypes[coordsOfInitialSeaArray.x, coordsOfInitialSeaArray.y]} instead.");


            bool propagated = false;
*/
            /*try
            {
                if (!propagated && terrainTypes[coordsOfInitialSeaArray.x - 1, coordsOfInitialSeaArray.y] == TerrainType.none)
                {
                    terrainTypes = SetAsSeaIfPossibleAndPropagate(terrainTypes, new Vector2(coordsOfInitialSea.x - 1, coordsOfInitialSea.y), iteration + 1);
                    propagated = true;
                    Debug.Log("PROPAGATED" + $"{new Vector2Int(coordsOfInitialSeaArray.x - 1, coordsOfInitialSeaArray.y)} W:{new Vector2(coordsOfInitialSea.x - 1, coordsOfInitialSea.y)}");
                }
                else
                {
                    Debug.Log($"propagated = {propagated}, terrainType = {terrainTypes[coordsOfInitialSeaArray.x - 1, coordsOfInitialSeaArray.y]}");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message + $"{new Vector2Int(coordsOfInitialSeaArray.x - 1, coordsOfInitialSeaArray.y)} W:{new Vector2(coordsOfInitialSea.x - 1, coordsOfInitialSea.y)}");
            }


            try
            {
                if (!propagated && terrainTypes[coordsOfInitialSeaArray.x + 1, coordsOfInitialSeaArray.y] == TerrainType.none)
                {
                    terrainTypes = SetAsSeaIfPossibleAndPropagate(terrainTypes, new Vector2(coordsOfInitialSea.x + 1, coordsOfInitialSea.y), iteration + 1);
                    propagated = true;
                    Debug.Log("PROPAGATED" + $"{new Vector2Int(coordsOfInitialSeaArray.x + 1, coordsOfInitialSeaArray.y)} W:{new Vector2(coordsOfInitialSea.x + 1, coordsOfInitialSea.y)}");
                }
                else
                {
                    Debug.Log($"propagated = {propagated}, terrainType = {terrainTypes[coordsOfInitialSeaArray.x + 1, coordsOfInitialSeaArray.y]}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message + $"{new Vector2Int(coordsOfInitialSeaArray.x + 1, coordsOfInitialSeaArray.y)} W:{new Vector2(coordsOfInitialSea.x + 1, coordsOfInitialSea.y)}");
            }

            try
            {
                if (!propagated && terrainTypes[coordsOfInitialSeaArray.x, coordsOfInitialSeaArray.y - 1] == TerrainType.none)
                {
                    terrainTypes = SetAsSeaIfPossibleAndPropagate(terrainTypes, new Vector2(coordsOfInitialSea.x, coordsOfInitialSea.y - 1), iteration + 1);
                    propagated = true;
                    Debug.Log("PROPAGATED" + $"{new Vector2Int(coordsOfInitialSeaArray.x, coordsOfInitialSeaArray.y - 1)} W:{new Vector2(coordsOfInitialSea.x, coordsOfInitialSea.y - 1)}");
                }
                else
                {
                    Debug.Log($"propagated = {propagated}, terrainType = {terrainTypes[coordsOfInitialSeaArray.x, coordsOfInitialSeaArray.y - 1]}");
                }
            }
            catch (Exception exc)
            {
                Debug.LogWarning(exc.Message + $"{new Vector2Int(coordsOfInitialSeaArray.x, coordsOfInitialSeaArray.y - 1)} W:{new Vector2(coordsOfInitialSea.x, coordsOfInitialSea.y - 1)}");
            }

            try
            {
                if (!propagated && terrainTypes[coordsOfInitialSeaArray.x, coordsOfInitialSeaArray.y + 1] == TerrainType.none)
                {
                    terrainTypes = SetAsSeaIfPossibleAndPropagate(terrainTypes, new Vector2(coordsOfInitialSea.x, coordsOfInitialSea.y + 1), iteration + 1);
                    propagated = true;
                    Debug.Log("PROPAGATED" + $"{new Vector2Int(coordsOfInitialSeaArray.x, coordsOfInitialSeaArray.y + 1)} W:{new Vector2(coordsOfInitialSea.x, coordsOfInitialSea.y + 1)}");
                }
                else
                {
                    Debug.Log($"propagated = {propagated}, terrainType = {terrainTypes[coordsOfInitialSeaArray.x, coordsOfInitialSeaArray.y - 1]}");
                }
            }
            catch (Exception exce)
            {
                Debug.LogWarning(exce.Message + $"{new Vector2Int(coordsOfInitialSeaArray.x, coordsOfInitialSeaArray.y + 1)} W:{new Vector2(coordsOfInitialSea.x, coordsOfInitialSea.y + 1)}");
            }
            */

            return terrainTypes;
        }
        
        // It will assume that all land has been labeled as "land" in the given "TerrainTypes"
        /*private TerrainType[,] CheckSetAndPropagateSea(TerrainType[,] terrainTypes, Vector2Int arrayCoords, List<Vector2Int> pendingLocations, out bool success)
        {
            success = false;
            
            if (arrayCoords.x < 0 || arrayCoords.y < 0 || arrayCoords.x >= terrainTypes.GetLength(0) || arrayCoords.y >= terrainTypes.GetLength(1))
                return terrainTypes;
            
            TerrainType originalType = terrainTypes[arrayCoords.x, arrayCoords.y];

            if (originalType != TerrainType.none)
                return terrainTypes;

            Vector2 coords = new Vector2(arrayCoords.x - mapGenerator.mapConfiguration.mapRadius, arrayCoords.y - mapGenerator.mapConfiguration.mapRadius);
            
            terrainTypes[arrayCoords.x, arrayCoords.y] = TerrainType.sea;
            //Debug.DrawRay(new Vector3(coordsOfInitialSea.x,GetHeightAt(new Vector2(coordsOfInitialSea.x,coordsOfInitialSea.y)),coordsOfInitialSea.y),Vector3.up, Color.magenta, 25f );
            Debug.DrawRay(new Vector3(coords.x,mapGenerator.mapConfiguration.seaHeightAbsolute,coords.y),Vector3.up, Color.magenta, 25f );
            success = true;
            return PropagateSea(terrainTypes, arrayCoords, pendingLocations);
        }*/

        public bool IsLocationUnderWater(Vector2 worldCoords)
        {
            return GetHeightAt(worldCoords) < mapGenerator.mapConfiguration.seaHeightAbsolute;
        } 

        public float GetHeightAt(Vector2 worldCoords)
        {
            TerrainChunk chunk = terrainChunks[GetChunksCoordsAt(worldCoords)];
            
            /*Vector2 absoluteChunkCoords = chunk.transform.position.ToVector2WithoutY();
            float chunkAbsoluteSize = mapGenerator.mapConfiguration.chunkWorldSize;
            Vector2 distanceFromChunkCenter = new Vector2(worldCoords.x - absoluteChunkCoords.x, worldCoords.y - absoluteChunkCoords.y);
            float chunkAbsoluteWidth = mapGenerator.mapConfiguration.chunkWorldSize;
            int heightMapWidth = chunk.heightMap.values.GetLength(0);
            int heightMapCoordsX = Mathf.RoundToInt(chunkAbsoluteWidth / heightMapWidth * distanceFromChunkCenter.x + heightMapWidth / 2f);//(int)(coords.x * chunkAbsoluteSize / 2f / absoluteChunkCoords.x);
            int heightMapCoordsY = Mathf.RoundToInt(heightMapWidth - (chunkAbsoluteWidth / heightMapWidth * distanceFromChunkCenter.y + heightMapWidth / 2f));//(int)(coords.y * chunkAbsoluteSize / 2f / absoluteChunkCoords.y);
            //Debug.Log($"CHECKING HEIGHT AT {heightMapCoordsX}, {heightMapCoordsY}. coords = {coords}, chunkAbsoluteSize = {chunkAbsoluteSize}, absoluteChunkCoords = {absoluteChunkCoords}");
            return chunk.heightMap.values[heightMapCoordsX,heightMapCoordsY];*/

            return TerrainMeshGenerator.GetHeight(chunk.heightMap.values, mapGenerator.mapConfiguration,  worldCoords - chunk.transform.position.ToVector2WithoutY());
        }

        public Vector2Int GetChunksCoordsAt(Vector2 coords)
        {
            int chunkCordX = Mathf.RoundToInt(coords.x / mapGenerator.mapConfiguration.chunkWorldSize);
            int chunkCordY = Mathf.RoundToInt(coords.y / mapGenerator.mapConfiguration.chunkWorldSize);
            return new Vector2Int(chunkCordX, chunkCordY);
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

            DestroyWaterSources();
        }
        
        private void DestroyWaterSources()
        {
            if (Application.isPlaying)
                waterSourceParent.transform.DestroyAllChildren(); 
            else
                waterSourceParent.transform.DestroyImmediateAllChildren();

            Debug.LogWarning("NotImplementedException();"); // And remove them from the MapElement's list in the mapManager
        }
        protected override void GenerateStep(bool clearPrevious, bool generateNextStepOnFinish)
        {        
            Debug.Log($"Generating in {this.name}.generateNextStepOnFinish = {generateNextStepOnFinish}", this);
            //base.GenerateStep(clearPrevious, generateNextStepOnFinish);
            UpdateChunks(clearPrevious);
        }

        /*private void OnDrawGizmosSelected()
        {
            float step = 4.5f;
            float seaHeight = mapGenerator.mapConfiguration.seaHeightAbsolute;
            
            for (float x = -mapGenerator.mapConfiguration.mapRadius; x <= mapGenerator.mapConfiguration.mapRadius; x+=step)
            {
                for (float y = -mapGenerator.mapConfiguration.mapRadius; y <= mapGenerator.mapConfiguration.mapRadius; y+=step)
                {
                    float height = GetHeightAt(new Vector2(x, y));
                    //if (height > seaHeight)
                    if (!IsLocationUnderWater(new Vector2(x,y)))
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawSphere(new Vector3(x, height, y), 0.25f);
                    }
                    else
                    {
                        Gizmos.color = Color.blue;
                        Gizmos.DrawSphere(new Vector3(x, height, y), 0.25f);
                    }
                }                
            }
        }*/
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