using System.Collections;
using System.Collections.Generic;
using Thoughts;
using UnityEngine;

public class UserNameUI : CreationStepUI
{
    
    public void SetNewName(string newName)
    {
        AppManager.gameManager.localManualParticipant.name = newName;
    }
    
}
