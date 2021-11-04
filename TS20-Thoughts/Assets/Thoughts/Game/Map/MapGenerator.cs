using System;
using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.Map.MapElements;
using Thoughts.Game.Map.Terrain;
using Thoughts.Utils.Maths;
using Thoughts.Utils.ThreadsManagement;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.AI;

namespace Thoughts.Game.Map
{
    /// <summary>
    /// Component in charge of generating a Map
    /// </summary>
    [RequireComponent(typeof(MapManager))]
    [RequireComponent(typeof(ThreadedDataRequester))]
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
        public ThreadedDataRequester threadedDataRequester { get {
            if (_threadedDataRequester == null) _threadedDataRequester = this.GetComponentRequired<ThreadedDataRequester>();
            return _threadedDataRequester;
        } }
        private ThreadedDataRequester _threadedDataRequester;
        
        [SerializeField] private Transform sea;

        /// <summary>
        /// Reference to the TerrainGenerator component in charge of generating the Terrain
        /// </summary>
        [Header("Steps Generators")]
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
        /*
        [Header("Behaviour")]
        [SerializeField] private bool regenerateFullOnRecompilation = false;
         
        void OnEnable()
        {
            AssemblyReloadEvents.afterAssemblyReload += OnAfterAssemblyReload;
            CompilationPipeline.compilationFinished         += OnAfterAssemblyReload;
            CompilationPipeline.assemblyCompilationFinished += OnAfterAssemblyReload;
        }

        void OnDisable()
        {
            AssemblyReloadEvents.afterAssemblyReload -= OnAfterAssemblyReload;
            CompilationPipeline.compilationFinished         -= OnAfterAssemblyReload;
            CompilationPipeline.assemblyCompilationFinished -= OnAfterAssemblyReload;
        }

        public void OnAfterAssemblyReload() // Called after recompilation / assembly reload
        {
            Debug.Log("After Assembly Reload Map Generator");

            //Regenerate map after compilation
            if (regenerateFullOnRecompilation)
                RegenerateFull();
        }
        
        public static void TT2() // Called after recompilation / assembly reload
        {
            Debug.Log("TT2");
            Debug.Log("asdsad");

            //Regenerate map after compilation
            //if (regenerateFullOnRecompilation)
                GameManager.instance.mapManager.mapGenerator.RegenerateFull();
        }
        
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void CreateAssetWhenReady()
        {
            if(EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += CreateAssetWhenReady;
                return;
            }
 
            EditorApplication.delayCall += TT2;
        }
        */
        void OnDrawGizmos()
        {
            // Ensure continuous Update calls. Needed to generate the map in the editor (issues with threads)
            if (!Application.isPlaying)
            {
                UnityEditor.SceneView.RepaintAll();
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            }
            
            //Draws the lines to show where the base, sea and max height are
            float linesHalfSize = 1000;
            Gizmos.color = Color.black;
            Gizmos.DrawLine(new Vector3(-linesHalfSize, 0, 0), new Vector3(linesHalfSize, 0, 0));
            Gizmos.color = Color.blue;
            float seaHeight = mapConfiguration.seaHeightAbsolute;
            Gizmos.DrawLine(new Vector3(-linesHalfSize, seaHeight, 0), new Vector3(linesHalfSize, seaHeight, 0));
            Gizmos.color = Color.white;
            Gizmos.DrawLine(new Vector3(-linesHalfSize, mapConfiguration.terrainHeightSettings.maxHeight, 0), new Vector3(linesHalfSize, mapConfiguration.terrainHeightSettings.maxHeight, 0));

        }
    #endif
        
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
                mapConfiguration.terrainHeightSettings.OnValuesUpdated += RegenerateTerrain; //RegenerateFull;
            }

            if (mapConfiguration.terrainTextureSettings != null)
            {
                mapConfiguration.terrainTextureSettings.ClearOnValuesUpdated(); // So the subscription count stays at 1
                mapConfiguration.terrainTextureSettings.OnValuesUpdated += RegenerateTerrainTextures;
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
        void RegenerateTerrainTextures()
        {
            mapConfiguration.terrainTextureSettings.ApplyToMaterial(mapConfiguration.terrainHeightSettings.minHeight, mapConfiguration.terrainHeightSettings.maxHeight);
        }


        /// <summary>
        /// Deletes the currently (generated) existent map
        /// </summary>
        public void DeleteCurrentMap()
        {
            if (!Application.isPlaying)
                EditorUtility.SetDirty(mapManager.gameObject);
            
            terrainGenerator.Delete();
            vegetationGenerator.Delete();
            humanoidsGenerator.Delete();
            
            mapManager.navigationManager.RemoveAllNavMesh();
        }



        public void RegenerateLight(){ Regenerate(CreationStep.Light);}
        public void RegenerateTerrain(){ Regenerate(CreationStep.Terrain); }
        public void RegenerateVegetation(){ Regenerate(CreationStep.Vegetation);}
        public void RegenerateNight(){ Regenerate(CreationStep.Night);}
        public void RegenerateFishAndBirds(){ Regenerate(CreationStep.FishAndBirds);}
        public void RegenerateLandAnimals(){ Regenerate(CreationStep.LandAnimals);}
        public void RegenerateHumanoids(){ Regenerate(CreationStep.Humanoids);}

        /// <summary>
        /// The previously created map is destroyed and a new FULL map (with all the creation steps) is generated.
        /// </summary>
        public void RegenerateFull()
        {
            Regenerate(CreationStep.Terrain, true);
        } 
        
        /// <summary>
        /// Regenerates the things related to the given creation step 
        /// </summary>
        /// <param name="step">The creation step that contains the things that are wanted to be regenerated</param>
        public void Regenerate(CreationStep step, bool generateNextStepOnFinish = false)
        {
            // Debug.Log($"Regenerating '{step.ToString()}'");
            
            if (!Application.isPlaying)
                EditorUtility.SetDirty(mapManager.gameObject);

            switch (step)
            {
                case CreationStep.Light:
                    Debug.LogWarning("Light Regeneration: NotImplementedException();");
                    break;
                case CreationStep.Terrain:
                    terrainGenerator.Generate(true, generateNextStepOnFinish);
                    RegenerateTerrainTextures();
                    ReconfigureSea();
                    break;
                case CreationStep.Vegetation:
                    vegetationGenerator.Generate(true, generateNextStepOnFinish);
                    break;
                case CreationStep.Night:
                    Debug.LogWarning("Night Regeneration: NotImplementedException();");
                    break;
                case CreationStep.FishAndBirds:
                    Debug.LogWarning("FishAndBirds Regeneration: NotImplementedException();");
                    break;
                case CreationStep.LandAnimals:
                    Debug.LogWarning("LandAnimals Regeneration: NotImplementedException();");
                    break;
                case CreationStep.Humanoids:
                    humanoidsGenerator.Generate(true, generateNextStepOnFinish);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(step), step, $"Trying to generate creation step with no generation process: {Enum.GetName(typeof(CreationStep), step)}");
            }
        }
        private void ReconfigureSea()
        {
            float seaHeight = mapConfiguration.seaHeightAbsolute;
            sea.transform.position = Vector3.zero.WithY(seaHeight);
            float seaSize = mapConfiguration.mapRadius * 2 * 2;
            sea.transform.localScale = new Vector3(seaSize, 1, seaSize);
        }


        /// <summary>
        /// Makes a map element spawn.
        /// </summary>
        /// <param name="objectToSpawn">The map element's prefab to spawn</param>
        /// <param name="position">Position for the new object.</param>
        /// <param name="rotation">Orientation of the new object.</param>
        /// <param name="parent">The transform that must be the parent of the spawned MapElement</param>
        /// <returns></returns>
        public MapElement SpawnMapElement(GameObject objectToSpawn, Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject spawnedMapElement = Instantiate(objectToSpawn, position, rotation, parent);
            MapElement spawnedElement =  spawnedMapElement.GetComponentRequired<MapElement>();
            mapManager.existentMapElements.Add(spawnedElement);
            return spawnedElement;
        }

        public void DestroyAllMapElementsChildOf(Transform parentOfMapElements)
        {
            //Debug.Log($"DESTROYING ALL FROM {parentOfMapElements.transform.name}");
            
            if (!Application.isPlaying)
            {
                parentOfMapElements.DestroyImmediateAllChildren();
                return;
            }
            
            foreach (Transform child in parentOfMapElements)
            {
                MapElement mapElement = child.GetComponent<MapElement>();
                if (mapElement != null)
                {
                    DestroyMapElement(mapElement);
                }
            }
        } 
        
        public void DestroyMapElement(MapElement mapElement)
        {
            if (!mapManager.existentMapElements.Remove(mapElement))
                Debug.Log($"The MapElement {mapElement.name} was not registered in the 'existentMapElements' but it was intended to destroy it.", mapElement);
            
            if (Application.isPlaying)
                Destroy(mapElement.gameObject);
            else
                DestroyImmediate(mapElement.gameObject);
        } 

        /// <summary>
        /// Spawns a group of MapElements in the map using a pseudo random perlin noise distribution.
        /// </summary>
        /// <param name="objectToSpawn">The map element's prefab to spawn</param>
        /// <param name="seed">The seed to use to generate the perlin noise</param>
        /// <param name="spawningHeightRange">In which area of the map this is wanted to spawn. -1 means the bottom of the sea. 1 means the highest points in the world. 0 is the shoreline.</param>
        /// <param name="probability">The probability of the object being spawned at any given spot (following the perlin noise distribution)</param>
        /// <param name="density">The density of the spawning. 1 meaning all the available spots where the probability says spawn should happen will be filled. 0 means none.</param>
        /// <param name="parent">The transform that must be the parent of the spawned MapElement</param>
        /// <param name="noiseMapSettings">The settings to be used for the perlin noise map</param>
        /// <param name="requireNavMesh">Must the locations where the MapElements will spawn require a valid NavMeshSurface?</param>
        public void SpawnMapElementsWithPerlinNoiseDistribution(GameObject objectToSpawn, int seed, Vector2 spawningHeightRange, float probability, float density, Transform parent, NoiseMapSettings noiseMapSettings, bool requireNavMesh)
        {
            RandomEssentials rng = new RandomEssentials(seed);
            
            float[,] noise = Noise.GenerateNoiseMap((int)mapConfiguration.mapRadius*2, (int)mapConfiguration.mapRadius*2, noiseMapSettings, Vector2.zero, seed);
            for (int x = 0; x < noise.GetLength(0); x++)
            {
                for (int y = 0; y < noise.GetLength(1); y++)
                {
                    if (!(noise[x, y] >= 1 - probability))
                        continue;
                    
                    if (rng.GetRandomBool(1-density))
                        continue;
                    
                    if (IsSpawnablePosition( new Vector2(x - mapConfiguration.mapRadius, y - mapConfiguration.mapRadius), spawningHeightRange, requireNavMesh, out Vector3 spawnablePosition))
                        SpawnMapElement(objectToSpawn, spawnablePosition, Quaternion.identity, parent);
                }
            }
        }

        /// <summary>
        /// Checks if a MapElement can be spawned or not in a given position.
        /// </summary>
        /// <param name="positionCheck">The 2D position to check if a Map</param>
        /// <param name="spawningHeightRange">The minimum height at which the object can be spawned (0 means that can spawn on the sea)</param>
        /// <param name="requireNavMesh">Must the location require a valid NavMeshSurface?</param>
        /// <param name="spawnablePosition">If the given position allows an spawn, this is the 3D position (including the height at which it can be spawned) so there is no need to recalculate it again.</param>
        /// <returns>True if the location allows the spawn of the MapElement, false otherwise.</returns>
        private bool IsSpawnablePosition(Vector2 positionCheck, Vector2 spawningHeightRange, bool requireNavMesh, out Vector3 spawnablePosition)
        {
            spawnablePosition = Vector3.zero;

            //float raySecureOffset = 0.5f;
            float rayOriginHeight = mapConfiguration.terrainHeightSettings.maxHeight;// + raySecureOffset;
            float rayDistance = rayOriginHeight;// + raySecureOffset;
            
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(positionCheck.ToVector3NewY(rayOriginHeight), Vector3.down, out hit, rayDistance))
            {
                float aboveSeaLevelHeight = mapConfiguration.terrainHeightSettings.maxHeight * (1 - mapConfiguration.seaHeight);
                float underSeaLevelHeight = mapConfiguration.terrainHeightSettings.maxHeight * mapConfiguration.seaHeight;
                float relativeHitHeight = Single.NegativeInfinity; // [-1,1] once calculated
                
                // The impact happened under the sea
                if (hit.distance > aboveSeaLevelHeight) // + raySecureOffset)
                    relativeHitHeight = -1 / underSeaLevelHeight * (hit.distance /*-raySecureOffset*/ - aboveSeaLevelHeight);

                // The impact happened above the sea
                else
                    relativeHitHeight = 1 - 1 / aboveSeaLevelHeight * hit.distance;//-raySecureOffset;
                
                if (relativeHitHeight > spawningHeightRange.y || relativeHitHeight < spawningHeightRange.x)
                    return false;
                
                spawnablePosition = hit.point;
                if (requireNavMesh)
                {
                    NavMeshHit navMeshHit;
                    if (NavMesh.SamplePosition(spawnablePosition, out navMeshHit, 1.0f, NavMesh.AllAreas))
                    {
                        spawnablePosition = navMeshHit.position;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Randomly spawns in the map a given number of MapElements.
        /// </summary>
        /// <param name="objectToSpawn">The map element's prefab to spawn</param>
        /// <param name="seed">The seed to use to generate the perlin noise</param>
        /// <param name="spawningHeightRange">In which area of the map this is wanted to spawn. -1 means the bottom of the sea. 1 means the highest points in the world. 0 is the shoreline.</param>
        /// <param name="quantity">The amount of MapElements to spawn</param>
        /// <param name="parent">The transform that must be the parent of the spawned MapElement</param>
        /// <param name="requireNavMesh">Must the locations where the MapElements will spawn require a valid NavMeshSurface?</param>
        public void SpawnMapElementsRandomly(GameObject objectToSpawn, int seed, Vector2 spawningHeightRange, int quantity, Transform parent, bool requireNavMesh)
        {
            int totalCountToAvoidInfiniteLoop = 5000*quantity;
            int spawnedCount = 0;
            
            RandomEssentials randomEssentials = new RandomEssentials(seed);
            
            while (spawnedCount < quantity)
            {
                totalCountToAvoidInfiniteLoop--;
                if (totalCountToAvoidInfiniteLoop < 0)
                {
                    Debug.LogWarning($"Skipped the spawning of x{quantity-spawnedCount}/{{quantity}} {objectToSpawn.name}. No spawnable positions were found.");
                    break;
                }
                
                Vector2 checkPosition = randomEssentials.GetRandomVector2(-mapConfiguration.mapRadius, mapConfiguration.mapRadius);

                if (IsSpawnablePosition(checkPosition, spawningHeightRange, requireNavMesh, out Vector3 spawnablePosition))
                {
                    SpawnMapElement(objectToSpawn, spawnablePosition, Quaternion.identity, parent);
                    spawnedCount++;
                }
            }
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
