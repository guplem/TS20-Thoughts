using System;
using Thoughts.Game.Map.Terrain;
using Thoughts.Utils.Maths;
using Thoughts.Utils.ThreadsManagement;
using UnityEngine;
using UnityEngine.Serialization;

namespace Thoughts.Game.Map
{
    /// <summary>
    /// Component in charge of generating a Map
    /// </summary>
    public class MapGenerator : MonoBehaviour
    {

        [FormerlySerializedAs("autoRegenerate")]
        [Space]
        public bool autoRegenerateInEditor = false;

        /// <summary>
        /// Reference to the TerrainGenerator component in charge of generating the Terrain
        /// </summary>
        [Tooltip("Reference to the TerrainGenerator component in charge of generating the Terrain")]
        [SerializeField] public TerrainGenerator terrainGenerator;

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
        /// Reference to the transform that is going to be parent of all generated vegetation 
        /// </summary>
        [Tooltip("Reference to the transform that is going to be parent of all generated vegetation ")]
        [SerializeField] public Transform vegetationHolder;

        
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
            terrainGenerator.DeleteTerrain();
            DeleteVegetation();

            //Todo: delete other elements of the map apart from the terrain
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
                    GenerateTerrain(true);
                    break;
                case CreationStep.Vegetation:
                    GenerateVegetation(true);
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
                    GenerateHumanoids(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(step), step, $"Trying to generate creation step with no generation process: {Enum.GetName(typeof(CreationStep), step)}");
            }
        }
        
        private void GenerateLight(bool clearPrevious)
        {
            // TODO
            Debug.LogWarning($"'{System.Reflection.MethodBase.GetCurrentMethod().Name}' Not implemented");
        }
        
        private void GenerateTerrain(bool clearPrevious)
        {
            terrainGenerator.UpdateChunks(clearPrevious);
        }
        
        private void GenerateVegetation(bool clearPrevious)
        {
            if (clearPrevious)
                DeleteVegetation();
            
            float[,] noise = Noise.GenerateNoiseMap((int)mapConfiguration.mapRadius*2, (int)mapConfiguration.mapRadius*2, mapConfiguration.vegetationNoiseSettings, Vector2.zero, mapConfiguration.seed);
            float rayOriginHeight = mapConfiguration.heightMapSettings.heightMultiplier * 2f;
            float closinessToShore = 0.993f; //[0,1], 1 being that the vegetation can get on the sea
            float rayDistance = rayOriginHeight * closinessToShore; //[0,1], 1 being that the vegetation can get on the sea
            for (int x = 0; x < noise.GetLength(0); x++)
            {
                for (int y = 0; y < noise.GetLength(1); y++)
                {
                    if (noise[x, y] > 0.5f)
                    {
                        Vector2 positionCheck = new Vector2(x - mapConfiguration.mapRadius, y - mapConfiguration.mapRadius);
                        RaycastHit hit;
                        // Does the ray intersect any objects excluding the player layer
                        if (Physics.Raycast(positionCheck.ToVector3NewY(rayOriginHeight), transform.TransformDirection(Vector3.down), out hit, rayDistance*closinessToShore))
                        {
                            //Todo: be able to get more than just the first mapElement in the collection. Maybe even each one of the elements in the collection could have its own noise settings, prefab reference and treshold
                            Instantiate(mapConfiguration.vegetationCollection.mapElements[0], hit.point, Quaternion.identity, vegetationHolder);

                        }
                    }
                }
            }
        }
        
        private void DeleteVegetation()
        {
            if (Application.isPlaying)
                vegetationHolder.DestroyAllChildren(); 
            else
                vegetationHolder.DestroyImmediateAllChildren();
        }

        private void GenerateNight(bool clearPrevious)
        {
            // TODO
            Debug.LogWarning($"'{System.Reflection.MethodBase.GetCurrentMethod().Name}' Not implemented");
        }
        
        private void GenerateFishAndBirds(bool clearPrevious)
        {
            // TODO
            Debug.LogWarning($"'{System.Reflection.MethodBase.GetCurrentMethod().Name}' Not implemented");
        }
        
        private void GenerateLandAnimals(bool clearPrevious)
        {
            // TODO
            Debug.LogWarning($"'{System.Reflection.MethodBase.GetCurrentMethod().Name}' Not implemented");
        }
        
        private void GenerateHumanoids(bool clearPrevious)
        {
            // TODO
            Debug.LogWarning($"'{System.Reflection.MethodBase.GetCurrentMethod().Name}' Not implemented");
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
