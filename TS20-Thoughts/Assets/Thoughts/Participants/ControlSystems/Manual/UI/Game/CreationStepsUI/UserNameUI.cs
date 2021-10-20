using Thoughts.Game;

namespace Thoughts.ControlSystems.UI.CreationSteps
{
    public class UserNameUI : CreationStepUI
    {

        public void SetNewName(string newName)
        {
            GameManager.instance.localManualParticipant.name = newName;
        }

    }
}