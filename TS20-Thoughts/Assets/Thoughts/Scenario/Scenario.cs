using System.Collections.Generic;
using Essentials.Scripts.Extensions;
using Thoughts.Mobs;
using UnityEngine;

namespace Thoughts
{
    public class Scenario : MonoBehaviour
    {
        [SerializeField] private GameObject human;
        public void BuildNew(List<Participant> participants)
        {
            Mob mob = Instantiate(human).GetComponentRequired<Mob>();
            mob.gameObject.name = "HUMAN TEST";
            Instantiate(human).GetComponentRequired<Mob>();
        }
    }
}
