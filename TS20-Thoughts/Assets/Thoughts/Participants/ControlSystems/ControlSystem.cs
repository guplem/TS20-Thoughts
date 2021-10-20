using UnityEngine;

namespace Thoughts.Participants.ControlSystems
{
    /// <summary>
    /// A way of controlling a participant.
    /// </summary>
    public abstract class ControlSystem: MonoBehaviour
    {
        /// <summary>
        /// Tha participant using this ControlSystem
        /// </summary>
        public Participant controlledParticipant { get; private set; }
        
        /// <summary>
        /// Initializes the ControlSystem by giving it reference to the participant that will use it.
        /// </summary>
        /// <param name="participant"></param>
        public virtual void Initialize(Participant participant)
        {
            controlledParticipant = participant;
        }
        
        /// <summary>
        /// Disables the ControlSystem by removing reference to the controlled Participant.
        /// </summary>
        public virtual void Disable()
        {
            controlledParticipant = null;
        }
    }
}
