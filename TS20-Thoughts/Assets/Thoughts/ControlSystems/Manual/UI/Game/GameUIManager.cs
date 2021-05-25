using Thoughts.Game.GameMap;
using UnityEngine;

namespace Thoughts.ControlSystems.UI
{
    public class GameUIManager : MonoBehaviour
    {
        [SerializeField] private UIPovManager uiPovManager;

        public void DisplayUIFor(MapElement mapElement)
        {
            Debug.Log($"DisplayUIFor '{mapElement}'");
            uiPovManager.ShowUIFor(mapElement);
        }
    
    }
}
