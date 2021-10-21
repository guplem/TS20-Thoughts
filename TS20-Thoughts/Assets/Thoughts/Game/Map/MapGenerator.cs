using System;
using Thoughts.Game.Map.Terrain;
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
                GenerateFullMap(true);
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
            //Delete terrain
            terrainGenerator.DeleteTerrain();
        
            //Todo: delete other elements of the map apart from the terrain
        }
        
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

        }
        
        /// <summary>
        /// Regenerates the things related to the given creation step 
        /// </summary>
        /// <param name="creationStep">The creation step that contains the things that are wanted to be regenerated</param>
        public void GenerateCreationStep(CreationStep creationStep)
        {
            switch (creationStep)
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
                    throw new ArgumentOutOfRangeException(nameof(creationStep), creationStep, $"Trying to generate creation step with no generation process: {Enum.GetName(typeof(CreationStep), creationStep)}");
            }
        }
        
        public void GenerateLight(bool clearPrevious)
        {
            // TODO
            Debug.LogWarning($"'{System.Reflection.MethodBase.GetCurrentMethod().Name}' Not implemented");
        }
        
        public void GenerateTerrain(bool clearPrevious)
        {
            terrainGenerator.UpdateChunks(clearPrevious);
        }
        
        public void GenerateVegetation(bool clearPrevious)
        {
            // TODO
            Debug.LogWarning($"'{System.Reflection.MethodBase.GetCurrentMethod().Name}' Not implemented");
        }
        
        public void GenerateNight(bool clearPrevious)
        {
            // TODO
            Debug.LogWarning($"'{System.Reflection.MethodBase.GetCurrentMethod().Name}' Not implemented");
        }
        
        public void GenerateFishAndBirds(bool clearPrevious)
        {
            // TODO
            Debug.LogWarning($"'{System.Reflection.MethodBase.GetCurrentMethod().Name}' Not implemented");
        }
        
        public void GenerateLandAnimals(bool clearPrevious)
        {
            // TODO
            Debug.LogWarning($"'{System.Reflection.MethodBase.GetCurrentMethod().Name}' Not implemented");
        }
        
        public void GenerateHumanoids(bool clearPrevious)
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
