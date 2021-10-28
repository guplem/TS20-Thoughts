using Thoughts.Utils.Inspector;
using Thoughts.Utils.Maths;
using UnityEngine;

namespace Thoughts.Game.Map
{
    [CreateAssetMenu(fileName = "VegetationSettings", menuName = "Thoughts/Map/Vegetation Settings", order = 30)]
    public class VegetationSettings : UpdatableData
    {
        public MapElementGenerationSettings[] mapElementsToSpawn;
    }
}
