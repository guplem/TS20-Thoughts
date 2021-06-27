using Thoughts.Game.GameMap;
using UnityEngine;

namespace Thoughts.ControlSystems.UI
{
    /// <summary>
    /// Controls the UI of the game
    /// </summary>
    public class GameUIManager : MonoBehaviour
    {
        //ToDo: redo UI Pov. Remove it.
        [SerializeField] private UIPovManager uiPovManager;

        /// <summary>
        /// Displays the UI related to the given MapElement
        /// </summary>
        /// <param name="mapElement">The MapElement from which you want to display the information in the UI.</param>
        public void DisplayUIFor(MapElement mapElement)
        {
            Debug.Log($"DisplayUIFor '{mapElement}'");
            uiPovManager.ShowUIFor(mapElement);
        }
    
    }
}
