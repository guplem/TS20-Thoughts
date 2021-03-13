using Thoughts.ControlSystems;

namespace Thoughts.Participants
{
    public class Participant
    {
        private ControlSystem controlSystem;
        public Participant(ControlSystem controlSystem)
        {
            this.controlSystem = controlSystem;
        }
    }
}
