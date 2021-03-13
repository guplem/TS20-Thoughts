using System.Collections.Generic;
using UnityEngine;

namespace Thoughts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject manualControlSystemPrefab;
        private readonly List<Participant> participants = new List<Participant>();

        public void Initialize()
        {
            GameObject controlSystem = Instantiate(manualControlSystemPrefab);
            participants.Add(new Participant(controlSystem));
        }
    }
}
