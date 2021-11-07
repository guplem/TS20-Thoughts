using Thoughts.Utils.Inspector;
using UnityEngine;

namespace Thoughts.Game.Map.CreationSteps.Vegetation
{
    [CreateAssetMenu(fileName = "VegetationSettings", menuName = "Thoughts/Map/Vegetation Settings", order = 30)]
    public class VegetationSettings : UpdatableData
    {
        public MapElementGenerationSettings[] mapElementsToSpawn;
    }
}
