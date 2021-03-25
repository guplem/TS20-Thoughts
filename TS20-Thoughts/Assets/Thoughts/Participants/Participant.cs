using UnityEngine;

namespace Thoughts.ControlSystems
{
    [RequireComponent(typeof(ControlSystem))]
    public class Participant
    {
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
        
        public Participant(GameObject controlSystem)
        {
            this.controlSystem = controlSystem.GetComponentRequired<ControlSystem>();
        }
    }
}