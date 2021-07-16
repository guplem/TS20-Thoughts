using Thoughts.Game.GameMap;
using TMPro;
using UnityEngine;

namespace Thoughts.ControlSystems.UI
{
    public class DescriptionUI : MonoBehaviour
    {
        /// <summary>
        /// A reference to the text component that displays the name of the displayed MapElement
        /// </summary>
        [Tooltip("A reference to the text component that displays the name of the displayed MapElement")]
        [SerializeField] private TMP_Text mapElementNameText;

        /// <summary>
        /// A reference to the text component that displays the description of the displayed MapElement
        /// </summary>
        [Tooltip("A reference to the text component that displays the description of the displayed MapElement")]
        [SerializeField] private TMP_Text mapElementDescriptionText;

        /// <summary>
        /// Displays the the information of the given MapElement in the UI
        /// </summary>
        /// <param name="selectedMapElement">The MapElement to display the information of in the UI.</param>
        public void DisplayInformationOf(MapElement selectedMapElement)
        {
            mapElementNameText.text = selectedMapElement.name;
            mapElementDescriptionText.text = $"Description of the selected MapElement:\n {selectedMapElement.ToString()}";
        }
    }
}