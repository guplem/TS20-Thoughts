using Thoughts.Participants.ControlSystems.Manual.UI.Game.SelectionUI;
using UnityEngine;

namespace Thoughts.Participants.ControlSystems.Manual.UI.Game.CreationStepsUI
{

    public abstract class CreationStepUI : SimpleAnimationUIElement
    {
        public CreationStepUI nextStepMainPanel;

        public void ContinueToNextStep()
        {
            Debug.Log("Continuing to the next creation step");

            this.Hide();
            if (nextStepMainPanel != null)
            {
                nextStepMainPanel.Show();
            }
        }
    }
}