using UnityEngine;

namespace Thoughts.ControlSystems
{
    /// <summary>
    /// Each of the members of the the game.
    /// </summary>
    public class Participant
    {
        /// <summary>
        /// The control system of the participant.
        /// </summary>
        public ControlSystem controlSystem
        {
            get
            {
                return _controlSystem;
            }
            private set
            {
                if (_controlSystem != null)
                    _controlSystem.Disable();
                
                _controlSystem = value;
                
                if (_controlSystem != null)
                    _controlSystem.Initialize(this);
            }
        }
        private ControlSystem _controlSystem;
        
        /// <summary>
        /// The class constructor.
        /// </summary>
        /// <param name="controlSystem">The control system of the participant.</param>
        public Participant(GameObject controlSystem)
        {
            this.controlSystem = controlSystem.GetComponentRequired<ControlSystem>();
        }
    }
}