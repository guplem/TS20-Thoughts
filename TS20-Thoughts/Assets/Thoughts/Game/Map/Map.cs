using System;
using System.Collections.Generic;
using Thoughts.ControlSystems;
using Thoughts.Needs;
using UnityEngine;
using UnityEngine.AI;

namespace Thoughts.Game.GameMap
{

    public class Map : MonoBehaviour
    {

        [SerializeField] private List<GameObject> spawnableGameObjects;

        private List<MapElement> mapElements = new List<MapElement>();

        public void BuildNew(List<Participant> participants)
        {
            GenerateScenario();
            BuildNavMeshes();
            List<MapElement> generatedMobs = GenerateMobs();
        }
        
        private void BuildNavMeshes()
        {
            List<NavMeshSurface> generatedNavMeshSurfaces = new List<NavMeshSurface>();
            
            foreach (GameObject go in spawnableGameObjects)
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

        private void GenerateScenario()
        {
            RandomEssentials random = new RandomEssentials();
            GameObject spawnableGameObject = null;
            
            //Water
            spawnableGameObject = GetSpawnableGameObject("river");
            SpawnMapElement(spawnableGameObject, random.GetRandomVector3(-10f, 10f).WithY(0f), Quaternion.identity);
            
            //Rocks
            spawnableGameObject = GetSpawnableGameObject("rock");
            for (int i = 0; i < 30; i++)
                SpawnMapElement(spawnableGameObject, random.GetRandomVector3(-10f, 10f).WithY(0f), Quaternion.identity);
            
        }
        private void SpawnMapElement(GameObject spawnableGameObject, Vector3 position, Quaternion rotation)
        {
            GameObject spawnedMapElement = Instantiate(spawnableGameObject, position, rotation, this.transform);
            mapElements.Add(spawnedMapElement.GetComponentRequired<MapElement>());
        }

        private List<MapElement> GenerateMobs()
        {
            List<MapElement> generatedMobs = new List<MapElement>();
            
            GameObject spawnableGameObject = GetSpawnableGameObject("human");
            MapElement mapElement = Instantiate(spawnableGameObject).GetComponentRequired<MapElement>();
            mapElement.gameObject.name = "Guillermo";
            generatedMobs.Add(mapElement);
            //ToDo: add ownership

            return generatedMobs;
        }
        
        public MapAction FindActionToCoverNeed(Need need, out Vector3 positionToPerformAction)
        {
            foreach (MapElement mapElement in mapElements)
            {
                //Debug.Log($"Analazing {element} with inventory {map} for the need {need}");
                MapAction availableAction = mapElement.inventory.GetAvailableActionToCoverNeed(need, mapElement, out positionToPerformAction);
                if (availableAction != null)
                    return availableAction;
            }
            
            positionToPerformAction = Vector3.zero;
            return null;
        }

        public GameObject GetSpawnableGameObject(string name)
        {
            foreach (GameObject go in spawnableGameObjects)
            {
                //Debug.Log($"Looking for '{name}'. Searching now object '{go.name}'. Result = {string.Compare(go.name, name, StringComparison.OrdinalIgnoreCase)}");
                if (string.Compare(go.name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return go;
            }
            Debug.LogError($"The GameObject with name '{name}' could not be found in the list of spawnableGameObjects");
            spawnableGameObjects.DebugLog(", ","Spawnable Game Objects: ");
            return null;
        }
    }
}
