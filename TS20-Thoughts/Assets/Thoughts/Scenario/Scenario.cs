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
    [RequireComponent(typeof(NavMeshSurface))]
    public class Scenario : MonoBehaviour
    {
        [Header("Map Elements")]
        [SerializeField] private GameObject water;

        [Header("Mobs")]
        [SerializeField] private GameObject human;

        private List<MapElement> mapElements = new List<MapElement>();
        private NavMeshSurface navMeshSurface;

        private void Awake()
        {
            navMeshSurface = this.GetComponentRequired<NavMeshSurface>();
        }

        public void BuildNew(List<Participant> participants)
        {
            GenerateScenario();
            navMeshSurface.BuildNavMesh();
            GenerateMobs();
        }
        
        private void GenerateScenario()
        {
            RandomEssentials random = new RandomEssentials();
            mapElements.Add(Instantiate(water, random.GetRandomVector3(-10f, 10f).WithY(0f), Quaternion.identity, this.transform).GetComponentRequired<MapElement>());
        }
        
        private void GenerateMobs()
        {
            Mob mob = Instantiate(human).GetComponentRequired<Mob>();
            mob.gameObject.name = "Guillermo";
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
    }
}
