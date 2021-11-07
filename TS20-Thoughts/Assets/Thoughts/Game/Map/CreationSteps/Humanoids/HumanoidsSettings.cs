using GD.MinMaxSlider;
using Thoughts.Utils.Inspector;
using UnityEngine;

namespace Thoughts.Game.Map.CreationSteps.Humanoids
{
    [CreateAssetMenu(fileName = "HumanoidsSettings", menuName = "Thoughts/Map/Humanoids Settings", order = 70)]
    public class HumanoidsSettings: UpdatableData
    {
        /// <summary>
        /// In which area of the map this is wanted to spawn. -1 means the bottom of the sea. 1 means the highest points in the world. 0 is the shoreline.
        /// </summary>
        [Tooltip("In which area of the map this is wanted to spawn. -1 means the bottom of the sea. 1 means the highest points in the world. 0 is the shoreline.")]
        [MinMaxSlider(-1,1)]
        public Vector2 spawningHeightRange;

        /// <summary>
        /// Amount of humans to spawn
        /// </summary>
        [Tooltip("Amount of humans to spawn")]
        [SerializeField] public int quantity = 2;
        
        public GameObject[] spawnableMapElements;
        
    }
}
