using Thoughts.Game.Map.MapElements.Attributes;
using TMPro;
using UnityEngine;

namespace Thoughts.Participants.ControlSystems.Manual.UI.Game.Shared
{
    /// <summary>
    /// Controls the display of a single Attribute in the UI 
    /// </summary>
    public class AttributeUI : MonoBehaviour
    {

        /// <summary>
        /// A reference to the text component that displays the name of the displayed Attribute
        /// </summary>
        [Tooltip("A reference to the text component that displays the name of the displayed Attribute")]
        [SerializeField] private TMP_Text attributeText;

        /// <summary>
        /// Displays the given Attribute in the UI. Disables the GameObject if the Attribute is null.
        /// </summary>
        /// <param name="attributeOwnership">The AttributeOwnership to display in the UI.</param>
        public void Setup(AttributeOwnership attributeOwnership)
        {
            bool display = attributeOwnership != null;

            gameObject.SetActive(display);

            if (display)
                attributeText.text = attributeOwnership.attribute.name;
        }

    }
}