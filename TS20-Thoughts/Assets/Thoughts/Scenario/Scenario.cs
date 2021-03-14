using System.Collections.Generic;
using Essentials.Scripts.Extensions;
using Thoughts.MapElements;
using Thoughts.Mobs;
using UnityEngine;

namespace Thoughts
{
    public class Scenario : MonoBehaviour
    {
        [Header("Map Elements")]
        [SerializeField] private GameObject water;

        [Header("Mobs")]
        [SerializeField] private GameObject human;

        private List<MapElement> mapElements = new List<MapElement>();

        public void BuildNew(List<Participant> participants)
        {
            RandomEssentials random = new RandomEssentials();
            
            
            Mob mob = Instantiate(human).GetComponentRequired<Mob>();
            mob.gameObject.name = "Guillermo";
            
            mapElements.Add(Instantiate(water, random.GetRandomVector3(-10f, 10f).WithY(0f), Quaternion.identity, this.transform).GetComponentRequired<MapElement>());
        }
    }
}
