using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Thoughts.ControlSystems;
using Thoughts.Game.Attributes;
using UnityEngine;
using UnityEngine.AI;

namespace Thoughts.Game.GameMap
{

    /// <summary>
    /// The map of the game.
    /// </summary>
    public class Map : MonoBehaviour
    {
        /// <summary>
        /// All the map elements present in the map.
        /// </summary>
        private List<MapElement> mapElements = new List<MapElement>();

    #region MapGeneration

        /// <summary>
        /// All the map elements' prefabs that can be spawned in the map.
        /// </summary>
        [SerializeField] private List<GameObject> spawnableMapElements;
        
        /// <summary>
        /// Generates a new map.
        /// </summary>
        /// <param name="participants">The participants of the game that must be in the map.</param>
        public void GenerateNew(List<Participant> participants)
        {
            //Todo: use the "participants" 
            mapElements.AddRange(GenerateMapElements());
            SetupNewNavMeshes();
            mapElements.AddRange(GenerateMobs());
        }

        /// <summary>
        /// Makes a map element spawn.
        /// </summary>
        /// <param name="spawnableGameObject">The map element's prefab to spawn</param>
        /// <param name="position">Position for the new object.</param>
        /// <param name="rotation">Orientation of the new object.</param>
        /// <returns></returns>
        private MapElement SpawnMapElement(GameObject spawnableGameObject, Vector3 position, Quaternion rotation)
        {
            GameObject spawnedMapElement = Instantiate(spawnableGameObject, position, rotation, this.transform);
            return spawnedMapElement.GetComponentRequired<MapElement>();
        }
        
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
        }
        
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
        
        /// <summary>
        /// Randomly generates the MapElement with the given name a determined amount of times.
        /// <para> The MapElement's prefab is obtained from the spawnableMapElement list.</para>
        /// </summary>
        /// <param name="prefabName">The name of the GameObject to generate.</param>
        /// <param name="quantity">The amount of GameObjects to generate.</param>
        /// <param name="random">Random object to be used in the process.</param>
        /// <returns></returns>
        private List<MapElement> SpawnRandom(string prefabName, int quantity, RandomEssentials random)
        {
            GameObject spawnableGameObject = GetSpawnableGameObject(prefabName);
            MapElement spawnedElement = null;
            List<MapElement> generatedMapObjects = new List<MapElement>();
            for (int i = 0; i < quantity; i++)
            {
                spawnedElement = SpawnMapElement(spawnableGameObject, random.GetRandomVector3(-10f, 10f).WithY(0f), Quaternion.identity);
                generatedMapObjects.Add(spawnedElement);
            }
            return generatedMapObjects;
        }

        /// <summary>
        /// Sets up the NavMesh for all the NavMeshAgents in the spawnableMapElement list.
        /// </summary>
        private void SetupNewNavMeshes()
        {
            List<NavMeshSurface> generatedNavMeshSurfaces = new List<NavMeshSurface>();
            
            foreach (GameObject go in spawnableMapElements)
            {
                NavMeshAgent mobAgent = go.GetComponent<NavMeshAgent>();
                
                if (mobAgent == null)
                    continue;
                
                bool repeatedAgent = false;
                foreach (NavMeshSurface generatedNavMeshSurface in generatedNavMeshSurfaces)
                    if (generatedNavMeshSurface.agentTypeID == mobAgent.agentTypeID)
                        repeatedAgent = true;
                
                if (repeatedAgent)
                    continue;
                
                NavMeshSurface navMeshSurface = this.gameObject.AddComponent<NavMeshSurface>();
                navMeshSurface.agentTypeID = mobAgent.agentTypeID;
                navMeshSurface.BuildNavMesh();
                //navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData); //To update the whole NavMesh at runtime
                generatedNavMeshSurfaces.Add(navMeshSurface);

            }
        }

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
            generatedMobs.Add(spawnedElement);
            //ToDo: add ownership

            return generatedMobs;
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
            foundExecutionPlan = attributeOwnershipToCover.owner.attributeManager.GetExecutionPlanToCover(attributeOwnershipToCover, valueToCover, executer);
                
            //Trying to cover with an attribute/mapEvent in the caregiver/executer
            if (foundExecutionPlan == null)
                foundExecutionPlan = executer.attributeManager.GetExecutionPlanToCover(attributeOwnershipToCover, valueToCover, executer);
            
            //Trying to cover with an attribute/mapEvent in any map element
            if (foundExecutionPlan == null)
                foreach (MapElement mapElement in mapElements) // Todo: sort by distance
                {
                    ExecutionPlan foundMapEvent = mapElement.attributeManager.GetExecutionPlanToCover(attributeOwnershipToCover, valueToCover, executer);
                    if (foundMapEvent != null)
                        return foundMapEvent;
                }

            return foundExecutionPlan;
        }
        
    }
}
