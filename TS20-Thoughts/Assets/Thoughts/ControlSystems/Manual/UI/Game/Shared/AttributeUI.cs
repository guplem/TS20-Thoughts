using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.Attributes;
using TMPro;
using UnityEngine;

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
