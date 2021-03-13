using System.Collections.Generic;
using Thoughts.Participants;
using Thoughts.ControlSystems;
using UnityEngine;

namespace Thoughts
{
    public class GameManager : MonoBehaviour
    {
        private readonly List<Participant> participants = new List<Participant>();

        public void Initialize()
        {
            participants.Add(new Participant(new Manual()));
        }
    }
}
