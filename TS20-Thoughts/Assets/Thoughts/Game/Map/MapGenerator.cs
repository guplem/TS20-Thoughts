using System;
using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.Map.MapElements;
using Thoughts.Game.Map.Terrain;
using Thoughts.Utils.Maths;
using Thoughts.Utils.ThreadsManagement;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Thoughts.Game.Map
{
    /// <summary>
    /// Component in charge of generating a Map
    /// </summary>
    public class MapGenerator : MonoBehaviour
    {
        
        private MapManager mapManager { get {
                if (_mapManager == null) _mapManager = this.GetComponentRequired<MapManager>();
                return _mapManager;
        } }
        private MapManager _mapManager;
        
        /// <summary>
        /// Reference to the MapConfiguration with he settings to generate a map
        /// </summary>
        [Tooltip("Reference to the MapConfiguration with he settings to generate a map")]
        [SerializeField] public MapConfiguration mapConfiguration;

        /// <summary>
        /// Reference to the ThreadedDataRequester component in charge doing threaded requests of data
        /// </summary>
        [Tooltip("Reference to the ThreadedDataRequester component in charge doing threaded requests of data")]
        [SerializeField] public ThreadedDataRequester threadedDataRequester;
        
        /// <summary>
        /// Reference to the TerrainGenerator component in charge of generating the Terrain
        /// </summary>
        [Tooltip("Reference to the TerrainGenerator component in charge of generating the Terrain")]
        [SerializeField] public TerrainGenerator terrainGenerator;
        
        /// <summary>
        /// Reference to the VegetationGenerator component in charge of generating the Vegetation
        /// </summary>
        [Tooltip("Reference to the VegetationGenerator component in charge of generating the Vegetation")]
        [SerializeField] private VegetationGenerator vegetationGenerator;
        
        /// <summary>
        /// Reference to the HumanoidsGenerator component in charge of generating the Humanoids
        /// </summary>
        [Tooltip("Reference to the HumanoidsGenerator component in charge of generating the Humanoids")]
        [SerializeField] private HumanoidsGenerator humanoidsGenerator;
        

    #if UNITY_EDITOR
        void OnDrawGizmos()
        {
            // Ensure continuous Update calls. Needed to generate the map in the editor (issues with threads)
            if (!Application.isPlaying)
            {
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
                UnityEditor.SceneView.RepaintAll();
            }
        }
    #endif
        

        //TODO: Improve the auto update system (time intervals, wait for the previous preview to fully load, ...)
        public void OnValidate()
        {
            //GENERAL
            if (mapConfiguration != null)
            {
                mapConfiguration.ClearOnValuesUpdated(); // So the subscription count stays at 1
                mapConfiguration.OnValuesUpdated += RegenerateFull;
            }
            else
            {
                Debug.LogWarning($"MapConfiguration in MapGenerator in {gameObject.name} is null.");
                return;
            }



            //Light
            if (mapConfiguration.lightSettings != null)
            {
                mapConfiguration.lightSettings.ClearOnValuesUpdated(); // So the subscription count stays at 1
                mapConfiguration.lightSettings.OnValuesUpdated += RegenerateLight;
            }
            
            //Terrain
            if (mapConfiguration.terrainHeightSettings != null)
            {
                mapConfiguration.terrainHeightSettings.ClearOnValuesUpdated(); // So the subscription count stays at 1
                mapConfiguration.terrainHeightSettings.OnValuesUpdated += RegenerateFull;
            }

            if (mapConfiguration.terrainTextureSettings != null)
            {
                mapConfiguration.terrainTextureSettings.ClearOnValuesUpdated(); // So the subscription count stays at 1
                mapConfiguration.terrainTextureSettings.OnValuesUpdated += OnTerrainTextureValuesUpdated;
            }

            //Vegetation
            if (mapConfiguration.vegetationSettings != null)
            {
                mapConfiguration.vegetationSettings.ClearOnValuesUpdated(); // So the subscription count stays at 1
                mapConfiguration.vegetationSettings.OnValuesUpdated += RegenerateVegetation;
            }
            
            //Night
            if (mapConfiguration.nightSettings != null)
            {
                mapConfiguration.nightSettings.ClearOnValuesUpdated(); // So the subscription count stays at 1
                mapConfiguration.nightSettings.OnValuesUpdated += RegenerateNight;
            }
            
            //FishAndBirds
            if (mapConfiguration.fishAndBirdsSettings != null)
            {
                mapConfiguration.fishAndBirdsSettings.ClearOnValuesUpdated(); // So the subscription count stays at 1
                mapConfiguration.fishAndBirdsSettings.OnValuesUpdated += RegenerateFishAndBirds;
            }
            
            //LandAnimals
            if (mapConfiguration.landAnimalsSettings != null)
            {
                mapConfiguration.landAnimalsSettings.ClearOnValuesUpdated(); // So the subscription count stays at 1
                mapConfiguration.landAnimalsSettings.OnValuesUpdated += RegenerateLandAnimals;
            }
            
            //Humanoids
            if (mapConfiguration.humanoidsSettings != null)
            {
                mapConfiguration.humanoidsSettings.ClearOnValuesUpdated(); // So the subscription count stays at 1
                mapConfiguration.humanoidsSettings.OnValuesUpdated += RegenerateHumanoids;
            }

        }

        /// <summary>
        /// Manages the update of the TextureSettings by applying them to the map's Material
        /// </summary>
        void OnTerrainTextureValuesUpdated()
        {
            mapConfiguration.terrainTextureSettings.ApplyToMaterial(mapConfiguration.terrainHeightSettings.minHeight, mapConfiguration.terrainHeightSettings.maxHeight);
        }


        /// <summary>
        /// Deletes the currently (generated) existent map
        /// </summary>
        public void DeleteCurrentMap()
        {
            //Todo: delete other elements of the map apart from the terrain
            terrainGenerator.DeleteTerrain();
            vegetationGenerator.DeleteVegetation();
            humanoidsGenerator.DeleteHumanoids();

            
            
            // Delete all nav mesh data and components
            NavMesh.RemoveAllNavMeshData();
            
            foreach (NavMeshSurface navMeshSurface in mapManager.generatedNavMeshSurfaces)
                if (Application.isPlaying)
                    Destroy(navMeshSurface);
                else
                    DestroyImmediate(navMeshSurface);
            mapManager.generatedNavMeshSurfaces.Clear();
            
            if (Application.isPlaying)
                StartCoroutine(nameof(DeleteCurrentMapCheckCoroutine));
        }
        
        /// <summary>
        /// Coroutine that checks that the full deletion of the map has been successful
        /// </summary>
        private IEnumerator  DeleteCurrentMapCheckCoroutine()
        {
            if (Application.isPlaying) // Important(?). Coroutines only work in Play mode
                yield return new WaitForSecondsRealtime(3f); // To give a chance to the "Destroy" method. It is not immediate.
            
            NavMeshSurface[] remainingSurfaces = GetComponents<NavMeshSurface>();
            if (remainingSurfaces.Length > 0)
                Debug.LogWarning($"Not all NavMeshSurfaces from {gameObject} have been deleted. {remainingSurfaces.Length} still exist.", remainingSurfaces[0]);
        }
        
        private void RegenerateLight(){ Regenerate(CreationStep.Light);}
        //public void RegenerateTerrain(){ RegenerateFullMap(); }
        private void RegenerateVegetation(){ Regenerate(CreationStep.Vegetation);}
        private void RegenerateNight(){ Regenerate(CreationStep.Night);}
        private void RegenerateFishAndBirds(){ Regenerate(CreationStep.FishAndBirds);}
        private void RegenerateLandAnimals(){ Regenerate(CreationStep.LandAnimals);}
        private void RegenerateHumanoids(){ Regenerate(CreationStep.Humanoids);}

        /// <summary>
        /// The previously created map is destroyed and a new FULL map (with all the creation steps) is generated.
        /// </summary>
        public void RegenerateFull()
        {
            throw new NotImplementedException(); //Todo: code it. Use Regenerate(step) method
        } 
        
        /// <summary>
        /// Regenerates the things related to the given creation step 
        /// </summary>
        /// <param name="step">The creation step that contains the things that are wanted to be regenerated</param>
        public void Regenerate(CreationStep step)
        {
            // Debug.Log($"Regenerating '{step.ToString()}'");
            
            if (!Application.isPlaying)
                EditorUtility.SetDirty(mapManager.gameObject);
                
            switch (step)
            {
                case CreationStep.Light:
                    throw new NotImplementedException();
                    //break;
                case CreationStep.Terrain:
                    terrainGenerator.UpdateChunks(true);
                    //ToDo: Update material
                    break;
                case CreationStep.Vegetation:
                    vegetationGenerator.GenerateVegetation(true);
                    break;
                case CreationStep.Night:
                    throw new NotImplementedException();
                    //break;
                case CreationStep.FishAndBirds:
                    throw new NotImplementedException();
                    //break;
                case CreationStep.LandAnimals:
                    throw new NotImplementedException();
                    //break;
                case CreationStep.Humanoids:
                    humanoidsGenerator.GenerateHumanoids(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(step), step, $"Trying to generate creation step with no generation process: {Enum.GetName(typeof(CreationStep), step)}");
            }
        }
        

        /// <summary>
        /// Makes a map element spawn.
        /// </summary>
        /// <param name="objectToSpawn">The map element's prefab to spawn</param>
        /// <param name="position">Position for the new object.</param>
        /// <param name="rotation">Orientation of the new object.</param>
        /// <param name="parent">The transform that must be the parent of the spawned MapElement</param>
        /// <returns></returns>
        private MapElement SpawnMapElement(GameObject objectToSpawn, Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject spawnedMapElement = Instantiate(objectToSpawn, position, rotation, parent);
            MapElement spawnedElement =  spawnedMapElement.GetComponentRequired<MapElement>();
            mapManager.existentMapElements.Add(spawnedElement);
            return spawnedElement;
        }
        
        /// <summary>
        /// Spawns a group of MapElements in the map using a pseudo random perlin noise distribution.
        /// </summary>
        /// <param name="objectToSpawn">The map element's prefab to spawn</param>
        /// <param name="seed">The seed to use to generate the perlin noise</param>
        /// <param name="minHeight">The minimum height at which the object can spawn (0 means that can spawn on the sea)</param>
        /// <param name="probability">The probability of the object being spawned at any given spot (following the perlin noise distribution)</param>
        /// <param name="parent">The transform that must be the parent of the spawned MapElement</param>
        /// <param name="noiseMapSettings">The settings to be used for the perlin noise map</param>
        /// <param name="requireNavMesh">Must the locations where the MapElements will spawn require a valid NavMeshSurface?</param>
        public void SpawnMapElementsWithPerlinNoiseDistribution(GameObject objectToSpawn, int seed, float minHeight, float probability, Transform parent, NoiseMapSettings noiseMapSettings, bool requireNavMesh)
        {
            float[,] noise = Noise.GenerateNoiseMap((int)mapConfiguration.mapRadius*2, (int)mapConfiguration.mapRadius*2, noiseMapSettings, Vector2.zero, seed);
            for (int x = 0; x < noise.GetLength(0); x++)
            {
                for (int y = 0; y < noise.GetLength(1); y++)
                {
                    if (!(noise[x, y] > 1 - probability))
                        continue;
                    
                    if (IsSpawnablePosition( new Vector2(x - mapConfiguration.mapRadius, y - mapConfiguration.mapRadius), minHeight, requireNavMesh, out Vector3 spawnablePosition))
                        SpawnMapElement(objectToSpawn, spawnablePosition, Quaternion.identity, parent);
                }
            }
        }

        /// <summary>
        /// Checks if a MapElement can be spawned or not in a given position.
        /// </summary>
        /// <param name="positionCheck">The 2D position to check if a Map</param>
        /// <param name="minHeight">The minimum height at which the object can be spawned (0 means that can spawn on the sea)</param>
        /// <param name="requireNavMesh">Must the location require a valid NavMeshSurface?</param>
        /// <param name="spawnablePosition">If the given position allows an spawn, this is the 3D position (including the height at which it can be spawned) so there is no need to recalculate it again.</param>
        /// <returns>True if the location allows the spawn of the MapElement, false otherwise.</returns>
        private bool IsSpawnablePosition(Vector2 positionCheck, float minHeight, bool requireNavMesh, out Vector3 spawnablePosition)
        {


            float rayOriginHeight = mapConfiguration.terrainHeightSettings.heightMultiplier * 2f;
            float rayDistance = rayOriginHeight * minHeight; //[0,1], 1 being that the vegetation can get on the sea
            
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(positionCheck.ToVector3NewY(rayOriginHeight), Vector3.down, out hit, rayDistance * minHeight))
            {
                spawnablePosition = hit.point;
                if (requireNavMesh)
                {
                    NavMeshHit navMeshHit;
                    if (NavMesh.SamplePosition(spawnablePosition, out navMeshHit, 1.0f, NavMesh.AllAreas))
                    {
                        spawnablePosition = navMeshHit.position;
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
            spawnablePosition = Vector3.zero;
            return false;
        }

        /// <summary>
        /// Randomly spawns in the map a given number of MapElements.
        /// </summary>
        /// <param name="objectToSpawn">The map element's prefab to spawn</param>
        /// <param name="seed">The seed to use to generate the perlin noise</param>
        /// <param name="minHeight">The minimum height at which the object can spawn (0 means that can spawn on the sea)</param>
        /// <param name="quantity">The amount of MapElements to spawn</param>
        /// <param name="parent">The transform that must be the parent of the spawned MapElement</param>
        /// <param name="requireNavMesh">Must the locations where the MapElements will spawn require a valid NavMeshSurface?</param>
        public void SpawnMapElementsRandomly(GameObject objectToSpawn, int seed, float minHeight, int quantity, Transform parent, bool requireNavMesh)
        {
            int totalCountToAvoidInfiniteLoop = 5000*quantity;
            int spawnedCount = 0;
            
            RandomEssentials randomEssentials = new RandomEssentials(seed);
            
            while (spawnedCount < quantity)
            {
                totalCountToAvoidInfiniteLoop--;
                if (totalCountToAvoidInfiniteLoop < 0)
                {
                    Debug.LogWarning($"Skipped the spawning of x{quantity} {objectToSpawn}. No spawnable positions were found.");
                    break;
                }
                
                Vector2 checkPosition = randomEssentials.GetRandomVector2(-mapConfiguration.mapRadius, mapConfiguration.mapRadius);

                if (IsSpawnablePosition(checkPosition, minHeight, requireNavMesh, out Vector3 spawnablePosition))
                {
                    SpawnMapElement(objectToSpawn, spawnablePosition, Quaternion.identity, parent);
                    spawnedCount++;
                }
            }
        }
        
        /// <summary>
        /// Sets up the NavMesh the given NavMeshAgent
        /// </summary>
        /// <returns>The generated NavMeshSurface ig it has been created. Null if it has not been possible (maybe a mesh for the given agent already exists).</returns>
        public NavMeshSurface SetupNewNavMeshFor(NavMeshAgent navMeshAgent, bool skipIfRepeated = true)
        {
            if (navMeshAgent == null)
            {
                Debug.LogWarning("Tying to create the NavMesh surface for a null navMeshAgent", this);
                return null;
            }
                
            bool repeatedAgent = false;
            foreach (NavMeshSurface generatedNavMeshSurface in mapManager.generatedNavMeshSurfaces)
                if (generatedNavMeshSurface.agentTypeID == navMeshAgent.agentTypeID)
                    repeatedAgent = true;

            if (repeatedAgent && skipIfRepeated)
            {
                Debug.LogWarning($"Skipping the creation of the NavMeshSurface for the agent {navMeshAgent.ToString()} because it is duplicated.");
                return null;
            }
            if (repeatedAgent && !skipIfRepeated)
                Debug.LogWarning($"Recreating a NavMeshSurface for a duplicated agent {navMeshAgent.ToString()}.");

            NavMeshSurface navMeshSurface = this.gameObject.AddComponent<NavMeshSurface>();
            navMeshSurface.agentTypeID = navMeshAgent.agentTypeID;
            navMeshSurface.BuildNavMesh();
            //navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData); //To update the whole NavMesh at runtime
            mapManager.generatedNavMeshSurfaces.Add(navMeshSurface);
            return navMeshSurface;
        }
    }
    
    /// <summary>
    /// Listing of all the possible creation steps that will appear at the beginning of each game
    /// </summary>
    public enum CreationStep
    {
        UserName,
        Light,
        Terrain, 
        Vegetation,
        Night,
        FishAndBirds,
        LandAnimals,
        Humanoids
    }
}
