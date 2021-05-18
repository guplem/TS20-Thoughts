using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Thoughts.ControlSystems;
using UnityEngine;
using UnityEngine.AI;

namespace Thoughts.Game.GameMap
{

    public class Map : MonoBehaviour
    {
        private List<MapElement> mapElements = new List<MapElement>();

    #region MapGeneration

        [SerializeField] private List<GameObject> spawnableMapElement;
        public void BuildNew(List<Participant> participants)
        {
            mapElements.AddRange(GenerateMapObjects());
            BuildNavMeshes();
            mapElements.AddRange(GenerateMobs());
        }

        private MapElement SpawnMapElement(GameObject spawnableGameObject, Vector3 position, Quaternion rotation)
        {
            GameObject spawnedMapElement = Instantiate(spawnableGameObject, position, rotation, this.transform);
            return spawnedMapElement.GetComponentRequired<MapElement>();
        }
        
        public GameObject GetSpawnableGameObject(string name)
        {
            foreach (GameObject go in spawnableMapElement)
            {
                //Debug.Log($"Looking for '{name}'. Searching now object '{go.name}'. Result = {string.Compare(go.name, name, StringComparison.OrdinalIgnoreCase)}");
                if (string.Compare(go.name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return go;
            }
            Debug.LogError($"The GameObject with name '{name}' could not be found in the list of spawnableGameObjects");
            spawnableMapElement.DebugLog(", ","Spawnable Game Objects: ");
            return null;
        }
        
        private List<MapElement> GenerateMapObjects()
        {
            List<MapElement> generatedMapObjects = new List<MapElement>();
            RandomEssentials random = new RandomEssentials();
            
            //generatedMapObjects.AddRange(SpawnRandom("river", 1, random));
            //generatedMapObjects.AddRange(SpawnRandom("rock", 10, random));
            generatedMapObjects.AddRange(SpawnRandom("tree", 1, random));
            generatedMapObjects.AddRange(SpawnRandom("bonfire", 1, random));
            //generatedMapObjects.AddRange(SpawnRandom("Berry Plant", 3, random));

            return generatedMapObjects;
        }
        private List<MapElement> SpawnRandom(string prefabName, int quantity, RandomEssentials random)
        {
            GameObject spawnableGameObject = GetSpawnableGameObject(prefabName);
            MapElement spawnedElement = null;
            List<MapElement> generatedMapObjects = new List<MapElement>();
            for (int i = 0; i < quantity; i++)
            {
                spawnedElement = SpawnMapElement(spawnableGameObject, random.GetRandomVector3(-40f, 40f).WithY(0f), Quaternion.identity);
                generatedMapObjects.Add(spawnedElement);
            }
            return generatedMapObjects;
        }

        private void BuildNavMeshes()
        {
            List<NavMeshSurface> generatedNavMeshSurfaces = new List<NavMeshSurface>();
            
            foreach (GameObject go in spawnableMapElement)
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

        public ExecutionPlan GetExecutionPlanToTakeCareOf([NotNull] OwnedAttribute ownedAttributeToTakeCare, int remainingValueToCover, MapElement caregiver)
        {
            if (ownedAttributeToTakeCare == null)
                throw new ArgumentNullException(nameof(ownedAttributeToTakeCare));
            
            ExecutionPlan foundExecutionPlan = null;
            
            //Trying to take care with an attribute/mapEvent in the target
            foundExecutionPlan = ownedAttributeToTakeCare.ownerMapElement.attributeManager.GetExecutionPlanToTakeCareOf(ownedAttributeToTakeCare, remainingValueToCover, caregiver);
                
            //Trying to take care with an attribute/mapEvent in the caregiver
            if (foundExecutionPlan == null)
                foundExecutionPlan = caregiver.attributeManager.GetExecutionPlanToTakeCareOf(ownedAttributeToTakeCare, remainingValueToCover, caregiver);
            
            //Trying to take care with an attribute/mapEvent in any map element
            if (foundExecutionPlan == null)
                foreach (MapElement mapElement in mapElements) // Todo: sort by distance
                {
                    ExecutionPlan foundMapEvent = mapElement.attributeManager.GetExecutionPlanToTakeCareOf(ownedAttributeToTakeCare, remainingValueToCover, caregiver);
                    if (foundMapEvent != null)
                        return foundMapEvent;
                }

            return foundExecutionPlan;
        }
        
    }
}
