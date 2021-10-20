using Thoughts.Game;

namespace Thoughts.Participants.ControlSystems.Manual.UI.Game.CreationStepsUI
{
    public class TerrainUI : CreationStepUI // Like UserNameUI
    {
        public void GenerateTerrain()
        {
            GameManager.instance.mapManager.GenerateNew(); //Todo: Generate terrain, not all map
        }
    }
}