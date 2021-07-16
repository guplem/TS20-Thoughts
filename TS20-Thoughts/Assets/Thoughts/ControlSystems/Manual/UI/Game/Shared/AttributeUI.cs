using Thoughts.Game.Attributes;
using TMPro;
using UnityEngine;

namespace Thoughts.ControlSystems.UI
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
        /// Displays the given Attribute in the UI 
        /// </summary>
        /// <param name="attributeOwnership">The AttributeOwnership to display in the UI.</param>
        public void Setup(AttributeOwnership attributeOwnership)
        {
            if (attributeOwnership == null)
                return;

            attributeText.text = attributeOwnership.attribute.name;
        }

    }
}