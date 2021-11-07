using Thoughts.Utils.Inspector;
using UnityEngine;

namespace Thoughts.Game.Map.CreationSteps.FishAndBirds
{
    [CreateAssetMenu(fileName = "FishAndBirdsSettings", menuName = "Thoughts/Map/Fish and Birds Settings", order = 50)]
    public class FishAndBirdsSettings : UpdatableData
    {
        public MapElementGenerationSettings[] fishMapElementsToSpawn;
    
        public MapElementGenerationSettings[] birdsMapElementsToSpawn;
    }
}
