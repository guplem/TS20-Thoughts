using UnityEngine;

namespace Thoughts.ControlSystems
{
    public abstract class ControlSystem: MonoBehaviour
    {
        public Participant controlledParticipant { get; private set; }
        
        public virtual void Initialize(Participant participant)
        {
            controlledParticipant = participant;
        }
        
        public virtual void Disable()
        {
            controlledParticipant = null;
        }
    }
}
