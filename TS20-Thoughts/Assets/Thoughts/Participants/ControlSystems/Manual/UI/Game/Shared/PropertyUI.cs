using Thoughts.Game.Map.MapElements.Properties;
using TMPro;
using UnityEngine;

namespace Thoughts.Participants.ControlSystems.Manual.UI.Game.Shared
{
    /// <summary>
    /// Controls the display of a single Property in the UI 
    /// </summary>
    public class PropertyUI : MonoBehaviour
    {

        /// <summary>
        /// A reference to the text component that displays the name of the displayed Property
        /// </summary>
        [Tooltip("A reference to the text component that displays the name of the displayed Property")]
        [SerializeField] private TMP_Text propertyText;

        /// <summary>
        /// Displays the given Property in the UI. Disables the GameObject if the Property is null.
        /// </summary>
        /// <param name="propertyOwnershipp">The PropertyOwnership to display in the UI.</param>
        public void Setup(PropertyOwnership propertyOwnership)
        {
            bool display = propertyOwnership != null;

            gameObject.SetActive(display);

            if (display)
                propertyText.text = propertyOwnership.property.name;
        }

    }
}