using Thoughts.Game.Map;
using Thoughts.Utils.Inspector;
using Thoughts.Utils.Maths;
using UnityEngine;
[CreateAssetMenu(fileName = "LandAnimalsSettings", menuName = "Thoughts/Map/Land Animals Settings", order = 60)]
public class LandAnimalsSettings : UpdatableData
{
    public MapElementGenerationSettings[] mapElementsToSpawn;
}
