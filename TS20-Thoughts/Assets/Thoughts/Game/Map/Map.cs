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

        [SerializeField] private List<GameObject> spawnableMapElement;
        private List<MapElement> mapElements = new List<MapElement>();

        public void BuildNew(List<Participant> participants)
        {
            mapElements.AddRange(GenerateMapObjects());
            BuildNavMeshes();
            mapElements.AddRange(GenerateMobs());
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
        
        private List<MapElement> GenerateMapObjects()
        {
            List<MapElement> generatedMapObjects = new List<MapElement>();
            GameObject spawnableGameObject = null;
            MapElement spawnedElement = null;
            RandomEssentials random = new RandomEssentials();

            //Water
            spawnableGameObject = GetSpawnableGameObject("river");
            spawnedElement = SpawnMapElement(spawnableGameObject, random.GetRandomVector3(-10f, 10f).WithY(0f), Quaternion.identity);
            generatedMapObjects.Add(spawnedElement);
            
            //Rocks
            spawnableGameObject = GetSpawnableGameObject("rock");
            for (int i = 0; i < 30; i++)
            {
                spawnedElement = SpawnMapElement(spawnableGameObject, random.GetRandomVector3(-10f, 10f).WithY(0f), Quaternion.identity);
                generatedMapObjects.Add(spawnedElement);
            }

            return generatedMapObjects;
        }

        private MapElement SpawnMapElement(GameObject spawnableGameObject, Vector3 position, Quaternion rotation)
        {
            GameObject spawnedMapElement = Instantiate(spawnableGameObject, position, rotation, this.transform);
            return spawnedMapElement.GetComponentRequired<MapElement>();
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
        
        public MapEvent FindEventToCoverStat(Stat stat, MapElement needyMapElement, out MapElement mapElementWithActionToCoverNeed, out Attribute attributeWithEventToCoverNeed)
        {
            foreach (MapElement mapElement in mapElements)
            {
                //Debug.Log($"Analazing {element} with inventory {map} for the need {need}");
                MapEvent availableEvent = mapElement.attributeManager.GetAvailableActionToCoverNeed(stat, needyMapElement, out attributeWithEventToCoverNeed);
                if (availableEvent != null)
                {

                    mapElementWithActionToCoverNeed = mapElement;
                    return availableEvent;
                }
            }

            mapElementWithActionToCoverNeed = null;
            attributeWithEventToCoverNeed = null;
            return null;
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
    }
}
