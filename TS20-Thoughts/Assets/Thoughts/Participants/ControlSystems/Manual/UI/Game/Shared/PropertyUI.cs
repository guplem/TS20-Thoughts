using System;
using System.Globalization;
using Thoughts.Game.Map.MapElements.Properties;
using Thoughts.Game.Map.Properties;
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
        [SerializeField] private TMP_Text propertyName;
        
        /// <summary>
        /// A reference to the text component that displays the value of the displayed Property
        /// </summary>
        [Tooltip("A reference to the text component that displays the value of the displayed Property")]
        [SerializeField] private TMP_Text propertyValue;
        
        /// <summary>
        /// The displayed property ownership
        /// </summary>
        private PropertyOwnership propertyOwnership;

        /// <summary>
        /// Displays the given Property in the UI. Disables the GameObject if the Property is null.
        /// </summary>
        /// <param name="propertyOwnershipp">The PropertyOwnership to display in the UI.</param>
        public void Setup(PropertyOwnership propertyOwnership)
        {
            bool display = propertyOwnership != null;

            gameObject.SetActive(display);

            this.propertyOwnership = propertyOwnership;

            if (display)
            {
                propertyName.text = propertyOwnership.property.name;
            }
        }

        private void Update()
        {
            propertyValue.text = this.propertyOwnership.value.ToString(CultureInfo.InvariantCulture);
        }

    }
}