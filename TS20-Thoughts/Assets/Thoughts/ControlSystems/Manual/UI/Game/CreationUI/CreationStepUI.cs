using System.Collections;
using System.Collections.Generic;
using Thoughts.ControlSystems.UI;
using UnityEngine;

public abstract class CreationStepUI : SimpleAnimationUIElement
{
    public CreationStepUI nextStepMainPanel;
    
    public void ContinueToNextStep()
    {
        Debug.Log("Continuing to the next creation step");
        this.Hide();
        nextStepMainPanel.Show();
    }
}
