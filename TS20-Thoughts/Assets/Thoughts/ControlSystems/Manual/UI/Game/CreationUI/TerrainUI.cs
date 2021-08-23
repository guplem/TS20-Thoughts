using Thoughts.Game;

public class TerrainUI : CreationStepUI // Like UserNameUI
{
    public void GenerateTerrain()
    {
        GameManager.instance.map.GenerateNew(); //Todo: Generate terrain, not all map
    }
}
