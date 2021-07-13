using System.Collections;
using System.Collections.Generic;
using Thoughts.Game.Attributes;
using TMPro;
using UnityEngine;

public class AttributeUI : MonoBehaviour
{

    private AttributeOwnership attributeOwnership;
    [SerializeField] private TMP_Text attributeText;
    
    public void Setup(AttributeOwnership attributeOwnership)
    {
        if (attributeOwnership == null)
            return;
        
        this.attributeOwnership = attributeOwnership;
        attributeText.text = attributeOwnership.attribute.name;
    }
    
}
