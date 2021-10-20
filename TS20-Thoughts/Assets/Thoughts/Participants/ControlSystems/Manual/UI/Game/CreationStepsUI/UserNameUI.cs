using Thoughts.Game;

namespace Thoughts.Participants.ControlSystems.Manual.UI.Game.CreationStepsUI
{
    public class UserNameUI : CreationStepUI
    {

        public void SetNewName(string newName)
        {
            GameManager.instance.localManualParticipant.name = newName;
        }

    }
}