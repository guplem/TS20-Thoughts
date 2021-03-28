using System;
using System.Collections.Generic;
using Thoughts.Game.GameMap;
using UnityEngine;
using Attribute = Thoughts.Attribute;

public class UIAttributesManager : MonoBehaviour
{
    [SerializeField] private GameObject UIAttributePrefab;
    private List<UIAttributeController> uiAttributeControllers = new List<UIAttributeController>();
    
    public void ShowUIFor(MapElement mapElement)
    {
        this.gameObject.SetActive(mapElement != null);
        if (mapElement == null)
            return;

        Clear();
        
        foreach (Attribute attribute in mapElement.attributeManager.attributes)
        {
            UIAttributeController attributeController = Instantiate(UIAttributePrefab, this.transform).GetComponentRequired<UIAttributeController>();
            attributeController.Initialize(attribute);
            uiAttributeControllers.Add(attributeController);
        }
    }
    
    private void Clear()
    {
        this.transform.DestroyAllChildren();
        uiAttributeControllers.Clear();
    }
    
}
