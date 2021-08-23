using Thoughts.Game;

public class UserNameUI : CreationStepUI
{
    
    public void SetNewName(string newName)
    {
        GameManager.instance.localManualParticipant.name = newName;
    }
    
}
