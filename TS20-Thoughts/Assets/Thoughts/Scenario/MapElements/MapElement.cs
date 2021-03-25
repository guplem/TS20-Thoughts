using UnityEngine;

namespace Thoughts.MapElements
{
     public abstract class MapElement : MonoBehaviour
     {
          [SerializeField] public Inventory inventory = new Inventory();
     }
}
