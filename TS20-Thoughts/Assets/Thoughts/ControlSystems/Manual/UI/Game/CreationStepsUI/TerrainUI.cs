using Thoughts.Game;

namespace Thoughts.ControlSystems.UI.CreationSteps
{
    public class TerrainUI : CreationStepUI // Like UserNameUI
    {
        public void GenerateTerrain()
        {
            GameManager.instance.map.GenerateNew(); //Todo: Generate terrain, not all map
        }
    }
}