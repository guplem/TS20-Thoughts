using Thoughts.Utils.Inspector;
using UnityEngine;

namespace Thoughts.Game.Map
{
    public class HumanoidsSettings: UpdatableData
    {
        /// <summary>
        /// How close the vegetation can be to the sea shore.
        /// </summary>
        [Tooltip("How close the vegetation can be to the sea shore. 1 means that it can get in the sea. 0.5 means a long distance to the sea shore.")]
        [Range(0,1)]
        [SerializeField] public float closenessToShore = 0.993f; //[0,1], 1 being that the vegetation can get on the sea

        /// <summary>
        /// Amount of humans to spawn
        /// </summary>
        [Tooltip("Amount of humans to spawn")]
        [SerializeField] public int quantity = 2;
        
        
        public GameObject[] spawnableMapElements;
        
        
    }
}
