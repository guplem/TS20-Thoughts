using Thoughts.Game.Map;
using Thoughts.Utils.Inspector;
using Thoughts.Utils.Maths;
using UnityEngine;

[CreateAssetMenu(fileName = "FishAndBirdsSettings", menuName = "Thoughts/Map/Fish and Birds Settings", order = 50)]
public class FishAndBirdsSettings : UpdatableData
{
    public MapElementGenerationSettings[] fishMapElementsToSpawn;
    
    public MapElementGenerationSettings[] birdsMapElementsToSpawn;
}
