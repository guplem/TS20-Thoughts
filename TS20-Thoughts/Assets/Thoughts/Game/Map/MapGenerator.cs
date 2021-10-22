using System;
using System.Collections.Generic;
using Thoughts.Game.Map.MapElements;
using Thoughts.Game.Map.Terrain;
using Thoughts.Utils.Maths;
using Thoughts.Utils.ThreadsManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

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
    
        /// <summary>
        /// If the app is not in Play Mode, the previously created map is destroyed and a new map is generated.
        /// </summary>
        private void RegenerateMapNotPlaying()
        {
            if (!Application.isPlaying)
            {
                //GenerateFullMap(true);
                Debug.LogWarning("WARNING: Fully regenerating a full map is no longer supported!");
            }
            else
            {
                Debug.LogWarning("Trying to regenerate the map as if the app were not in playing mode but it is.");
            }
        }

        //TODO: Improve the auto update system (time intervals, wait for the previous preview to fully load, ...)
        public void OnValidate()
        {
            
            if (mapConfiguration == null)
                return;
            mapConfiguration.OnValuesUpdated -= RegenerateMapNotPlaying; // So the subscription count stays at 1
            mapConfiguration.OnValuesUpdated += RegenerateMapNotPlaying;

            if (mapConfiguration.heightMapSettings == null)
                return;
            mapConfiguration.heightMapSettings.OnValuesUpdated -= RegenerateMapNotPlaying; // So the subscription count stays at 1
            mapConfiguration.heightMapSettings.OnValuesUpdated += RegenerateMapNotPlaying;
        
            if (mapConfiguration.textureSettings == null)
                return;
            mapConfiguration.textureSettings.OnValuesUpdated -= OnTextureValuesUpdated; // So the subscription count stays at 1
            mapConfiguration.textureSettings.OnValuesUpdated += OnTextureValuesUpdated;
            
            
        }

        /// <summary>
        /// Manages the update of the TextureSettings by applying them to the map's Material
        /// </summary>
        void OnTextureValuesUpdated()
        {
            mapConfiguration.textureSettings.ApplyToMaterial(mapConfiguration.heightMapSettings.minHeight, mapConfiguration.heightMapSettings.maxHeight);
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

            /*foreach (NavMeshSurface generatedNavMeshSurface in generatedNavMeshSurfaces)
            {
                NavMesh.RemoveNavMeshData(generatedNavMeshSurface.GetInstanceID());
            }*/
            NavMesh.RemoveAllNavMeshData();
            mapManager.generatedNavMeshSurfaces.Clear();
        }
        
        /*
        /// <summary>
        /// Updates or generates a full map.
        /// </summary>
        /// <param name="clearPreviousMap">If existent, should the previously created map be deleted?</param>
        public void GenerateFullMap(bool clearPreviousMap)
        {
            //1. Light
            GenerateLight(clearPreviousMap);
                
            //2. Terrain
            GenerateTerrain(clearPreviousMap);

            //3. Vegetation
            GenerateVegetation(clearPreviousMap);

            //4. Night
            GenerateNight(clearPreviousMap);

            //5. Fish and Birds
            GenerateFishAndBirds(clearPreviousMap);

            //6. Land Animals
            GenerateLandAnimals(clearPreviousMap);

            //6. Humanoids
            GenerateHumanoids(clearPreviousMap);

        }*/
        
        /// <summary>
        /// Regenerates the things related to the given creation step 
        /// </summary>
        /// <param name="step">The creation step that contains the things that are wanted to be regenerated</param>
        public void Regenerate(CreationStep step)
        {
            switch (step)
            {
                case CreationStep.Light:
                    GenerateLight(true);
                    break;
                case CreationStep.Terrain:
                    terrainGenerator.UpdateChunks(true);
                    break;
                case CreationStep.Vegetation:
                    vegetationGenerator.GenerateVegetation(true);
                    break;
                case CreationStep.Night:
                    GenerateNight(true);
                    break;
                case CreationStep.FishAndBirds:
                    GenerateFishAndBirds(true);
                    break;
                case CreationStep.LandAnimals:
                    GenerateLandAnimals(true);
                    break;
                case CreationStep.Humanoids:
                    humanoidsGenerator.GenerateHumanoids(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(step), step, $"Trying to generate creation step with no generation process: {Enum.GetName(typeof(CreationStep), step)}");
            }
        }
        
        private void GenerateLight(bool clearPrevious)
        {
            // TODO: remove this method, follow standards (like vegetationGenerator)
            Debug.LogWarning($"'{System.Reflection.MethodBase.GetCurrentMethod().Name}' Not implemented");
        }
        private void GenerateNight(bool clearPrevious)
        {
            // TODO: remove this method, follow standards (like vegetationGenerator)
            Debug.LogWarning($"'{System.Reflection.MethodBase.GetCurrentMethod().Name}' Not implemented");
        }
        private void GenerateFishAndBirds(bool clearPrevious)
        {
            // TODO: remove this method, follow standards (like vegetationGenerator)
            Debug.LogWarning($"'{System.Reflection.MethodBase.GetCurrentMethod().Name}' Not implemented");
        }
        private void GenerateLandAnimals(bool clearPrevious)
        {
            // TODO: remove this method, follow standards (like vegetationGenerator)
            Debug.LogWarning($"'{System.Reflection.MethodBase.GetCurrentMethod().Name}' Not implemented");
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
        public void SpawnMapElementsWithPerlinNoiseDistribution(GameObject objectToSpawn, int seed, float minHeight, float probability, Transform parent, NoiseMapSettings noiseMapSettings)
        {
            float[,] noise = Noise.GenerateNoiseMap((int)mapConfiguration.mapRadius*2, (int)mapConfiguration.mapRadius*2, noiseMapSettings, Vector2.zero, seed);
            float rayOriginHeight = mapConfiguration.heightMapSettings.heightMultiplier * 2f;
            float rayDistance = rayOriginHeight * minHeight; //[0,1], 1 being that the vegetation can get on the sea
            for (int x = 0; x < noise.GetLength(0); x++)
            {
                for (int y = 0; y < noise.GetLength(1); y++)
                {
                    if (noise[x, y] > 1-probability)
                    {
                        Vector2 positionCheck = new Vector2(x - mapConfiguration.mapRadius, y - mapConfiguration.mapRadius);
                        RaycastHit hit;
                        // Does the ray intersect any objects excluding the player layer
                        if (Physics.Raycast(positionCheck.ToVector3NewY(rayOriginHeight), Vector3.down, out hit, rayDistance*minHeight))
                        {
                            //Todo: be able to get more than just the first mapElement in the collection. Maybe even each one of the elements in the collection could have its own noise settings, prefab reference and threshold
                            SpawnMapElement(objectToSpawn, hit.point, Quaternion.identity, parent);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Sets up the NavMesh the given NavMeshAgent
        /// </summary>
        /// <returns>True if the nav mesh has been created, false if it has not been possible (maybe a mesh for the given agent already exists).</returns>
        public bool SetupNewNavMeshFor(NavMeshAgent navMeshAgent, bool skipIfRepeated = true)
        {
            if (navMeshAgent == null)
            {
                Debug.LogWarning("Tying to create the NavMesh surface for a null navMeshAgent", this);
                return false;
            }
                
            bool repeatedAgent = false;
            foreach (NavMeshSurface generatedNavMeshSurface in mapManager.generatedNavMeshSurfaces)
                if (generatedNavMeshSurface.agentTypeID == navMeshAgent.agentTypeID)
                    repeatedAgent = true;

            if (repeatedAgent && skipIfRepeated)
            {
                Debug.LogWarning($"Skipping the creation of the NavMeshSurface for the agent {navMeshAgent.ToString()} because it is duplicated.");
                return false;
            }
            if (repeatedAgent && !skipIfRepeated)
                Debug.LogWarning($"Recreating a NavMeshSurface for a duplicated agent {navMeshAgent.ToString()}.");

            NavMeshSurface navMeshSurface = this.gameObject.AddComponent<NavMeshSurface>();
            navMeshSurface.agentTypeID = navMeshAgent.agentTypeID;
            navMeshSurface.BuildNavMesh();
            //navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData); //To update the whole NavMesh at runtime
            mapManager.generatedNavMeshSurfaces.Add(navMeshSurface);
            return true;
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
