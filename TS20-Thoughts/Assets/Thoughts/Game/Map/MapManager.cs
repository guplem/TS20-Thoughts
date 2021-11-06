using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Thoughts.Game.Map.MapElements;
using Thoughts.Game.Map.MapElements.Attributes;
using Thoughts.Game.Map.MapElements.Attributes.MapEvents;
using Thoughts.Game.Map.Terrain;
using Thoughts.Utils.Maths;
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
        /// Reference to the MapConfiguration with he settings to generate a map
        /// </summary>
        [Tooltip("Reference to the MapConfiguration with he settings to generate a map")]
        [SerializeField] public MapConfiguration mapConfiguration;
        
        /// <summary>
        /// Reference to the MapGenerator component, the manager of the generation of the map
        /// </summary>
        internal MapGenerator mapGenerator { get {
            if (_mapGenerator == null) _mapGenerator = this.GetComponentRequired<MapGenerator>();
            return _mapGenerator;
        } }
        private MapGenerator _mapGenerator;
        
        /// <summary>
        /// All the map elements present in the map.
        /// </summary>
        [NonSerialized] public List<MapElement> existentMapElements = new List<MapElement>();
        
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
            mapGenerator.RegenerateFullMap();
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
            mapGenerator.DeleteMap();
            existentMapElements.Clear();
        }

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
        
        public float GetHeightAt(Vector2 location)
        {
            return mapGenerator.terrainGenerator.GetHeightAt(location);
        }
        
        public bool IsLocationUnderWater(Vector2 worldCoords)
        {
            return GetHeightAt(worldCoords) < mapConfiguration.seaHeightAbsolute;
        } 

        public TerrainType GetTerrainTypeAtLocation(Vector2 location)
        {
            throw new NotImplementedException();
        }

        public List<MapElement> SpawnMapElementsRandomly(GameObject objectToSpawn, int seed, Vector2 spawningHeightRange, int quantity, Transform parent, bool requireNavMesh)
        {
            return mapGenerator.SpawnMapElementsRandomly(objectToSpawn, seed, spawningHeightRange, quantity, parent, requireNavMesh);
        }
        
        public MapElement SpawnMapElement(GameObject objectToSpawn, Vector3 position, Quaternion rotation, Transform parent)
        {
            return mapGenerator.SpawnMapElement(objectToSpawn, position, rotation, parent);
        }

        public List<MapElement> SpawnMapElementsWithPerlinNoiseDistribution(GameObject objectToSpawn, int seed, Vector2 spawningHeightRange, float probability, float density, Transform parent, NoiseMapSettings noiseMapSettings, bool requireNavMesh)
        {
            return mapGenerator.SpawnMapElementsWithPerlinNoiseDistribution(objectToSpawn, seed,  spawningHeightRange, probability, density, parent, noiseMapSettings, requireNavMesh);
        }
    }
}

public enum TerrainType
{
    none = 0,
    sea,
    interior,
    interiorShoreline,
    land,
}
