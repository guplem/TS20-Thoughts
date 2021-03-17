using System;
using System.Collections.Generic;
using Thoughts.MapElements;
using Thoughts.Mobs;
using Thoughts.Needs;
using UnityEngine;
using UnityEngine.AI;
using NavMeshBuilder = UnityEditor.AI.NavMeshBuilder;

namespace Thoughts
{

    public class Scenario : MonoBehaviour
    {

        [SerializeField] private List<GameObject> spawnableGameObjects;

        private List<MapElement> mapElements = new List<MapElement>();

        public void BuildNew(List<Participant> participants)
        {
            GenerateScenario();
            BuildNavMeshes();
            List<Mob> generatedMobs = GenerateMobs();
        }
        
        private void BuildNavMeshes()
        {
            // ToDo: Check if any navmesh for the same type of agent already exist
            foreach (GameObject go in spawnableGameObjects)
            {
                NavMeshAgent mobAgent = go.GetComponent<NavMeshAgent>();
                if (mobAgent == null)
                    continue;
                NavMeshSurface navMeshSurface = this.gameObject.AddComponent<NavMeshSurface>();
                navMeshSurface.agentTypeID = mobAgent.agentTypeID;
                navMeshSurface.BuildNavMesh();
            }
        }

        private void GenerateScenario()
        {
            RandomEssentials random = new RandomEssentials();
            GameObject spawnableGameObject = GetSpawnableGameObject("water");
            mapElements.Add(Instantiate(spawnableGameObject, random.GetRandomVector3(-10f, 10f).WithY(0f), Quaternion.identity, this.transform).GetComponentRequired<MapElement>());
        }
        
        private List<Mob> GenerateMobs()
        {
            List<Mob> generatedMobs = new List<Mob>();
            
            GameObject spawnableGameObject = GetSpawnableGameObject("human");
            Mob mob = Instantiate(spawnableGameObject).GetComponentRequired<Mob>();
            mob.gameObject.name = "Guillermo";
            generatedMobs.Add(mob);

            return generatedMobs;
        }
        
        public MapElement FindElementToCoverNeed(Need need, out Item itemToCoverNeed)
        {
            foreach (MapElement element in mapElements)
            {
                //Debug.Log($"Analazing {element} with inventory {map} for the need {need}");
                if (element.inventory.ContainsItemToCoverNeed(need, out itemToCoverNeed))
                    return element;
            }
            
            itemToCoverNeed = null;
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
