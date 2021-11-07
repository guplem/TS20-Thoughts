using Thoughts.Utils.Inspector;
using UnityEngine;

namespace Thoughts.Game.Map.CreationSteps.LandAnimals
{
    [CreateAssetMenu(fileName = "LandAnimalsSettings", menuName = "Thoughts/Map/Land Animals Settings", order = 60)]
    public class LandAnimalsSettings : UpdatableData
    {
        public MapElementGenerationSettings[] mapElementsToSpawn;
    }
}
