using System.Collections.Generic;
using Thoughts.ControlSystems;
using UnityEngine;

namespace Thoughts.Game
{
    public class GameManager : MonoBehaviour
    {
        [Header("Control Systems")]
        [SerializeField] private GameObject manualControlSystemPrefab;
        
        [Header("Game Elements")]
        [SerializeField] public GameMap.Map map;
        
        private readonly List<Participant> participants = new List<Participant>();

        public void StartNewGame()
        {
            // Create a manual control system
            GameObject controlSystem = Instantiate(manualControlSystemPrefab);
            
            // Add a participant to the game (controlled manually)
            participants.Add(new Participant(controlSystem));
            
            // Build a new scenario
            map.BuildNew(participants);
        }
    }
}
