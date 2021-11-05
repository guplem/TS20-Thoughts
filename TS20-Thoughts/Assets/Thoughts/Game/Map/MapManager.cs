using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Thoughts.Game.Map.MapElements;
using Thoughts.Game.Map.MapElements.Attributes;
using Thoughts.Game.Map.MapElements.Attributes.MapEvents;
using UnityEngine;
using UnityEngine.AI;

namespace Thoughts.Game.Map
{

    /// <summary>
    /// The map of the game.
    /// </summary>
    [RequireComponent(typeof(MapGenerator))]
    public class MapManager : MonoBehaviour
    {
        /// <summary>
        /// All the map elements present in the map.
        /// </summary>
        [NonSerialized] public List<MapElement> existentMapElements = new List<MapElement>();

        /// <summary>
        /// Reference to the MapGenerator component, the manager of the generation of the map
        /// </summary>
        public MapGenerator mapGenerator { get {
            if (_mapGenerator == null) _mapGenerator = this.GetComponentRequired<MapGenerator>();
            return _mapGenerator;
        } }
        private MapGenerator _mapGenerator;
        
        /// <summary>
        ///  Reference to the manager of the AI navigation
        /// </summary>
        [Tooltip("Reference to the manager of the AI navigation")]
        [SerializeField] public MapNavigationManager navigationManager;
        

        

    #region MapGeneration

        /// <summary>
        /// The previously created map is destroyed and a new FULL map (with all the creation steps) is generated.
        /// </summary>
        public void RegenerateFullMap()
        {
            mapGenerator.RegenerateFull();
            existentMapElements.Clear();
        }
        
        /// <summary>
        /// Generates the contents of a creation step
        /// </summary>
        public void RegenerateCreationStep(CreationStep creationStep)
        {
            mapGenerator.Regenerate(creationStep);
        }
        
        /// <summary>
        /// Deletes the currently (generated) existent map
        /// </summary>
        public void DeleteMap()
        {
            mapGenerator.DeleteCurrentMap();
            existentMapElements.Clear();
        }

        /*
        /// <summary>
        /// All the map elements' prefabs that can be spawned in the map.
        /// </summary>
        [SerializeField] private List<GameObject> spawnableMapElements;
*/
        /*
        /// <summary>
        /// Obtains the GameObject with the given name from the spawnableMapElement list.
        /// </summary>
        /// <param name="name">The name of the GameObject.</param>
        /// <returns>The GameObject with a matching name from inside the spawnableMapElements list. Null if no object is found with the given name in the list.</returns>
        private GameObject GetSpawnableGameObject(string name)
        {
            foreach (GameObject go in spawnableMapElements)
            {
                //Debug.Log($"Looking for '{name}'. Searching now object '{go.name}'. Result = {string.Compare(go.name, name, StringComparison.OrdinalIgnoreCase)}");
                if (string.Compare(go.name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return go;
            }
            Debug.LogError($"The GameObject with name '{name}' could not be found in the list of spawnableGameObjects");
            spawnableMapElements.DebugLog(", ","Spawnable Game Objects: ");
            return null;
        }*/
        /*
        /// <summary>
        /// Generates MapElements in the map (not mobs).
        /// </summary>
        /// <returns>A list of the generated map elements.</returns>
        private List<MapElement> GenerateMapElements()
        {
            List<MapElement> generatedMapObjects = new List<MapElement>();
            RandomEssentials random = new RandomEssentials();
            
            generatedMapObjects.AddRange(SpawnRandom("river", 3, random));
            generatedMapObjects.AddRange(SpawnRandom("rock", 10, random));
            generatedMapObjects.AddRange(SpawnRandom("tree", 1, random));
            generatedMapObjects.AddRange(SpawnRandom("bonfire", 1, random));
            generatedMapObjects.AddRange(SpawnRandom("Berry Plant", 3, random));

            return generatedMapObjects;
        }
        */
        /*
        /// <summary>
        /// Generates Mobs in the map.
        /// </summary>
        /// <returns>A list of the generated map elements (mobs).</returns>
        private List<MapElement> GenerateMobs()
        {
            List<MapElement> generatedMobs = new List<MapElement>();
            GameObject spawnableGameObject = null;
            MapElement spawnedElement = null;

            // HUMAN
            spawnableGameObject = GetSpawnableGameObject("human");
            
            spawnedElement = SpawnMapElement(spawnableGameObject, Vector3.zero, Quaternion.identity);
            spawnedElement.gameObject.name = "Guillermo";

            return generatedMobs;
        }
        */

    #endregion

        /// <summary>
        /// Look for all MapEvents available in the map that, as consequence of a MapEvent in an Attribute they own, they make a desired AttributeOwnership's value increase for the owner/executer/target (the needed participant).
        /// </summary>
        /// <param name="attributeOwnershipToCover"> The AttributeOwnership to increase the value of.</param>
        /// <param name="valueToCover">The amount of value needed to be covered (increased).</param>
        /// <param name="executer">Map element that is going to execute the ExecutionPlan.</param>
        /// <returns>The Execution Plan needed to achieve the goal (to increase the value of the attributeToCover by valueToCover)</returns>
        public ExecutionPlan GetExecutionPlanToCover([NotNull] AttributeOwnership attributeOwnershipToCover, int valueToCover, MapElement executer)
        {
            ExecutionPlan foundExecutionPlan = null;
            
            //Trying to cover with an attribute/mapEvent in the target
            foundExecutionPlan = attributeOwnershipToCover.owner.attributesManager.GetExecutionPlanToCover(attributeOwnershipToCover, valueToCover, executer);
                
            //Trying to cover with an attribute/mapEvent in the caregiver/executer
            if (foundExecutionPlan == null)
                foundExecutionPlan = executer.attributesManager.GetExecutionPlanToCover(attributeOwnershipToCover, valueToCover, executer);
            
            //Trying to cover with an attribute/mapEvent in any map element
            if (foundExecutionPlan == null)
                foreach (MapElement mapElement in existentMapElements)
                {
                    ExecutionPlan foundMapEvent = mapElement.attributesManager.GetExecutionPlanToCover(attributeOwnershipToCover, valueToCover, executer);
                    if (foundMapEvent != null)
                        return foundMapEvent;
                }

            return foundExecutionPlan;
        }
        
        
        GetHeightAtLocation
            
        GetTerrainTypeAtLocation
    }
}

enum TerrainType
{
    none = 0,
    sea,
    interior,
    interiorShoreline,
    land,
}
